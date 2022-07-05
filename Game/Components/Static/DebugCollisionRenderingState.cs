namespace Game;

struct DebugCollisionDrawingState : IEcsAutoReset<DebugCollisionDrawingState>
{
    public bool draw;

    public void AutoReset(ref DebugCollisionDrawingState c)
    {
        c.draw = true;
    }
}
