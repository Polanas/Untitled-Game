using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game;

class GameData
{

    public Vector2 inGameMousePosition;

    public EcsSystems gameSystems;

    public readonly SFX SFX;

    public readonly MyGameWindow game;

    public readonly List<Level> levelsList = new();

    public readonly Dictionary<string, Level> levels = new();

    public readonly Vector2 gravity;

    public readonly Camera camera;

    public readonly NetworkLogger logger;

    public readonly Dictionary<string, IEcsSystem[]> groupSystems;

    public GameData(Camera camera, MyGameWindow game, SFX sfx, Vector2 gravity, NetworkLogger logger)
    {
        this.camera = camera;
        SFX = sfx;
        this.game = game;
        this.gravity = gravity;
        this.logger = logger;

        groupSystems = new();
    }
}