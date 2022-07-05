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

    public override void Init(SharedData sharedData, EcsWorld world)
    {
        base.Init(sharedData, world);

        _drawablePool = world.GetPool<Drawable>();
        _rectanglePool = world.GetPool<DrawableRectangle>();
        _linePool = world.GetPool<DrawableLine>();

    }

    public void DrawRect(Vector2 position, Vector2 size, Vector3 color, float angle = 0)
    {
        var e = world.AddEntity();
        ref var rect = ref _rectanglePool.Add(e);
        rect = new(position, size, color, angle);
        _drawablePool.Add(e);
    }

    public void DrawRect(DrawableRectangle rect)
    {
        var e = world.AddEntity();
        ref var rect1 = ref _rectanglePool.Add(e);
        rect1 = rect;
        _drawablePool.Add(e);
    }

    public void DrawLine(Vector2 startPoint, Vector2 endPoint, Vector3 color, float thickness = 1)
    {
        var e = world.AddEntity();
        ref var line = ref _linePool.Add(e);
        line = new(startPoint, endPoint, color, thickness);
        _drawablePool.Add(e);
    }

    public void DrawLine(DrawableLine line)
    {
        var e = world.AddEntity();
        ref var line1 = ref _linePool.Add(e);
        line1 = line; 
        _drawablePool.Add(e);
    }

    public void DrawTriangle(Vector2 a, Vector2 b, Vector2 c, Vector3 color, float thickness = 1)
    {
        DrawLine(new DrawableLine(a, b, color, thickness));
        DrawLine(new DrawableLine(b, c, color, thickness));
        DrawLine(new DrawableLine(a, c, color, thickness));
    }
}