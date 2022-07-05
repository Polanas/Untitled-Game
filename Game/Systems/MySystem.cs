using Leopotam.EcsLite.Di;

namespace Game;

abstract class MySystem : IEcsRunSystem, IEcsInitSystem, IEcsDestroySystem, IEcsSystem
{

    protected readonly EcsWorldInject _worldInject;

    protected SharedData sharedData;

    protected EcsWorld world;

    public virtual void Init(EcsSystems systems)
    {
        world = _worldInject.Value;
        sharedData = systems.GetShared<SharedData>();
    }

    public virtual void Run(EcsSystems systems) { }

    public virtual void Destroy(EcsSystems systems) { }

    public virtual void OnGroupDiactivate() { }

    public virtual void OnGroupActivate() { }
}