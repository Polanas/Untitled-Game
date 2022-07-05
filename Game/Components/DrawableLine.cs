namespace Game;

struct DrawableLine
{
    public Vector2 startPoint;

    public Vector2 endPoint;

    public Vector3 color;

    public float thickness;

    public DrawableLine(Vector2 startPoint, Vector2 endPoint, Vector3 color, float thickness = 1)
    {
        this.startPoint = startPoint;
        this.endPoint = endPoint;
        this.thickness = thickness;
        this.color = color;
    }
}
