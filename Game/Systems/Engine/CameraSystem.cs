using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game;

class CameraSystem : MySystem
{

    private SharedData _sharedData;

    private Camera _camera;

    private float angle;

    public override void Init(EcsSystems systems)
    {
        base.Init(systems);

        _sharedData = systems.GetShared<SharedData>();
        _camera = _sharedData.gameData.camera;

        _camera.zoom = 1f;
        UpdateProjections();
    }

    public void UpdateProjections()
    {
        Layer layer;

        if (Keyboard.Down(Keys.K))
        {
            angle += 0.1f;

            _camera.position.X = 0 + 10 * MathF.Cos(angle);
            _camera.position.Y = 0 + 10 * MathF.Sin(angle);
        }

        for (int i = 0; i < sharedData.renderData.layersList.Count; i++)
        {
            layer = sharedData.renderData.layersList[i];

            if (layer.pixelated)
                sharedData.renderData.cameraLayerProjections[layer] = Maths.CreateCameraMatrix(new Vector2(MathF.Floor(_camera.position.X * layer.cameraPosModifier),
                                                                                                           MathF.Floor(_camera.position.Y * layer.cameraPosModifier)),
                                                                                               (Vector2)MyGameWindow.ScreenSize / MyGameWindow.FullToPixelatedRatio / _camera.zoom);
            else sharedData.renderData.cameraLayerProjections[layer] = Maths.CreateCameraMatrix(_camera.position * layer.cameraPosModifier, (Vector2)MyGameWindow.ScreenSize / _camera.zoom / 8);
        }

    }

    public override void Run(EcsSystems systems)
    {
        UpdateProjections();
    }
}