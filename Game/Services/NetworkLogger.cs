using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game;

enum LogType
{
    Info,
    Warn,
    Error,
    Debug
}

enum SenderType
{
    Sever,
    Client
}

class NetworkLogger : Service
{

    private Dictionary<LogType, ConsoleColor> _logColors;

    private Dictionary<SenderType, ConsoleColor> _senderColors;


    public NetworkLogger()
    {
        _logColors = new();
        _logColors[LogType.Info] = ConsoleColor.Green;
        _logColors[LogType.Warn] = ConsoleColor.Yellow;
        _logColors[LogType.Error] = ConsoleColor.Red;
        _logColors[LogType.Debug] = ConsoleColor.Cyan;

        _senderColors = new();
        _senderColors[SenderType.Sever] = ConsoleColor.Blue;
        _senderColors[SenderType.Client] = ConsoleColor.DarkBlue;
    }

    public void Log(string message, LogType logType, SenderType senderType)
    {
        Console.ForegroundColor = _senderColors[senderType];
        Console.Write($"|{senderType}|");

        Console.ForegroundColor = _logColors[logType];
        Console.Write($"|{logType}|");

        Console.ForegroundColor = ConsoleColor.White;
        Console.WriteLine($" {message}");
    }
}