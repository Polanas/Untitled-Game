using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Leopotam.EcsLite.Di;
using Leopotam.EcsLite.ExtendedSystems;

namespace Game;

class SetGroupsStateSystem : MySystem
{

    private EcsPoolInject<GroupSystemState> _groupSystemStates;

    private EcsPoolInject<EcsGroupSystemState> _ecsGroupSystemStates;

    public override void Run(EcsSystems systems)
    {
        foreach(var e in world.Filter<GroupSystemState>().End())
        {
            ref var gss = ref _groupSystemStates.Value.Get(e);

            foreach(var e1 in world.Filter<EcsGroupSystemState>().End())
            {
                ref var egss = ref _ecsGroupSystemStates.Value.Get(e1);

                if (egss.Name == gss.name)
                {
                    egss.State = gss.state;
                    var groupSystems = sharedData.gameData.groupSystems[egss.Name];

                    if (egss.State)
                    {
                        for (int k = 0; k < groupSystems.Length; k++)
                            if (groupSystems[k] is MySystem)
                                ((MySystem)groupSystems[k]).OnGroupActivate();

                        continue;
                    }

                    for (int k = 0; k < groupSystems.Length; k++)
                        if (groupSystems[k] is MySystem)
                            ((MySystem)groupSystems[k]).OnGroupDiactivate();

                    continue;
                }
            }
        }
    }
}
