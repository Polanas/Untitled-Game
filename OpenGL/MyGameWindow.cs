#define ENABLE_LOGGING
#region stuff

using Box2DX.Collision;
using Box2DX.Common;
using Box2DX.Dynamics;
using Leopotam.EcsLite.Di;
using Leopotam.EcsLite.ExtendedSystems;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using System.Reflection;
using System.Runtime.InteropServices;

namespace Game;

class MyGameWindow : GameWindow
{

    public static Vector2i ScreenSize { get; private set; }

    public static Vector2 FullToPixelatedRatio { get; private set; }

    public static TextWriter LogWriter => _writer;

    public EcsWorld world;

    public SharedData sharedData;

    private EcsSystems _gameSystems;

    private EcsSystems _renderSystems;

    private EcsSystems _physicsSystems;

    private EcsSystems _networkSystems;

    private double _summedRenderTime;

    private double _lastElapsedRenderTime;

    private double _lastTIme;

    private double _lastTIme2;

    private double _elapsedUpdateTime;

    private double _elapsedUpdateTime2;

    private int _framesCount;

    private FileStream _logFile;

    private const string USELESS_LOG = "will use VIDEO memory as the source for buffer object operations.";

    public static TextWriter _writer;

    private static DebugProc _debugProcCallback = DebugCallBack;

    private static GCHandle _debugProcCallbackHandle;

    public MyGameWindow(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings) :
     base(gameWindowSettings, nativeWindowSettings)
    {
        UpdateFrame += OnUpdate;
        RenderFrame += OnRender;

#if ENABLE_LOGGING
        InitFileLogger();
#endif
        Init();
    }

    protected override void Dispose(bool disposing)
    {
        _renderSystems.Destroy();
        _physicsSystems.Destroy();
        _networkSystems.Destroy();
        _gameSystems.Destroy();

        base.Dispose(disposing);

#if ENABLE_LOGGING
        _writer.Dispose();
        _logFile.Dispose();
#endif
    }

    protected void OnUpdate(FrameEventArgs e)
    {
        _networkSystems.Run();
        _gameSystems.Run();
        _physicsSystems.Run();

        if (Keyboard.Pressed(Keys.R))
            ReloadShaders(@"Content\Shaders");

        if (KeyboardState.IsKeyPressed(Keys.Escape))
            Close();
    }

    protected void OnRender(FrameEventArgs args)
    {

#if RELEASE
        if (!IsFocused)
            return;
#endif

#if DEBUG
        double lastTime = GLFW.GetTime();
        _framesCount++;
#endif

        GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

        _renderSystems.Run();

        SwapBuffers();

#if DEBUG
        double currentTime = GLFW.GetTime();
        double elapsed = currentTime - _lastElapsedRenderTime;

        _summedRenderTime += elapsed;

        if (elapsed >= 1.0)
        {
            double averageTime = _summedRenderTime / _framesCount;
            Title = $"Average rendering time: {averageTime} ms";
            _lastElapsedRenderTime += elapsed;
        }
#endif
    }

    private void InitFileLogger()
    {
        var currentDateTime = DateTime.Now.ToString().Replace(":", ".");

        _logFile = File.Create($@"Logs\{currentDateTime}.txt");
        _writer = new StreamWriter(_logFile);

        _debugProcCallbackHandle = GCHandle.Alloc(_debugProcCallback);

        GL.DebugMessageCallback(_debugProcCallback, IntPtr.Zero);
        GL.Enable(EnableCap.DebugOutput);
        GL.Enable(EnableCap.DebugOutputSynchronous);
    }

    private static void DebugCallBack(
       DebugSource source,
       DebugType type,
       int id,
       DebugSeverity severity,
       int length,
       IntPtr message,
       IntPtr userParam)
    {
        string messageStr = Marshal.PtrToStringAnsi(message, length);

        if (messageStr.Contains(USELESS_LOG))
            return;

        _writer.WriteLine($"{severity} {type} | {messageStr}");
    }

    #endregion

