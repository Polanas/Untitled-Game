using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game;

class ConnectToServerSystem : MySystem
{

    Task<string> _task;

    public override void OnGroupActivate()
    {
        Console.Write("Enter IP to connect:");
        _task = GetInputAsync();
    }

    public override void Run(EcsSystems systems)
    {
        if (_task.IsCompleted)
        {
            var ip = _task.Result;

            sharedData.networkData.client = new(sharedData.gameData.game, 1337, ip, sharedData.networkData.gameName);
            sharedData.networkData.isActive = true;
            world.AddComponent(world.AddEntity(), new GroupSystemState("ConnectToServer", false));
        }
    }

    private async Task<string> GetInputAsync() =>
     await Task.Run(() => Console.ReadLine());
}