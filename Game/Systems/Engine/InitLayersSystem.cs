
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game;

class InitLayersSystem : MySystem
{
    public override void Init(EcsSystems systems)
    {
        base.Init(systems);

        sharedData.renderData.drawUtils.AddLayer("default", new(MyGameWindow.ScreenSize, false, false, 0));
        sharedData.renderData.drawUtils.AddLayer("background1", new(new Vector2i(512), true, false, -1));
        sharedData.renderData.drawUtils.AddLayer("background2", new(new Vector2i(512), true, false, -2, 0.5f));
        sharedData.renderData.drawUtils.AddLayer("rectangle", new(MyGameWindow.ScreenSize, false, false, 1));
    }
}
