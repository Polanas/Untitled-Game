using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Leopotam.EcsLite.Di;
using Leopotam.EcsLite.ExtendedSystems;

namespace Game;

class RemoveDebugEntitiesSystem : MySystem
{

    private EcsFilter _lineFilter;

    private EcsFilter _rectangleFilter;

    private EcsPool<Drawable> _drawablePool = default;

    public override void Init(EcsSystems systems)
    {
        base.Init(systems);

        _lineFilter = world.Filter<DrawableLine>().Inc<Drawable>().End();
        _rectangleFilter = world.Filter<DrawableRectangle>().Inc<Drawable>().End();
        _drawablePool = world.GetPool<Drawable>();
    }

    public override void Run(EcsSystems systems)
    {
        foreach (var entity in _lineFilter)
        {
            var drawable = _drawablePool.Get(entity);

            if (drawable.wereDrawn)
                world.DelEntity(entity);
        }

        foreach (var entity in _rectangleFilter)
        {
            var drawable = _drawablePool.Get(entity);

            if (drawable.wereDrawn)
                world.DelEntity(entity);
        }
    }
}