    private void Init()
    {
#if DEBUG //TODO: this is temp
        //  WindowState = WindowState.Fullscreen;
#endif
        ScreenSize = new Vector2i(1920, 1080);
        GL.ClearColor(System.Drawing.Color.FromArgb(0, 100, 100, 100));

        FullToPixelatedRatio = (Vector2)ScreenSize / new Vector2(512);

        CursorState = CursorState.Grabbed;
        VSync = VSyncMode.On;
        GL.Enable(EnableCap.Blend);
        GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
        GL.Enable(EnableCap.DepthTest);
        GL.DepthFunc(DepthFunction.Lequal);
        GL.Viewport(0, 0, ScreenSize.X, ScreenSize.Y);

        float deltaTime = 1f / 60f;
        world = new EcsWorld();

        AABB aabb = new();
        aabb.LowerBound.Set(-10000);
        aabb.UpperBound.Set(10000);
        World b2World = new(aabb, new Vec2(0, 98), true);

        AudioManager audioManager = new();
        SharedData sharedData = new(new GameData(new Camera(), this, audioManager, new Vector2(0, 98f), new NetworkLogger()),
                                    new RenderData(new DebugDrawer(), new Graphics()),
                                    new PhysicsData(deltaTime, 1f / 8f, new PhysicsObjectsFactory(), b2World, 8, 3),
                                    new NetworkData("game"));

        this.sharedData = sharedData;

        sharedData.gameData.gameSystems = _gameSystems = new EcsSystems(world, sharedData);
        sharedData.renderData.renderSystems = _renderSystems = new EcsSystems(world, sharedData);
        sharedData.physicsData.physicsSystems = _physicsSystems = new EcsSystems(world, sharedData);
        sharedData.networkData.networkSystems = _networkSystems = new EcsSystems(world, sharedData);

        var gameDataFields = typeof(GameData).GetFields(BindingFlags.Public | BindingFlags.Instance);
        var renderDataFields = typeof(RenderData).GetFields(BindingFlags.Public | BindingFlags.Instance);
        var physicsDataFields = typeof(PhysicsData).GetFields(BindingFlags.Public | BindingFlags.Instance);
        var networkDataFields = typeof(NetworkData).GetFields(BindingFlags.Public | BindingFlags.Instance);

        PreInitServices(gameDataFields, renderDataFields, physicsDataFields, networkDataFields);

        LoadShaders(@"Content\Shaders");
        LoadShaders(@"Content\Shaders\Materials");
        LoadTextures(@"Content\Textures");
        LoadTextures(@"Content\Textures\Tilesets");
        LoadSounds(@"Content\Sounds");

        Mouse.Init(this);
        Keyboard.Init(this);

        _gameSystems
            //init and update stuff
            //levels and camera
            //.Add(new FileLoggerSystem())
            .Add(new InitLayersSystem())
            //  .Add(new GenBitmapFromPiSystem())
            .Add(new LevelsSystem())
            .Add(new CameraFollowCursorSystem())
            .Add(new CameraSystem())
            .Add(new DebugSystem())
            //update thing
            .Add(new PlayerSystem())
            // .Add(new DrawCollisionsSystem())
            .Add(new SetPostProcessingProjectionSystem())
            .Add(new RemoveDebugEntitiesSystem())
            .Add(new SetGroupsStateSystem())
            .DelHere<SetGroupSystemState>()
            .Inject()
            .Init();

        _renderSystems
            .Add(new RenderSpritesSystem())
            .Add(new RenderTextSystem())
           //  .Add(new LightingSystem())
            .Add(new PostProcessingSystem())
            .Inject()
            .Init();

        _physicsSystems
            .Add(new UpdatePhysicsSystem())
            .Inject()
            .Init();

        _networkSystems
            .AddGroup(typeof(PrintIPSystem).Name, false, null, this,
                new PrintIPSystem())
            .AddGroup("InitNetworkState", true, null, this,
                new InitNetworkState())
            .AddGroup("ConnectToServer", false, null, this,
                new ConnectToServerSystem())
            .Add(new UpdateNetworkSystem())
            .Inject()
            .Init();

        PostInitServices(gameDataFields, renderDataFields, physicsDataFields, networkDataFields);
    }

    public void AddGroupSystems(string name, IEcsSystem[] systems) =>
        sharedData.gameData.groupSystems[name] = systems;

