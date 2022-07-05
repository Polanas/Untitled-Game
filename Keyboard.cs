using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game;

static class Keyboard
{
    public static KeyboardState State => _game.KeyboardState;

    private static MyGameWindow _game;

    public static void Init(MyGameWindow game) => _game = game;

    public static bool Pressed(Keys key) => State.IsKeyPressed(key);

    public static bool Down(Keys key) => State.IsKeyDown(key);
}

