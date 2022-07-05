using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL;

namespace Game;

class Material
{
    public readonly Shader shader;

    public readonly string name;

    public readonly Texture texture;

    public readonly Sprite sprite;

    protected readonly Dictionary<string, object> uniformInfo;

    public bool isApplying = true;

    public Material(Shader shader, Sprite sprite)
    {
        this.shader = shader;
        this.sprite = sprite;
        name = shader.name;
       texture = Texture.LoadEmpty(sprite.TexWidth, sprite.TexHeight);
    }

    public void Dispose()
    {
        GL.DeleteTexture(texture.handle);
    }
}