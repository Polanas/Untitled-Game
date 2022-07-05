using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game;

class CameraFollowCursorSystem : MySystem
{

    private Camera _camera;

    public override void Init(EcsSystems systems)
    {
        base.Init(systems);

        _camera = sharedData.gameData.camera;
    }

    public override void Run(EcsSystems systems)
    {
        _camera.position = Mouse.ScreenPosition / 8;
    }
}
