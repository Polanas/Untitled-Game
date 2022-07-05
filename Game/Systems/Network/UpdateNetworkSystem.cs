using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game;

class UpdateNetworkSystem : MySystem
{
    public override void Run(EcsSystems systems)
    {
        if (!sharedData.networkData.isActive)
            return;

        if (sharedData.networkData.isServer)
            sharedData.networkData.server.Update();
        else if (sharedData.networkData.isClient)
            sharedData.networkData.client.Update();
    }
}