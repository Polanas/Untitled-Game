using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Windowing.Desktop;
using OpenTK.Graphics.OpenGL;
using System.Runtime.InteropServices;

namespace Game;

class Game
{

    private FileStream _logFile;

    private const string USELESS_LOG = "will use VIDEO memory as the source for buffer object operations.";

    private static TextWriter _writer;

    private static DebugProc _debugProcCallback = DebugCallBack;

    private static GCHandle _debugProcCallbackHandle;

    public void Run()
    {
#if DEBUG
        SetWorkingPath();
#endif

        NativeWindowSettings nativeWindowSettings = new NativeWindowSettings()
        {
            Size = new Vector2i(1920, 1080),
            Title = "ECS Game",
        };
        var gameWindiow = new MyGameWindow(GameWindowSettings.Default, nativeWindowSettings);

        var currentDateTime = DateTime.Now.ToString().Replace(":", ".");

        _logFile = File.Create($@"Logs\{currentDateTime}.txt");
        _writer = new StreamWriter(_logFile);

        _debugProcCallbackHandle = GCHandle.Alloc(_debugProcCallback);

        GL.DebugMessageCallback(_debugProcCallback, IntPtr.Zero);
        GL.Enable(EnableCap.DebugOutput);
        GL.Enable(EnableCap.DebugOutputSynchronous);

#if !DEBUG
        try
        {
            using (gameWindiow)
                gameWindiow.Run();
        }
        catch (Exception e)
        {
            _writer.WriteLine(e.ToString());
        }
#else
        using (gameWindiow)
            gameWindiow.Run();
#endif

        _writer.Dispose();
        _logFile.Dispose();
    }

#if DEBUG
    private void SetWorkingPath()
    {
        StringBuilder sBuilder = new();
        string currentPath = Directory.GetCurrentDirectory();

        for (var i = 0; i < currentPath.Length; i++)
        {
            sBuilder.Append(currentPath[i]);
            var bString = sBuilder.ToString();

            if (bString.Contains(@"Untitled Game\Untitled Game\"))
            {
                Directory.SetCurrentDirectory(bString);
                return;
            }
        }
    }
#endif

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
}