using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Leopotam.EcsLite.Di;

namespace Game;

class SharedData
{
 
    public readonly GameData gameData;

    public readonly RenderData renderData;

    public readonly PhysicsData physicsData;

    public readonly NetworkData networkData;

    public SharedData(GameData gameData, RenderData renderData, PhysicsData physicsData, NetworkData networkData)
    {
        this.gameData = gameData;
        this.renderData = renderData;
        this.physicsData = physicsData;
        this.networkData = networkData;
    }
}
