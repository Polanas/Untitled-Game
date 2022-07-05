namespace Game;

struct Transform
{
    public Vector2 position;

    public float angle;

    public Vector2 size;

    public Transform(Vector2 position, float angle, Vector2 size)
    {
        this.angle = angle;
        this.position = position;
        this.size = size;
    }
}
