using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL;
using System.Drawing;
using System.Drawing.Imaging;
using StbImageSharp;

namespace Game;

class Texture
{
    public readonly int handle;

    public int Height { get; private set; }

    public string Path { get; set; }

    public int Width { get; private set; }

    public Vector2 Size => new Vector2(Width, Height);

    public static Texture LoadEmpty(Vector2i size, TextureUnit unit = TextureUnit.Texture0, PixelType pixelType = PixelType.UnsignedByte)
    {
        int handle = GL.GenTexture();

        GL.ActiveTexture(unit);
        GL.BindTexture(TextureTarget.Texture2D, handle);

        GL.TexImage2D(
               TextureTarget.Texture2D,
                0,
                PixelInternalFormat.Rgba,
                size.X,
                size.Y,
                0,
                OpenTK.Graphics.OpenGL.PixelFormat.Rgba,
                pixelType,
                (IntPtr)0
            );

        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);

        GL.TextureParameter(handle, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
        GL.TextureParameter(handle, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);

        if (unit != TextureUnit.Texture0)
            GL.ActiveTexture(TextureUnit.Texture0);

        return new Texture(handle, size.X, size.Y);
    }

    public static Texture LoadEmpty(int width, int height, TextureUnit unit = TextureUnit.Texture0, PixelType pixelType = PixelType.UnsignedByte)
    {
        int handle = GL.GenTexture();

        GL.ActiveTexture(unit);
        GL.BindTexture(TextureTarget.Texture2D, handle);

        GL.TexImage2D(
               TextureTarget.Texture2D,
                0,
                PixelInternalFormat.Rgba,
                width,
                height,
                0,
                OpenTK.Graphics.OpenGL.PixelFormat.Rgba,
                pixelType,
                (IntPtr)0
            );

        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);

        GL.TextureParameter(handle, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
        GL.TextureParameter(handle, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);

        if (unit != TextureUnit.Texture0)
            GL.ActiveTexture(TextureUnit.Texture0);

        return new Texture(handle, width, height);
    }

    public static Texture Load(ImageResult image)
    {
        int handle = GL.GenTexture();
        
        GL.ActiveTexture(TextureUnit.Texture0);
        GL.BindTexture(TextureTarget.Texture2D, handle);

        GL.TexImage2D(
            TextureTarget.Texture2D,
            0,
            PixelInternalFormat.Rgba,
            image.Width,
            image.Height,
            0,
            OpenTK.Graphics.OpenGL.PixelFormat.Rgba,
            PixelType.UnsignedByte,
            image.Data
            );

        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);

        GL.TextureParameter(handle, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
        GL.TextureParameter(handle, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);

        return new Texture(handle, image.Width, image.Height);
    }

    public Texture(int handle, int width, int height)
    {
        this.handle = handle;

        Width = width;
        Height = height;
    }

    public void SaveRGB(string path)
    {
        byte[] data = new byte[Height * Width * 3];
        GL.GetTextureImage(this, 0, OpenTK.Graphics.OpenGL.PixelFormat.Rgb, PixelType.UnsignedByte, Width * Height * 3, data);
        Bitmap bitmap;

        using (LockedBitmap lockedBitmap = new LockedBitmap(Width, Height))
        {
            for (int y = 0; y < lockedBitmap.Height; y++)
            {
                for (int x = 0; x < lockedBitmap.Width; x++)
                {
                    byte r = data[(lockedBitmap.Width * y + x) * 3];
                    byte g = data[(lockedBitmap.Width * y + x) * 3 + 1];
                    byte b = data[(lockedBitmap.Width * y + x) * 3 + 2];

                    var color = Color.FromArgb(r, g, b);

                    lockedBitmap.SetPixel(x, y, color);
                }
            }

            bitmap = lockedBitmap.Bitmap;
        }

        using (FileStream fs = new(path, FileMode.OpenOrCreate))
            bitmap.Save(fs, ImageFormat.Png);
    }

    public void SaveRGBA(string path)
    {
        byte[] data = new byte[Height * Width * 4];
        GL.GetTextureImage<byte>(handle, 0, OpenTK.Graphics.OpenGL.PixelFormat.Rgba, PixelType.UnsignedByte, Width * Height * 4, data);
        Bitmap bitmap;

        using (LockedBitmap lockedBitmap = new LockedBitmap(Width, Height))
        {
            for (int y = 0; y < lockedBitmap.Height; y++)
            {
                for (int x = 0; x < lockedBitmap.Width; x++)
                {
                    byte r = data[(lockedBitmap.Width * y + x) * 4];
                    byte g = data[(lockedBitmap.Width * y + x) * 4 + 1];
                    byte b = data[(lockedBitmap.Width * y + x) * 4 + 2];
                    byte a = data[(lockedBitmap.Width * y + x) * 4 + 3];

                    var color = Color.FromArgb(a, r, g, b);

                    lockedBitmap.SetPixel(x, y, color);
                }
            }

            bitmap = lockedBitmap.Bitmap;
        }

        using (FileStream fs = new(path, FileMode.OpenOrCreate))
            bitmap.Save(fs, ImageFormat.Png);
    }

    public static implicit operator int(Texture texture) => texture.handle;

    public void Use() => GL.BindTexture(TextureTarget.Texture2D, handle);
}
