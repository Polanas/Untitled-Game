using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game;

static class Utils
{

    private static Camera _camera;

    public static Vector2 ToIngameSpace(Vector2 position)
    {
        return (Mouse.ScreenPosition - MyGameWindow.ScreenSize / 8 + (_camera.position + _camera.offset) * _camera.zoom) / _camera.zoom + MyGameWindow.ScreenSize / 16;
    }

    public static void Swap<T>(ref T a, ref T b)
    {
        T temp = a;
        a = b;
        b = temp;
    }

    public static void Init(Camera camera)
    {
        _camera = camera;
    }
}
