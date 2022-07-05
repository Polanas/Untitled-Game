using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game;

class GameData
{

    public Vector2 inGameMousePosition;

    public readonly AudioManager audioManager;

    public readonly MyGameWindow game;

    public readonly List<Level> levels = new();

    public readonly Vector2 gravity;

    public readonly Camera camera;

    public readonly NetworkLogger logger;

    public readonly Dictionary<string, IEcsSystem[]> groupSystems;

    public GameData(Camera camera, MyGameWindow game, AudioManager manager, Vector2 gravity, NetworkLogger logger)
    {
        this.camera = camera;
        this.audioManager = manager;
        this.game = game;
        this.gravity = gravity;
        this.logger = logger;

        groupSystems = new();
    }
}