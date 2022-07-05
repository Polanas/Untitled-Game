using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Box2DX.Common;
using Box2DX.Collision;
using Box2DX.Dynamics;

namespace Game;

class PhysicsObjectsFactory : Service
{

    private EcsPool<Transform> _transforms;

    private EcsPool<StaticBody> _staticBodies;

    private EcsPool<DynamicBody> _dynamicBodies;

    private World _b2World;

    private float _PTM;

    public override void Init(SharedData sharedData, EcsWorld world)
    {
        base.Init(sharedData, world);

        _transforms = world.GetPool<Transform>();
        _staticBodies = world.GetPool<StaticBody>();
        _dynamicBodies = world.GetPool<DynamicBody>();

        _b2World = sharedData.physicsData.b2World;
        _PTM = sharedData.physicsData.PTM;
    }

    public int AddSaticBody(Transform transform)
    {
        int entity = world.AddEntity();

        ref var tr = ref _transforms.Add(entity);
        tr = transform;

        ref var stBody = ref _staticBodies.Add(entity);

        BodyDef bodyDef = new();
        Body staticBody = _b2World.CreateBody(bodyDef);

        PolygonDef polygonDef = new();
        polygonDef.SetAsBox(transform.size.X / 2 * _PTM, transform.size.Y / 2 * _PTM);
        staticBody.SetPosition(new Vec2(transform.position.X * _PTM, transform.position.Y * _PTM));
        staticBody.CreateFixture(polygonDef);

        stBody.body = staticBody;

        return entity;
    }

    public int AddDynamicBody(Transform transform)
    {
        int entity = world.AddEntity();

        ref var tr = ref _transforms.Add(entity);
        tr = transform;

        ref var dBody = ref _dynamicBodies.Add(entity);

        BodyDef bodyDef = new();
        bodyDef.FixedRotation = true;

        Body body = _b2World.CreateBody(bodyDef);

        PolygonDef box = new();
        box.Density = 1;
        box.Friction = 0f;
        box.SetAsBox(transform.size.X / 2 * _PTM, transform.size.Y / 2 * _PTM);
        body.SetPosition(new Vec2(transform.position.X * _PTM, transform.position.Y * _PTM));
        body.CreateFixture(box);
        body.SetMassFromShapes();

        dBody.body = body;

        return entity;
    }
}