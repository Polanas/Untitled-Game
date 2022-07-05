using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Leopotam.EcsLite.Di;
using Box2DX.Common;

namespace Game;

class DrawCollisionsSystem : MySystem
{

    private EcsPoolInject<Transform> _transforms;

    private EcsPoolInject<StaticBody> _staticBodies;

    private EcsPoolInject<DynamicBody> _dynamicBodies;

    private EcsPoolInject<DebugCollisionDrawingState> _drawingState;

    private DebugDrawer _drawer;

    private PhysicsData _physicsData;

    public override void Init(EcsSystems systems)
    {
        base.Init(systems);

         _drawer = sharedData.renderData.debugDrawer;
         _physicsData = sharedData.physicsData;

        world.AddComponent<DebugCollisionDrawingState>(world.NewEntity());
    }

    public override void Run(EcsSystems systems)
    {
        foreach(var e in world.Filter<DebugCollisionDrawingState>().End())
        {
            ref var state = ref _drawingState.Value.Get(e);

            if (!state.draw)
                return;

            break;
        }

        Vec2 vec2;
        Vector2 pos;

        foreach (var e in world.Filter<StaticBody>().End())
        {
            ref var staticBody = ref _staticBodies.Value.Get(e);
            ref var transform = ref _transforms.Value.Get(e);

            vec2 = staticBody.body.GetPosition();
            pos.X = vec2.X / _physicsData.PTM;
            pos.Y = vec2.Y / _physicsData.PTM;

           _drawer.DrawRect(pos, transform.size, Vector3.One * 255);
        }

        foreach (var e in world.Filter<DynamicBody>().End())
        {
            ref var staticBody = ref _dynamicBodies.Value.Get(e);
            ref var transform = ref _transforms.Value.Get(e);

            vec2 = staticBody.body.GetPosition();
            pos.X = vec2.X / _physicsData.PTM;
            pos.Y = vec2.Y / _physicsData.PTM;

            _drawer.DrawRect(pos, transform.size, new Vector3(0,255,0));
        }
    }
}