using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL;

namespace Game;

class Layer
{

    public Texture ScreenTexture => _screenTexture;

    public readonly bool pixelated;

    public float cameraPosModifier;

    public int depth;

    private Texture _screenTexture;

    public Layer(Vector2i size, bool pixelated, int depth = 0, float cameraPosModifier = 1)
    {
        this.pixelated = pixelated;
        this.depth = depth;
        this.cameraPosModifier = cameraPosModifier;

        int texture = GL.GenTexture();

        GL.ActiveTexture(TextureUnit.Texture0);
        GL.BindTexture(TextureTarget.Texture2D, texture);

        GL.TexImage2D(
               TextureTarget.Texture2D,
                0,
                PixelInternalFormat.Rgba,
                size.X,
                size.Y,
                0,
                PixelFormat.Rgba,
                PixelType.UnsignedByte,
                (IntPtr)0
            );

        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);

        GL.TextureParameter(texture, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
        GL.TextureParameter(texture, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);

        _screenTexture = new Texture(texture, size.X, size.Y);
    }
}