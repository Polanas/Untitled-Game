using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Leopotam.EcsLite.ExtendedSystems;
using Leopotam.EcsLite.Di;

namespace Game;

class InitNetworkState : MySystem
{

    private Task<string> _task;

    public override void Run(EcsSystems systems)
    {
        if (_task.IsCompleted)
        {
            string answer = _task.Result;
            sharedData.networkData.isServer = answer == "s";
            sharedData.networkData.isClient = !sharedData.networkData.isServer;

            world.AddComponent(world.AddEntity(), new GroupSystemState("InitNetworkState", false));

            if (!sharedData.networkData.isServer)
                world.AddComponent(world.AddEntity(), new GroupSystemState("ConnectToServer", true));
            else
            {
                sharedData.networkData.server = new(sharedData, 1337, "game");
                sharedData.networkData.isActive = true;
            }
        }
    }

    public override void Init(EcsSystems systems)
    {
        base.Init(systems);

        Console.Write("Are you a client or a server? (c/s)? ");
        _task = GetInputAsync();
    }

    private async Task<string> GetInputAsync() =>
         await Task.Run(() => Console.ReadLine());
}