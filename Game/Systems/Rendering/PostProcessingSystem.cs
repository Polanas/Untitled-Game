using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTK.Graphics.OpenGL;
using System.Threading.Tasks;

namespace Game;

class PostProcessingSystem : RenderSystem
{

    private Shader _shader2;

    public override void Run(EcsSystems systems)
    {
        // shader.Use();
        //   shader.SetMatrix4("projection", sharedData.RenderData.pixelatedProjection);

        //if (Keyboard.Pressed(Keys.K))
        //      sharedData.RenderData.rectangleLayer.ScreenTexture.SaveRGBA(@"C:\Users\ivanh\Downloads\test1.png");

        GL.BindVertexArray(VAO);
        GL.Clear(ClearBufferMask.ColorBufferBit);

        //shader.UseTexture("spritesTexture", sharedData.RenderData.background1Layer.ScreenTexture, TextureUnit.Texture0);
        //shader.UseTexture("lightTexture", sharedData.RenderData.lightTexture, TextureUnit.Texture1);
        //shader.UseTexture("lightMaskTexture", sharedData.RenderData.maskTexture, TextureUnit.Texture2);
        //shader.UseTexture("shadowCastersTexture", sharedData.RenderData.shadowCastersTexture, TextureUnit.Texture3);
        //GL.DrawElements(PrimitiveType.Triangles, 6, DrawElementsType.UnsignedInt, (IntPtr)0);

        //GL.ActiveTexture(TextureUnit.Texture0);

        _shader2.Use();

        foreach (var layer in sharedData.renderData.layersList)
        {
            Matrix4 projection;

            projection = sharedData.renderData.layerProjections[layer];

            _shader2.SetMatrix4("projection", projection);
            layer.ScreenTexture.Use();

            GL.BindBuffer(BufferTarget.ElementArrayBuffer, EAO);
            GL.DrawElements(PrimitiveType.Triangles, 6, DrawElementsType.UnsignedInt, (IntPtr)0);
        }

        GL.BindVertexArray(0);
    }

    private void DrawTexture(Texture texture, Matrix4? projection = null)
    {
        if (projection is not null)
            shader.SetMatrix4("projection", projection.Value);

        shader.UseTexture("spritesTexture", texture, TextureUnit.Texture0);
        GL.DrawElements(PrimitiveType.Triangles, 6, DrawElementsType.UnsignedInt, (IntPtr)0);
    }

    public override void Init(EcsSystems systems)
    {
        base.Init(systems);

        VAO = GL.GenVertexArray();
        VBO = GL.GenBuffer();
        EAO = GL.GenBuffer();

        shader = ResourceManager.GetShader("postProcessing");
        _shader2 = ResourceManager.GetShader("postProcessing1");

        uint[] indices =
        {
          0,1,3,
          1,2,3
        };

        float[] vertices =
        {
           0,0,
           1,0,
           1,1,
           0,1
        };

        GL.BindVertexArray(VAO);

        GL.BindBuffer(BufferTarget.ElementArrayBuffer, EAO);
        GL.BufferData(BufferTarget.ElementArrayBuffer, indices.Length * sizeof(uint), indices, BufferUsageHint.StaticDraw);

        GL.BindBuffer(BufferTarget.ArrayBuffer, VBO);
        GL.EnableVertexAttribArray(0);
        GL.VertexAttribPointer(0, 2, VertexAttribPointerType.Float, false, 2 * sizeof(float), 0);
        GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsageHint.StaticDraw);
    }
}