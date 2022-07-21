
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

        sharedData.renderData.graphics.AddLayer("text", new(MyGameWindow.ScreenSize, false, 3, 0));
        sharedData.renderData.graphics.AddLayer("UI", new(MyGameWindow.ScreenSize, false, 2, 0));
        sharedData.renderData.graphics.AddLayer("rectangle", new(MyGameWindow.ScreenSize, false, 1));
        sharedData.renderData.graphics.AddLayer("front", new(new Vector2i(512), true, 0));
        sharedData.renderData.graphics.AddLayer("default", new(MyGameWindow.ScreenSize, false, -1));
    //    sharedData.renderData.graphics.AddLayer("background2", new(new Vector2i(512), true, -2, 0.5f));

        Layer.InitLayers(
            sharedData.renderData.layers["default"],
            sharedData.renderData.layers["front"],
            sharedData.renderData.layers["UI"],
            sharedData.renderData.layers["text"]);
    }
}