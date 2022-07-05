namespace Game;

class Service
{
    protected SharedData sharedData;

    protected EcsWorld world;

    public virtual void Init(SharedData sharedData, EcsWorld world)
    {
        this.sharedData = sharedData;
        this.world = world;
    }
}
