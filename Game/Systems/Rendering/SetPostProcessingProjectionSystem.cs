using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game;

class SetPostProcessingProjectionSystem : MySystem
{

    private Camera _camera;

    private Matrix4 _projection;

    public override void Init(EcsSystems systems)
    {
        base.Init(systems);

        _camera = sharedData.gameData.camera;
        _projection = Maths.CreateCameraMatrix(MyGameWindow.ScreenSize / 2, MyGameWindow.ScreenSize);
    }

    public override void Run(EcsSystems systems)
    {
        Layer layer;
        Matrix4 model = default(Matrix4);

        for (int i = 0; i < sharedData.renderData.layersList.Count; i++)
        {
            layer = sharedData.renderData.layersList[i];

            if (layer.pixelated)
            {
                Vector2 camPos = _camera.position * layer.cameraPosModifier;

                Vector2 fracCamPos = new Vector2
                {
                    X =  (camPos.X - MathF.Floor(camPos.X)),
                    Y =  ( camPos.Y - MathF.Floor(camPos.Y)),
                };

                model = Maths.CreateTransformMatrix(MyGameWindow.ScreenSize / 2 - (fracCamPos * 8), MyGameWindow.ScreenSize / MyGameWindow.FullToPixelatedRatio * 8);
            }
            else model = Maths.CreateTransformMatrix(MyGameWindow.ScreenSize / 2, MyGameWindow.ScreenSize);

            sharedData.renderData.layerProjections[layer] = model * _projection;
            //sharedData.renderData.pixelatedProjection = modelPixelated * _projection;
            // sharedData.renderData.nonPixelatedProjection = model * _projection;
        }
    }
}