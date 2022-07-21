using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game;

class InitSwordSystem : MySystem
{
    public override void Init(EcsSystems systems)
    {
        base.Init(systems);

        int player = world.GetEntitiyWithComponent<Player>();
        int sword = world.AddEntity();

        Sprite swordSprite = new Sprite("sword");
        swordSprite.depth = 6.05f;
        swordSprite.offset = new Vector2(4, -8);

        world.AddComponent(sword, new Renderable(swordSprite));
        world.AddComponent(sword, new Holdable());
        world.AddComponent(sword, new Transform());
        world.AddComponent(sword, new Tag("sword"));
        ref var armsState = ref world.AddComponent<ArmsState>(sword);

        armsState.armsMode = ArmsMode.Connected;
        armsState.armEndOffsets[0] = armsState.armEndOffsets[1] = new Vector2(10, 0);

        ref var playerC = ref world.GetComponent<Player>(player);
        playerC.holdable = world.PackEntity(sword);
    }
}
