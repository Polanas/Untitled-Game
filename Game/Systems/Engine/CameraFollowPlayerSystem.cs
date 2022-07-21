using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Leopotam.EcsLite.Di;

namespace Game;

class CameraFollowPlayerSystem : MySystem
{

    private EcsFilterInject<Inc<Player>> _playersFilter;

    private EcsPoolInject<Player> _players;

    private EcsPoolInject<DynamicBody> _dynamicBodies;

    private EcsPoolInject<Transform> _transforms;

    private Camera _camera;

    public override void Run(EcsSystems systems)
    {
        foreach (var e in _playersFilter.Value)
        {
            var body = _dynamicBodies.Value.Get(e).body;
            Vector2 followPos = body.GetPixelatedPosition();

            followPos += (sharedData.gameData.inGameMousePosition - followPos) / 16f;
            float additionalAmount = body.GetLinearVelocity().Y;

            _camera.position = _camera.position.LerpDist(followPos, Vector2.Distance(_camera.position, followPos) / (5 - (additionalAmount / 80f)));

            //_camera.position = followPos;

            //if (_followCursor)
            //    followingPos = _attachedThing.position + (Mouse.InGamePosition - _attachedThing.position) / 16;
            //else followingPos = _attachedThing.position;

            //float additionalAmount = 0;
            //if (_attachedThing as PhysicsObject is PhysicsObject attachedPhysicsObj && attachedPhysicsObj != null)
            //    additionalAmount = attachedPhysicsObj.speed.Y;

            //position = LerpStuff.Vector2(position, followingPos, Vector2.Distance(position, followingPos) / (10 - (additionalAmount / 100)));
        }
    }

    public override void Init(EcsSystems systems)
    {
        base.Init(systems);

        _camera = sharedData.gameData.camera;
    }
}