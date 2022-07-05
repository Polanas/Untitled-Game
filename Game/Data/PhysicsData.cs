using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Box2DX.Dynamics;

namespace Game;

class PhysicsData
{

    public readonly float deltaTime;

    public readonly float PTM;

    public readonly PhysicsObjectsFactory physicsFactory;

    public readonly World b2World;

    public readonly int velocityIterations;

    public readonly int positionIterations;

    public PhysicsData(float deltaTime, float PTM, PhysicsObjectsFactory physicsFactory, World b2World, int velocityIterations, int positionIterations)
    {
        this.deltaTime = deltaTime;
        this.PTM = PTM;
        this.physicsFactory = physicsFactory;
        this.b2World = b2World;
        this.velocityIterations = velocityIterations;
        this.positionIterations = positionIterations;
    }
}
