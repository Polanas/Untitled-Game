#region stuff

using System;
using System.Text;
using System.Linq;
using System.Collections.Generic;
using OpenTK.Graphics.OpenGL;
using Leopotam.EcsLite.Di;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using Box2DX.Dynamics;
using Box2DX.Collision;
using Box2DX.Common;
using System.Reflection;
using Leopotam.EcsLite.ExtendedSystems;
using System.ComponentModel;

namespace Game;

class MyGameWindow : GameWindow
{

    public static Vector2i ScreenSize { get; private set; }

    public static Vector2 FullToPixelatedRatio { get; private set; }

    public EcsWorld world;

    public SharedData sharedData;

    private EcsSystems _gameSystems;

    private EcsSystems _renderSystems;

    private EcsSystems _physicsSystems;

    private EcsSystems _networkSystems;

    private double _summedRenderTime;

    private double _lastElapsedTime;

    private int _framesCount;

    public MyGameWindow(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings) :
     base(gameWindowSettings, nativeWindowSettings)
    { }

    protected override void OnLoad()
    {
        base.OnLoad();

        GL.ClearColor(System.Drawing.Color.FromArgb(0, 0, 0, 0));

        Init();
    }

    protected override void Dispose(bool disposing)
    {
        if (_renderSystems != null)
        {
            _renderSystems.Destroy();
            _physicsSystems.Destroy();
            _networkSystems.Destroy();
            _gameSystems.Destroy();
        }


        base.Dispose(disposing);
    }

    protected override void OnUpdateFrame(FrameEventArgs args)
    {
        base.OnUpdateFrame(args);

#if RELEASE
        if (!IsFocused)
            return;
#endif


        _networkSystems.Run();
        _gameSystems.Run();
        _physicsSystems.Run();

        if (Keyboard.Pressed(Keys.R))
            ReloadShaders(@"OpenGL\Shaders");

        if (Keyboard.Pressed(Keys.Escape))
            Close();

        SwapBuffers();
    }

    protected override void OnRenderFrame(FrameEventArgs args)
    {
        base.OnRenderFrame(args);

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


#if DEBUG
        double currentTime = GLFW.GetTime();
        double elapsed = currentTime - _lastElapsedTime;

        _summedRenderTime += elapsed;

        if (elapsed >= 1.0)
        {
            double averageTime = _summedRenderTime / _framesCount;
            Title = $"Average rendering time:  {averageTime} ms";
            _lastElapsedTime += elapsed;
        }
#endif
    }

    #endregion

    private void Init()
    {

#if !DEBUG
        WindowState = WindowState.Fullscreen;
#endif

        ScreenSize = new Vector2i(1920, 1080);
        FullToPixelatedRatio = (Vector2)ScreenSize / new Vector2(512);

        CursorVisible = false;
        CursorGrabbed = true;
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
                                    new RenderData(Matrix4.CreateOrthographicOffCenter(0, ScreenSize.X, ScreenSize.Y, 0, -1, 1), new DebugDrawer(), new DrawUtils(this)),
                                    new PhysicsData(deltaTime, 1f / 8f, new PhysicsObjectsFactory(), b2World, 8, 3),
                                    new NetworkData("game"));

        this.sharedData = sharedData;

        InitServices();

        LoadShaders(@"Content\Shaders");
        LoadShaders(@"Content\Shaders\MaterialShaders");
        LoadTextures(@"Content\Textures");
        LoadTextures(@"Content\Textures\Tilesets");
        LoadSounds(@"Content\Sounds");

        Mouse.Init(this);
        Keyboard.Init(this);

        _gameSystems = new EcsSystems(world, sharedData);
        _gameSystems
            //init and update stuff
            //levels and camera
            //.Add(new FileLoggerSystem())
            .Add(new InitLayersSystem())
            //  .Add(new GenBitmapFromPiSystem())
            .Add(new LevelsSystem())
            .Add(new CameraFollowCursorSystem())
            .Add(new CameraSystem())
            //update things
            .Add(new DebugSystem())
            //.Add(new DrawCollisionsSystem())
            .Add(new SetPostProcessingProjectionSystem())
            .Add(new RemoveDebugEntitiesSystem())
            .Add(new SetGroupsStateSystem())
            .DelHere<GroupSystemState>()
            .Inject()
            .Init();

        _renderSystems = new EcsSystems(world, sharedData);
        _renderSystems
            .Add(new RenderSpritesSystem())
            //  .Add(new LightingSystem())
            .Add(new PostProcessingSystem())
            .Inject()
            .Init();

        _physicsSystems = new EcsSystems(world, sharedData);
        _physicsSystems
            .Add(new UpdatePhysicsSystem())
            .Inject()
            .Init();

        _networkSystems = new EcsSystems(world, sharedData);
        _networkSystems
            .Add(new PrintIPSystem())
            .AddGroup("InitNetworkState", true, null, this,
                new InitNetworkState())
            .AddGroup("ConnectToServer", false, null, this,
                new ConnectToServerSystem())
            .Add(new UpdateNetworkSystem())
            .Inject()
            .Init();

        sharedData.renderData.drawUtils.Init(_renderSystems);
    }

    public void AddGroupSystems(string name, IEcsSystem[] systems) =>
        sharedData.gameData.groupSystems[name] = systems;

    private void InitServices()
    {
        var gameDataFields = typeof(GameData).GetFields(BindingFlags.Public | BindingFlags.Instance);
        var renderDataFields = typeof(RenderData).GetFields(BindingFlags.Public | BindingFlags.Instance);
        var physicsDataFields = typeof(PhysicsData).GetFields(BindingFlags.Public | BindingFlags.Instance);

        var serviceType = typeof(Service);

        foreach (var field in gameDataFields)
        {
            if (!field.FieldType.IsAssignableTo(serviceType))
                continue;

            var instance = (Service)field.GetValue(sharedData.gameData);
            instance.Init(sharedData, world);
        }

        foreach (var field in renderDataFields)
        {
            if (!field.FieldType.IsAssignableTo(serviceType))
                continue;

            var instance = (Service)field.GetValue(sharedData.renderData);
            instance.Init(sharedData, world);
        }

        foreach (var field in physicsDataFields)
        {
            if (!field.FieldType.IsAssignableTo(serviceType))
                continue;

            var instance = (Service)field.GetValue(sharedData.physicsData);
            instance.Init(sharedData, world);
        }
    }

    private void LoadTextures(string dirPath)
    {
        DirectoryInfo directoryInfo = new(dirPath);

        foreach (var file in directoryInfo.GetFiles())
            ResourceManager.LoadTexture(Path.GetFileNameWithoutExtension(file.FullName), file.FullName);
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

            ResourceManager.ReloadShader(name, vertFile.FullName, fragFile.FullName, geomFile?.FullName);
        }
    }

    private void LoadShaders(string dirPath)
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

            ResourceManager.LoadShader(name, vertFile.FullName, fragFile.FullName, geomFile?.FullName);
        }
    }
}