    private void PreInitServices(FieldInfo[] gameDataFields, FieldInfo[] renderDataFields, FieldInfo[] physicsDataFields, FieldInfo[] networkDataFields)
    {
        var serviceType = typeof(Service);

        foreach (var field in gameDataFields)
        {
            if (!field.FieldType.IsAssignableTo(serviceType))
                continue;

            var instance = (Service)field.GetValue(sharedData.gameData);
            instance.PreInit(sharedData, world);
        }

        foreach (var field in renderDataFields)
        {
            if (!field.FieldType.IsAssignableTo(serviceType))
                continue;

            var instance = (Service)field.GetValue(sharedData.renderData);
            instance.PreInit(sharedData, world);
        }

        foreach (var field in physicsDataFields)
        {
            if (!field.FieldType.IsAssignableTo(serviceType))
                continue;

            var instance = (Service)field.GetValue(sharedData.physicsData);
            instance.PreInit(sharedData, world);
        }

        foreach (var field in networkDataFields)
        {
            if (!field.FieldType.IsAssignableTo(serviceType))
                continue;

            var instance = (Service)field.GetValue(sharedData.networkData);
            instance.PreInit(sharedData, world);
        }
    }

    //postInit exists to allow services to cache systems to call their methods (not too ecsy, but who cares ;v)
    private void PostInitServices(FieldInfo[] gameDataFields, FieldInfo[] renderDataFields, FieldInfo[] physicsDataFields, FieldInfo[] networkDataFields)
    {
        var serviceType = typeof(Service);

        foreach (var field in gameDataFields)
        {
            if (!field.FieldType.IsAssignableTo(serviceType))
                continue;

            var instance = (Service)field.GetValue(sharedData.gameData);
            instance.PostInit();
        }

        foreach (var field in renderDataFields)
        {
            if (!field.FieldType.IsAssignableTo(serviceType))
                continue;

            var instance = (Service)field.GetValue(sharedData.renderData);
            instance.PostInit();
        }

        foreach (var field in physicsDataFields)
        {
            if (!field.FieldType.IsAssignableTo(serviceType))
                continue;

            var instance = (Service)field.GetValue(sharedData.physicsData);
            instance.PostInit();
        }

        foreach (var field in networkDataFields)
        {
            if (!field.FieldType.IsAssignableTo(serviceType))
                continue;

            var instance = (Service)field.GetValue(sharedData.networkData);
            instance.PostInit();
        }
    }

    private void LoadTextures(string dirPath)
    {
        DirectoryInfo directoryInfo = new(dirPath);

        foreach (var file in directoryInfo.GetFiles())
            Content.LoadTexture(Path.GetFileNameWithoutExtension(file.FullName), file.FullName);
    }

    private void LoadSounds(string dirPath)
    {
        DirectoryInfo directoryInfo = new(dirPath);

        foreach (var file in directoryInfo.GetFiles())
            sharedData.gameData.audioManager.Add(file.FullName);

    }

    private void ReloadShaders(string dirPath)
    {
        DirectoryInfo directoryInfo = new(dirPath);
        var directories = directoryInfo.GetDirectories();

        foreach (var dir in directories)
        {
            var files = dir.GetFiles().ToList();

            var vertFile = files.Find(file => file.Name == "shader.vert");
            var fragFile = files.Find(file => file.Name == "shader.frag");
            var geomFile = files.Find(files => files.Name == "shader.geom");

            if (vertFile == null || fragFile == null)
                continue;

            var name = dir.Name;

            Content.ReloadShader(name, vertFile.FullName, fragFile.FullName, geomFile?.FullName);
        }

        GL.UseProgram(0);
    }

    private void LoadShaders(string dirPath)
    {
        DirectoryInfo directoryInfo = new(dirPath);

        foreach (var file in directoryInfo.GetFiles())
        {
            switch (file.Extension)
            {
                case ".frag":
                    Content.LoadShaderSource(SourceType.Frag, file.FullName);
                    break;
                case ".vert":
                    Content.LoadShaderSource(SourceType.Vert, file.FullName);
                    break;
                case ".geom":
                    Content.LoadShaderSource(SourceType.Geom, file.FullName);
                    break;
            }
        }

        var directories = directoryInfo.GetDirectories();
        foreach (var dir in directories)
        {
            var files = dir.GetFiles().ToList();

            var vertFile = files.Find(file => file.Extension == ".vert");
            var fragFile = files.Find(file => file.Extension == ".frag");
            var geomFile = files.Find(files => files.Extension == ".geom");

            if (vertFile == null || fragFile == null)
                continue;

            var name = dir.Name;

            Content.LoadShader(name, vertFile.FullName, fragFile.FullName, geomFile?.FullName);
        }

        GL.UseProgram(0);
    }
}