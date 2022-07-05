using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game;

class DrawUtils
{
    private MyGameWindow _game;

    private RenderSpritesSystem _renderSpritesSystem;

    private Comparer<Layer> _layerComparer = Comparer<Layer>.Create((x, y) => x.depth.CompareTo(y.depth));

    public DrawUtils(MyGameWindow game)
    {
        _game = game;
    }

    public void Init(EcsSystems renderSystems)
    {
        IEcsSystem[] systems = null;
        renderSystems.GetAllSystems(ref systems);

        for (int i = 0; i < systems.Length; i++)
        {
            if (systems[i] is RenderSpritesSystem)
            {
                _renderSpritesSystem = (RenderSpritesSystem)systems[i];  //a bit cheasy but WHO CARES
                break;
            }
        }
    }

    public void AddLayer(string name, Layer layer)
    {
        _game.sharedData.renderData.layers[name] = layer;

        _game.sharedData.renderData.layersList.Add(layer);
        _game.sharedData.renderData.layersList.Sort(_layerComparer);

        _game.sharedData.renderData.cameraLayerProjections.Add(layer, default(Matrix4));

        _game.sharedData.renderData.layerProjections.Add(layer, default(Matrix4));
    }

    public void DrawSprite(Sprite sprite)
    {
        sprite.UpdateFrame();
        _renderSpritesSystem?.DrawSprite(sprite);
    }

    public void DrawTexture(Texture texture, Layer layer, Vector2 position, float depth = 0, float alpha = 1, float angle = 0)
    {
        _renderSpritesSystem?.DrawTexture(texture, layer, position, new Vector2(texture.Width, texture.Height), new Vector3(255), depth, alpha ,angle);
    }

    public void DrawTexture(Texture texture, Layer layer, Vector2 position, Vector2 size, Vector3 color, float depth = 0, float alpha = 1, float angle = 0)
    {
        _renderSpritesSystem?.DrawTexture(texture, layer, position, size, color, depth, alpha, angle);
    }
}