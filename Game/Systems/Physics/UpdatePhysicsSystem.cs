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

    private EcsPoolInject<StaticBody> _staticBodies;

    private EcsPoolInject<DynamicBody> _dynamicBodies;

    private EcsPoolInject<Transform> _transforms;

    public override void Run(EcsSystems systems)
    {
        Vec2 vec2;

        foreach (var e in world.Filter<StaticBody>().End())
        {
            ref var staticBody = ref _staticBodies.Value.Get(e);
            ref var transform = ref _transforms.Value.Get(e);

            vec2.X = transform.position.X * _physicsData.PTM;
            vec2.Y = transform.position.Y * _physicsData.PTM;
            staticBody.body.SetPosition(vec2);
        }

        foreach (var e in world.Filter<DynamicBody>().End())
        {
            ref var dynamicBody = ref _dynamicBodies.Value.Get(e);
            ref var transform = ref _transforms.Value.Get(e);

            vec2.X = transform.position.X * _physicsData.PTM;
            vec2.Y = transform.position.Y * _physicsData.PTM;
            dynamicBody.body.SetPosition(vec2);
        }

        _physicsData.b2World.Step(_physicsData.deltaTime, _physicsData.velocityIterations, _physicsData.positionIterations);

        foreach (var e in world.Filter<StaticBody>().End())
        {
            ref var staticBody = ref _staticBodies.Value.Get(e);
            ref var transform = ref _transforms.Value.Get(e);

            vec2 = staticBody.body.GetPosition();
            transform.position.X = vec2.X / _physicsData.PTM;
            transform.position.Y = vec2.Y / _physicsData.PTM;
        }

        foreach (var e in world.Filter<DynamicBody>().End())
        {
            ref var staticBody = ref _dynamicBodies.Value.Get(e);
            ref var transform = ref _transforms.Value.Get(e);

            vec2 = staticBody.body.GetPosition();
            transform.position.X = vec2.X / _physicsData.PTM;
            transform.position.Y = vec2.Y / _physicsData.PTM;
        }
    }

    public override void Init(EcsSystems systems)
    {
        base.Init(systems);

        _physicsData = sharedData.physicsData;
    }
}
