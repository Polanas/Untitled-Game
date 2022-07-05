namespace Game;

struct SetGroupSystemState
{
    public string name;

    public bool state;

    public SetGroupSystemState(string name, bool state)
    {
        this.name = name;
        this.state = state;
    }
}