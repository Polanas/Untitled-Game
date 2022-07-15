using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Box2DX.Dynamics;

namespace Game;

class PlayerSystem : MySystem
{

    private Body _playerBody;

    private Sprite _playerSprite;

    public override void Run(EcsSystems systems)
    {
       
    }

    public override void Init(EcsSystems systems)
    {
        base.Init(systems);

        int e = sharedData.physicsData.physicsFactory.AddDynamicBody(new Transform(new Vector2(150, 0), 0, new Vector2(6, 14)));
        Sprite playerSprite = new Sprite("player", null, 0, sharedData.renderData.layers["default"]);
        playerSprite.offset = new Vector2(0, -9);

        world.AddComponent(e, new Tag("player"));
        world.AddComponent(e, new Renderable(playerSprite));

        _playerBody = world.GetComponent<DynamicBody>(e).body;
        _playerSprite = playerSprite;
    }
}