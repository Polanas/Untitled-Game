using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpFont;
using OpenTK.Graphics.OpenGL4;

namespace Game;

readonly record struct DrawCall(string text, List<float[]> vertices, Vector3i color, float alpha);

class RenderTextSystem : RenderSystem
{
    private int _FBO;

    private Library _library;

    private Dictionary<char, Character> _characters = new();

    private List<DrawCall> _drawCalls = new();

    private Matrix4 _projection;

    private uint _fontSize = 32;

    public void DrawText(string text, Vector2 position, float scale, Vector3i color, bool centered = true, float alpha = 1)
    {
        color /= 255;
        scale /= (_fontSize / 8);

        List<float[]> verticesList = new();

        Vector2 size = Vector2.Zero;

        if (centered)
        {
            Vector2 bottomLeft = position;
            Vector2 topRight = bottomLeft;
            topRight.Y = bottomLeft.Y + _fontSize * scale;

            for (int i = 0; i < text.Length; i++)
                topRight.X += (_characters[text[i]].advance >> 6) * scale;

            size.X = topRight.X - bottomLeft.X;
            size.Y = bottomLeft.Y - topRight.Y;
        }

        for (int i = 0; i < text.Length; i++)
        {
            char ind = text[i];
            var character = _characters[ind];

            float xPos = position.X + character.bearing.X * scale;
            float yPos = position.Y + (character.size.Y - character.bearing.Y) * scale;

            float w = character.size.X * scale;
            float h = character.size.Y * scale;

            h *= -1;
            xPos -= size.X / 2;
            yPos -= size.Y / 2;
           // yPos += character.bearing.X - 

            float[] vertices = new float[] {
             xPos,     yPos + h,   0.0f, 0.0f,
             xPos,     yPos,       0.0f, 1.0f,
             xPos + w, yPos,       1.0f, 1.0f,

             xPos,     yPos + h,   0.0f, 0.0f,
             xPos + w, yPos,       1.0f, 1.0f,
             xPos + w, yPos + h,   1.0f, 0.0f,
            };

            verticesList.Add(vertices);

            position.X += (character.advance >> 6) * scale;
        }

        _drawCalls.Add(new(text, verticesList, color, alpha));
    }

    public override void Run(EcsSystems systems)
    {
        if (_drawCalls.Count == 0)
            return;

        GL.BindVertexArray(VAO);
        GL.BindFramebuffer(FramebufferTarget.Framebuffer, _FBO);

        shader.Use();
        shader.SetMatrix4("projection", _projection);

        for (int i = 0; i < _drawCalls.Count; i++)
        {
            shader.SetVector3("textColor", _drawCalls[i].color);
            shader.SetFloat("alpha", _drawCalls[i].alpha);

            for (int j = 0; j < _drawCalls[i].text.Length; j++)
            {
                GL.BindTexture(TextureTarget.Texture2D, _characters[_drawCalls[i].text[j]].textureID);

                GL.BindBuffer(BufferTarget.ArrayBuffer, VBO);
                GL.BufferData(BufferTarget.ArrayBuffer, _drawCalls[i].vertices[j].Length * sizeof(float), _drawCalls[i].vertices[j], BufferUsageHint.DynamicDraw);

                GL.DrawArrays(PrimitiveType.Triangles, 0, 6);
            }
        }

        GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
        GL.BindVertexArray(0);

        _drawCalls.Clear();
    }

    public override void Init(EcsSystems systems)
    {
        base.Init(systems);

        _library = new Library();

        Face face = new Face(_library, @"Content\Fonts\PixelFJVerdana12pt.TTF");
        face.SetPixelSizes(_fontSize, _fontSize);

        GL.PixelStore(PixelStoreParameter.UnpackAlignment, 1);

        for (uint c = 0; c < 128; c++)
        {
            face.LoadChar(c, LoadFlags.Render, LoadTarget.Normal);

            int texture = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2D, texture);
            GL.TexImage2D(
                TextureTarget.Texture2D,
                0,
                PixelInternalFormat.CompressedRed,
                face.Glyph.Bitmap.Width,
                face.Glyph.Bitmap.Rows,
                0,
                PixelFormat.Red,
                PixelType.UnsignedByte,
                face.Glyph.Bitmap.Buffer
                );

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);

            GL.TextureParameter(texture, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
            GL.TextureParameter(texture, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);

            Character character = new Character(
                texture,
                new Vector2i(face.Glyph.Bitmap.Width, face.Glyph.Bitmap.Rows),
                new Vector2i(face.Glyph.BitmapLeft, face.Glyph.BitmapTop),
                face.Glyph.Advance.X.Value
                );

            _characters[(char)c] = character;
        }

        face.Dispose();
        _library.Dispose();

        shader = Content.GetShader("text");

        VBO = GL.GenBuffer();
        VAO = GL.GenVertexArray();
        _FBO = GL.GenFramebuffer();

        GL.BindVertexArray(VAO);

        GL.BindBuffer(BufferTarget.ArrayBuffer, VBO);
        GL.EnableVertexAttribArray(0);
        GL.VertexAttribPointer(0, 4, VertexAttribPointerType.Float, false, 4 * sizeof(float), 0);
        GL.BindBuffer(BufferTarget.ArrayBuffer, 0);

        GL.BindFramebuffer(FramebufferTarget.Framebuffer, _FBO);
        GL.DrawBuffer(DrawBufferMode.ColorAttachment0);
        GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0, TextureTarget.Texture2D, sharedData.renderData.layers["text"].ScreenTexture, 0);
        GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);

        GL.BindVertexArray(0);

        _projection = sharedData.renderData.cameraLayerProjections[sharedData.renderData.layers["text"]];
    }
}
