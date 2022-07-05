namespace Game;

struct DrawableRectangle
{
    public Vector2 size;

    public Vector3 color;

    public Vector2 position;

    public float angle;

    public DrawableRectangle(Vector2 position, Vector2 size, Vector3 color, float angle = 0)
    {
        this.position = position;
        this.size = size;
        this.color = color;
        this.angle = angle;
    }
}