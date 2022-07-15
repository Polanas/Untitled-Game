using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Windowing.Desktop;
using OpenTK.Graphics.OpenGL4;

namespace Game;

class Game
{
    public void Run()
    {
#if DEBUG
        SetWorkingPath();
#endif
        NativeWindowSettings nativeWindowSettings = new NativeWindowSettings()
        {
            Size = new Vector2i(1920, 1081),
            Title = "UntitledGame",
            StartFocused = true,
            WindowBorder = OpenTK.Windowing.Common.WindowBorder.Hidden,
        };

        GameWindowSettings gameWindowSettings = new();

        MyGameWindow gameWindow = null;

#if !DEBUG
        try
        {
#endif
        gameWindow = new MyGameWindow(gameWindowSettings, nativeWindowSettings);

#if !DEBUG
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            Console.ReadKey();
        }
#endif

#if !DEBUG
        try
        {
            using (gameWindow)
                gameWindow.Run();
        }
        catch (Exception e)
        {
            if (MyGameWindow.LogWriter == null)
            {
                Console.WriteLine(e);
                Console.ReadKey();
            }
            else MyGameWindow.LogWriter.WriteLine(e.ToString());
        }
#else
        using (gameWindow)
            gameWindow.Run();
#endif
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
}