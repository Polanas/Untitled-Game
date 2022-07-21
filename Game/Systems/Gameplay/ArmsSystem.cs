using Leopotam.EcsLite.Di;

namespace Game;

public enum ArmsMode
{
    Regular,
    Connected
}

class ArmsSystem : MySystem
{

    private Sprite[] _arms = new Sprite[2];

    private float[] _angles = new float[2];

    private float[] _armLengths = new float[2];

    private Vector2[] _armOffsets = new Vector2[2];

    private float[] _armDepths = new float[2];

    private ArmMaterial[] _armMaterials = new ArmMaterial[2];

    private Vector2[] _armStartPositions = new Vector2[2];

    private EcsFilterInject<Inc<Player>> _playerFilter;

    private EcsFilterInject<Inc<ArmsState>> _armStateFilter;

    private EcsPoolInject<ArmsState> _armState;

    private EcsPoolInject<Transform> _transforms;

    private EcsPoolInject<Player> _player;

    private bool _mouseWasInTheLeftSide;

    public override void Run(EcsSystems systems)
    {
        foreach (var e in _playerFilter.Value)
        {
            ArmsState? armState = null;

            var player = _player.Value.Get(e);

            if (player.holdable.Unpack(world, out int holdable))
            {
                armState = _armState.Value.Get(holdable);
                player.holdable = world.RepackEntity(ref player.holdable);
            }
            
            if (armState == null)
                foreach (var e1 in _armStateFilter.Value)
                    armState = _armState.Value.Get(e1);

            if (_mouseWasInTheLeftSide != Mouse.InTheLeftSize)
                Utils.Swap(ref _armDepths[0], ref _armDepths[1]);

            ref var playerTransform = ref _transforms.Value.Get(e);

            for (int i = 0; i < _arms.Length; i++)
            {
                _arms[i].depth = _armDepths[i];

                _arms[i].position = playerTransform.position;
                _arms[i].position.Y += 1f / 8f;

                _angles[i] = -Maths.AngleBetweenPoints(_arms[i].position + new Vector2(0, _armOffsets[i].Y), sharedData.gameData.inGameMousePosition);

                _armMaterials[i].startPosition = _armStartPositions[i] + GetArmOffset(_angles[i]);
                _armMaterials[i].endPosition = _armStartPositions[i] + armState.Value.armEndOffsets[i].Rotate(_angles[i], Vector2.Zero);

                if (armState.Value.armsMode == ArmsMode.Connected)
                {
                    float cos = MathF.Cos(MathHelper.DegreesToRadians(_angles[i]));
                    _armMaterials[i].endPosition.X = _armStartPositions[i].X + armState.Value.armEndOffsets[0].X * cos - _armOffsets[i].X;
                }
            }

            _mouseWasInTheLeftSide = Mouse.InTheLeftSize;
        }
    }

    public override void Init(EcsSystems systems)
    {
        base.Init(systems);

        for (int i = 0; i < _arms.Length; i++)
        {
            _arms[i] = new Sprite(Texture.LoadEmpty(new Vector2i(100)));
            _arms[i].material = _armMaterials[i] = new ArmMaterial(_arms[i]);
            world.AddComponent(world.AddEntity(), new Renderable(_arms[i]));
        }

        _armDepths[0] = 6.1f;
        _armDepths[1] = 6f;

        _armOffsets[0] = new Vector2(-2, -4);
        _armOffsets[1] = new Vector2(3, -4);

        _armLengths[0] = 10;
        _armLengths[1] = 10;

        _armStartPositions[0] = new Vector2(50 - 2, 50 - 4);
        _armStartPositions[1] = new Vector2(50 + 3, 50 - 4);

        ref var armState = ref world.AddComponent<ArmsState>(world.NewEntity());
        armState.armsMode = ArmsMode.Connected;
        armState.armEndOffsets[0] = new Vector2(10, 0);
        armState.armEndOffsets[1] = new Vector2(10, 0);
    }

    private Vector2 GetArmOffset(float angle)
    {
        float angleMod = angle % 360;
        //  angleMod += 180;

        if (angleMod <= -90 && angleMod >= -180)
            return new Vector2(0, 1);
        if (angleMod <= 0 && angleMod >= -90)
            return new Vector2(-1, 1);
        if (angleMod >= 0 && angleMod <= 90)
            return new Vector2(-1, 0);

        return Vector2.Zero;
    }
}
