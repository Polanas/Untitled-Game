using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game;

class Graphics : Service
{
    private RenderSpritesSystem _renderSpritesSystem;

    private RenderTextSystem _renderTextSystem;

    private Comparer<Layer> _layerComparer = Comparer<Layer>.Create((x, y) => x.depth.CompareTo(y.depth));

    public override void PostInit()
    {
        _renderSpritesSystem = sharedData.renderData.renderSystems.Find<RenderSpritesSystem>();
        _renderTextSystem = sharedData.renderData.renderSystems.Find<RenderTextSystem>();
    }

    public void AddLayer(string name, Layer layer)
    {
        sharedData.renderData.layers[name] = layer;

        sharedData.renderData.layersList.Add(layer);
        sharedData.renderData.layersList.Sort(_layerComparer);

        sharedData.renderData.cameraLayerProjections.Add(layer, default(Matrix4));
        sharedData.renderData.layerProjections.Add(layer, default(Matrix4));
    }

    public void DrawSprite(Sprite sprite)
    {
        sprite.UpdateFrame();
        _renderSpritesSystem?.DrawSprite(sprite);
    }

    public void DrawText(string text, Vector2 position, float scale, Vector3i color, bool centered = true, float alpha = 1) =>
        _renderTextSystem?.DrawText(text, position, scale, color, centered, alpha);

    public void DrawTexture(Texture texture, Layer layer, Vector2 position, float depth = 0, float alpha = 1, float angle = 0)
    {
        _renderSpritesSystem?.DrawTexture(texture, layer, position, new Vector2(texture.Width, texture.Height), new Vector3(255), depth, alpha, angle);
    }

    public void DrawTexture(Texture texture, Layer layer, Vector2 position, Vector2 size, Vector3 color, float depth = 0, float alpha = 1, float angle = 0)
    {
        _renderSpritesSystem?.DrawTexture(texture, layer, position, size, color, depth, alpha, angle);
    }
}