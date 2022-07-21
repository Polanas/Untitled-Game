using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game;

class CursorSystem : MySystem
{

    private Sprite _cursorSprite;

    public override void Run(EcsSystems systems)
    {
        _cursorSprite.position = Mouse.ScreenPosition - MyGameWindow.ScreenSize/16;
    }

    public override void Init(EcsSystems systems)
    {
        base.Init(systems);

        _cursorSprite = new Sprite("cursor");
        _cursorSprite.layer = Layer.UI;
        //_cursorSprite.scale = 1f/8f;

        int e = world.NewEntity();
        world.AddComponent(e, new Renderable(_cursorSprite));
        world.AddComponent(e, new Cursor());
    }
}
