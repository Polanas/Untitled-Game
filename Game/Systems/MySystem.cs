using Leopotam.EcsLite.Di;

namespace Game;

abstract class MySystem : IEcsRunSystem, IEcsInitSystem, IEcsDestroySystem, IEcsSystem
{

    public bool GroupState { set => isGroupActive = value; }

    protected SharedData sharedData;

    protected EcsWorld world;

    protected bool isGroupActive;

    public virtual void Init(EcsSystems systems)
    {
        world = systems.GetWorld();
        sharedData = systems.GetShared<SharedData>();
    }

    public virtual void Run(EcsSystems systems) { }

    public virtual void Destroy(EcsSystems systems) { }

    public virtual void OnGroupDiactivate() { isGroupActive = false; }

    public virtual void OnGroupActivate() { isGroupActive = true; }
}