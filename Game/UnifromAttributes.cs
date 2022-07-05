using OpenTK.Graphics.OpenGL;

namespace Game;

enum UniformType
{
    Float,
    Vector2,
    Vector3,
    Vector4,
    Int,
    Bool,
    Texture
}

[AttributeUsage(AttributeTargets.Field)]
class UniformAttribute : Attribute
{

    public readonly UniformType uniformType;

    public readonly string uniformName;

    public UniformAttribute(string uniformName, UniformType type)
    {
        uniformType = type;
        this.uniformName = uniformName;
    }
}