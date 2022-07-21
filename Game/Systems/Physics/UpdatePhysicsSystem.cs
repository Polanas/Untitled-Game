using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Box2DX.Dynamics;
using Leopotam.EcsLite.Di;
using Box2DX.Common;

namespace Game;

class UpdatePhysicsSystem : MySystem
{

    private PhysicsData _physicsData;

    private EcsPoolInject<StaticBody> _staticBodies = default;

    private EcsPoolInject<DynamicBody> _dynamicBodies = default;

    private EcsPoolInject<Transform> _transforms = default;

    private EcsFilterInject<Inc<StaticBody>> _staticBodiesFilter = default;

    private EcsFilterInject<Inc<DynamicBody>> _dynamicBodiesFilter = default;

    public override void Run(EcsSystems systems)
    {
        Vec2 box2DPos;
        Vector2 pixelPos;

        _physicsData.b2World.Step(_physicsData.deltaTime, _physicsData.velocityIterations, _physicsData.positionIterations);

        foreach (var e in _staticBodiesFilter.Value)
        {
            ref var staticBody = ref _staticBodies.Value.Get(e);
            ref var transform = ref _transforms.Value.Get(e);

            box2DPos = staticBody.body.GetPosition();
            transform.position.X = box2DPos.X / _physicsData.PTM;
            transform.position.Y = box2DPos.Y / _physicsData.PTM;
        }

        foreach (var e in _dynamicBodiesFilter.Value)
        {
            ref var dynamicBody = ref _dynamicBodies.Value.Get(e);
            ref var transform = ref _transforms.Value.Get(e);

            box2DPos = dynamicBody.body.GetPosition();
            pixelPos.X = box2DPos.X / _physicsData.PTM;
            pixelPos.Y = box2DPos.Y / _physicsData.PTM;

            transform.position.X = box2DPos.X / _physicsData.PTM;
            transform.position.Y = box2DPos.Y / _physicsData.PTM;
        }
    }

    public override void Init(EcsSystems systems)
    {
        base.Init(systems);

        _physicsData = sharedData.physicsData;
    }
}
