using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL;
using System.Runtime.InteropServices;

namespace Game;

class FileLoggerSystem : MySystem
{

    private FileStream _logFile;

    private const string USELESS_LOG = "will use VIDEO memory as the source for buffer object operations.";

    private static TextWriter _writer;

    private static DebugProc _debugProcCallback = DebugCallBack;

    private static GCHandle _debugProcCallbackHandle;

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

    public override void Init(EcsSystems systems)
    {
        base.Init(systems);

        var currentnDateTime = DateTime.Now.ToString().Replace(":", ".");

        _logFile = File.Create($@"Logs\{currentnDateTime}.txt");
        _writer = new StreamWriter(_logFile);

        _debugProcCallbackHandle = GCHandle.Alloc(_debugProcCallback);

        GL.DebugMessageCallback(_debugProcCallback, IntPtr.Zero);
        GL.Enable(EnableCap.DebugOutput);
        GL.Enable(EnableCap.DebugOutputSynchronous);
    }

    public override void Destroy(EcsSystems systems)
    {
        _writer.Dispose();
        _logFile.Dispose();
    }
}