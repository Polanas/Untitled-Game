using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game;

class DebugDrawer : Service
{
    private EcsPool<Drawable> _drawablePool;

    private EcsPool<DrawableRectangle> _rectanglePool;

    private EcsPool<DrawableLine> _linePool;

    private RenderSpritesSystem _renderSpritesSystem;

    public override void PreInit(SharedData sharedData, EcsWorld world)
    {
        base.PreInit(sharedData, world);

        _drawablePool = world.GetPool<Drawable>();
        _rectanglePool = world.GetPool<DrawableRectangle>();
        _linePool = world.GetPool<DrawableLine>();

    }

    public override void PostInit()
    {
        _renderSpritesSystem = sharedData.renderData.renderSystems.Find<RenderSpritesSystem>();
    }

    public void DrawRect(Vector2 position, Vector2 size, Vector3 color, float angle = 0) =>
        _renderSpritesSystem.DrawRect(position, color, size, angle);

    public void DrawLine(Vector2 startPoint, Vector2 endPoint, Vector3 color, float thickness = 1) =>
        _renderSpritesSystem.DrawLine(startPoint, endPoint, color, thickness);

    public void DrawTriangle(Vector2 a, Vector2 b, Vector2 c, Vector3 color, float thickness = 1)
    {
        DrawLine(a, b, color, thickness);
        DrawLine(b, c, color, thickness);
        DrawLine(a, c, color, thickness);
    }
}