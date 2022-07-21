using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Box2DX.Dynamics;
using Box2DX.Common;
using Leopotam.EcsLite.Di;

namespace Game;

enum PlayerState
{
    Idle,
    Stop,
    Walking,
    Jumping,
}

class PlayerSystem : MySystem
{

    private Body _playerBody;

    private Sprite _playerSprite;

    private StateMachine<PlayerState> _stateMachine;

    private EcsFilterInject<Inc<Player>> _playerFilter;

    private bool _mouseWasOnLeftSide;

    private PlayerState Idle()
    {
        if (Keyboard.Pressed(Keys.W))
        {
            _playerSprite.SetAnimation("jump");
            return PlayerState.Jumping;
        }
        else if (Keyboard.Down(Keys.A) == Keyboard.Down(Keys.D))
        {
            return PlayerState.Idle;
        }
        else
        {
            _playerSprite.SetAnimation("walk");
            return PlayerState.Walking;
        }
    }

    private PlayerState Walking()
    {
        Vec2 velocity = _playerBody.GetLinearVelocity();

        if (Keyboard.Pressed(Keys.W))
        {
            _playerSprite.SetAnimation("jump");
            return PlayerState.Jumping;
        }

        if (Keyboard.Down(Keys.A) && Keyboard.Down(Keys.D))
            return PlayerState.Stop;

        if (Keyboard.Down(Keys.A))
        {
            _playerBody.SetLinearVelocity(new Vec2(-147 * sharedData.physicsData.PTM, velocity.Y));
            return PlayerState.Walking;
        }
        if (Keyboard.Down(Keys.D))
        {
            _playerBody.SetLinearVelocity(new Vec2(147 * sharedData.physicsData.PTM, velocity.Y));
            return PlayerState.Walking;
        }

        return PlayerState.Stop;
    }

    private PlayerState Jumping()
    {
        Vec2 velocity = _playerBody.GetLinearVelocity();

        if (Keyboard.Pressed(Keys.W))
        {
            sharedData.gameData.SFX.Play("explotion", .5f);
            _playerBody.SetLinearVelocity(new Vec2(velocity.X, -300 * sharedData.physicsData.PTM));
            return PlayerState.Jumping;
        }

        if (velocity.Y == 0)
        {
            if (velocity.X == 0)
                return PlayerState.Stop;

            _playerSprite.SetAnimation("walk");
            return PlayerState.Walking;
        }

        if (Keyboard.Down(Keys.A) == Keyboard.Down(Keys.D))
        {
            _playerBody.SetLinearVelocity(new Vec2(0, _playerBody.GetLinearVelocity().Y));
            return PlayerState.Jumping;
        }

        if (Keyboard.Down(Keys.A))
        {
            _playerBody.SetLinearVelocity(new Vec2(-147 * sharedData.physicsData.PTM, velocity.Y));
            return PlayerState.Jumping;
        }
        if (Keyboard.Down(Keys.D))
        {
            _playerBody.SetLinearVelocity(new Vec2(147 * sharedData.physicsData.PTM, velocity.Y));
            return PlayerState.Jumping;
        }


        return PlayerState.Jumping;
    }

    private PlayerState Stop()
    {
        _playerBody.SetLinearVelocity(new Vec2(0, _playerBody.GetLinearVelocity().Y));
        _playerSprite.SetAnimation("idle");

        return PlayerState.Idle;
    }

    public override void Run(EcsSystems systems)
    {
        _stateMachine.Update();

        _playerSprite.flippedHorizontally = Mouse.InTheLeftSize;

        if (_stateMachine.State == PlayerState.Walking)
        {
            if ((Mouse.InTheLeftSize && _playerBody.GetLinearVelocity().X > 0) || (Mouse.InTheRightSide && _playerBody.GetLinearVelocity().X < 0))
                _playerSprite.SetAnimation("moonwalk", true);
            else _playerSprite.SetAnimation("walk", true);
        }

       // if (Mouse.Down(MouseButton.Button1))
       //     _playerBody.SetPosition(sharedData.gameData.inGameMousePosition);

        _mouseWasOnLeftSide = Mouse.InTheLeftSize;
    }

    public override void Init(EcsSystems systems)
    {
        base.Init(systems);

        int e = sharedData.physicsData.physicsFactory.AddDynamicBody(new Transform(new Vector2(150, 0), 0, new Vector2(6-1f/4f, 14)));
        Sprite playerSprite = new Sprite("player", new Vector2i(32,32), 5, sharedData.renderData.layers["default"]);
        playerSprite.offset = new Vector2(0, -9);
        playerSprite
            .AddAnimation("idle", 1, false, new int[1])
            .AddAnimation("jump", 1, false, new int[] { 1 })
            .AddAnimation("goDown", 1, false, new[] { 2 })
            .AddAnimation("walk", 0.4f, true, 3, 10)
            .AddAnimation("moonwalk", 0.4f, true, 11, playerSprite.FramesAmount - 1)
            .SetAnimation("idle");

        world.AddComponent(e, new Tag("player"));
        world.AddComponent(e, new Renderable(playerSprite));
        world.AddComponent(e, new Player());

        _playerBody = world.GetComponent<DynamicBody>(e).body;
        _playerSprite = playerSprite;

        _stateMachine = new(Enum.GetNames(typeof(PlayerState)).Length);

        _stateMachine.SetCallBacks(PlayerState.Idle, Idle);
        _stateMachine.SetCallBacks(PlayerState.Walking, Walking);
        _stateMachine.SetCallBacks(PlayerState.Stop, Stop);
        _stateMachine.SetCallBacks(PlayerState.Jumping, Jumping);
    }
}