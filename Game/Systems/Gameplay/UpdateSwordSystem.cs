using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Leopotam.EcsLite.Di;

namespace Game;

class UpdateSwordSystem : MySystem
{
    private EcsPoolInject<Player> _player;

    private EcsFilterInject<Inc<Player>> _playeraFilter;

    private EcsPoolInject<ArmsState> _armState;

    private EcsPoolInject<Transform> _transforms;

    private EcsPoolInject<Renderable> _renderables;

    private EcsPoolInject<Tag> _tags;

    public override void Run(EcsSystems systems)
    {
        foreach (var e in _playeraFilter.Value)
        {
            var player =  _player.Value.Get(e);

            if (!player.holdable.Unpack(world, out int holdableE))
                return;

            world.RepackEntity(ref player.holdable);

            var tag = _tags.Value.Get(holdableE);
            if (tag != "sword")
                return;

            var armState = _armState.Value.Get(holdableE);
            ref var transform = ref _transforms.Value.Get(holdableE);
            var renderable = _renderables.Value.Get(holdableE);
            var playerTransform = _transforms.Value.Get(e).position;

            renderable.sprite.flippedHorizontally = Mouse.InTheLeftSize;

            transform.position = playerTransform + armState.armEndOffsets[0].Rotate(-Maths.AngleBetweenPoints(playerTransform + new Vector2(0,-4), sharedData.gameData.inGameMousePosition), Vector2.Zero);

            if (renderable.sprite.flippedHorizontally)
                transform.position.X -= renderable.sprite.offset.X*2;
        }
    }

}
