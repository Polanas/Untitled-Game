namespace Game;

class Service
{
    protected SharedData sharedData;

    protected EcsWorld world;

    public virtual void PreInit(SharedData sharedData, EcsWorld world)
    {
        this.sharedData = sharedData;
        this.world = world;
    }

    public virtual void PostInit() { }
}
