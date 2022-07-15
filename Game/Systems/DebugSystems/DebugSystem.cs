using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Box2DX.Common;
using Box2DX.Collision;
using Box2DX.Dynamics;
using Coroutines;
using SharpFont;

namespace Game;

unsafe class DebugSystem : MySystem
{

    private Sprite _sprite;

    private Body _box;

    private StateMachine _machine;

    private Sprite _gggame;

    private Sprite _test;

    private ColorMaterial _colorMaterial;

    private float mixAmount;

    public override void Run(EcsSystems systems)
    {
        _sprite.angle++;
        //  _sprite.position = sharedData.gameData.camera.position * _sprite.layer.cameraPosModifier;
        //}

        // sharedData.renderData.debugDrawer.DrawRect(Mouse.ScreenPosition / 8 - new Vector2(1920/16, 1080/16), new Vector2(20), new Vector3(125, 75, 200));

       //  _machine.Update();

            sharedData.networkData.client?.SendPosition(new Vector2(-100, 100));

        //if (Keyboard.Pressed(Keys.P))
        //    (_test.material as TestMaterial).isApplying = !(_test.material as TestMaterial).isApplying;

        //  (_fridge.material as TestMaterial).time += 1f / 60f;

        sharedData.renderData.graphics.DrawSprite(_test);

        mixAmount += .1f;

        _colorMaterial.mixAmount = MathF.Sin(mixAmount);

        sharedData.renderData.graphics.DrawText("Fuck you.", new Vector2(0), 1f, new Vector3i(255));
    }

    private int Walk()
    {
        if (Keyboard.Pressed(Keys.W))
            _box.SetLinearVelocity(new Vec2(_box.GetLinearVelocity().X, -300 * sharedData.physicsData.PTM));
        var velocity = _box.GetLinearVelocity();

        if (Keyboard.Down(Keys.A))
        {
            _box.SetLinearVelocity(new Vec2(-150 * sharedData.physicsData.PTM, velocity.Y));
            return 0;
        }
        else if (Keyboard.Down(Keys.D))
        {
            _box.SetLinearVelocity(new Vec2(150 * sharedData.physicsData.PTM, velocity.Y));

            return 0;
        }

        return 1;
    }

    private int StopBox()
    {
        _box.SetLinearVelocity(new Vec2(0, _box.GetLinearVelocity().Y));

        return 2;
    }

    private int Idle()
    {
        if (Keyboard.Pressed(Keys.W))
            _box.SetLinearVelocity(new Vec2(_box.GetLinearVelocity().X, -300 * sharedData.physicsData.PTM));

        if (Keyboard.Down(Keys.A) || Keyboard.Down(Keys.D))
            return 0;

        return 2;
    }

    public override void Init(EcsSystems systems)
    {
        base.Init(systems);

        int entity = world.AddEntity();
        ref var renderable = ref world.AddComponent<Renderable>(entity);

        renderable.sprite = new Sprite("test1", null, -1, sharedData.renderData.layers["background2"]);
        renderable.sprite.position = new Vector2(150, 70);
        _sprite = renderable.sprite;

        //  entity = world.AddEntity();
        //  renderable = ref world.AddComponent<Renderable>(entity);

        Sprite test = new Sprite("fridge", new Vector2i(32), 10);
        test.layer = sharedData.renderData.layers["UI"];
        test.AddAnimation("idle", 0.4f, true, 0, test.FramesAmount - 1);
        test.SetAnimation("idle");
        //
        _test = test;
        _test.material = _colorMaterial = new ColorMaterial(_test);
        _colorMaterial.color = new Vector3(255);
        _colorMaterial.mixAmount = .5f;

        //   renderable.sprite = _fridge;

        // _materialRenderer.Render(_fridge);

        // _fridge.SetTexture(_fridge.material.texture);

        //_fridge.material.texture.SaveRGBA(@"C:\Users\ivanh\Downloads\test.png");

        //  sharedData.gameData.camera.position -= new Vector2(200);

        //int e = sharedData.physicsData.physicsFactory.AddDynamicBody(new Transform(new Vector2(150, 0), 0, new Vector2(12,19)));

        //ref var body = ref world.GetComponent<DynamicBody>(e);
        //_box = body.body;

        //_machine = new(3);

        //_machine.SetCallBacks(0, Walk);
        //_machine.SetCallBacks(1, StopBox);
        //_machine.SetCallBacks(2, Idle);

        //_machine.ForceState(2);
    }
}