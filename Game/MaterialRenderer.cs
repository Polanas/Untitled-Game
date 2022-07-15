using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL4;
using System.Reflection;

namespace Game;

class MaterialRenderer
{

    struct MaterialFieldInfo
    {
        public FieldInfo field;

        public UniformAttribute uniformInfo;

        public MaterialFieldInfo(FieldInfo field, UniformAttribute uniformInfo)
        {
            this.field = field;
            this.uniformInfo = uniformInfo;
        }
    }

    private int _VAO;

    private int _textureUnit;

    private int _EAO;

    private int _FBO;

    private int _texCoordsSSBO;

    private Shader _lastShader;

    private Dictionary<Type, List<MaterialFieldInfo>> _materialUnifromFields = new();

    public MaterialRenderer()
    {
        Init();
    }

    public void Render(Sprite sprite) //TODO: add unifroms using attributes
    {
        if ((sprite.material.shader is var shader) && shader != _lastShader)
            sprite.material.shader.Use();

        GL.BindVertexArray(_VAO);
        GL.BindFramebuffer(FramebufferTarget.Framebuffer, _FBO);
        GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0, TextureTarget.Texture2D, sprite.material.texture, 0);

        GL.Clear(ClearBufferMask.ColorBufferBit);

        GL.Viewport(-sprite.material.texture.Width, -sprite.material.texture.Height, sprite.material.texture.Width * 2, sprite.material.texture.Height * 2);

        GL.BindBuffer(BufferTarget.ShaderStorageBuffer, _texCoordsSSBO);
        GL.BufferData(BufferTarget.ShaderStorageBuffer, sprite.TexCoords.Length * sizeof(float), new float[] { 0, 0, 1, 0, 1, 1, 0, 1 }, BufferUsageHint.DynamicDraw);

        shader.UseTexture("image", sprite.Texture, TextureUnit.Texture0);

        var materialInfo = _materialUnifromFields[sprite.material.GetType()];
        _textureUnit = (int)TextureUnit.Texture1;

        for (int i = 0; i < materialInfo.Count; i++)
        {
            var value = materialInfo[i].field.GetValue(sprite.material);

            switch (materialInfo[i].uniformInfo.uniformType)
            {
                case UniformType.Float:
                    shader.SetFloat(materialInfo[i].uniformInfo.uniformName, (float)value);
                    break;
                case UniformType.Vector2:
                    shader.SetVector2(materialInfo[i].uniformInfo.uniformName, (Vector2)value);
                    break;
                case UniformType.Vector3:
                    shader.SetVector3(materialInfo[i].uniformInfo.uniformName, (Vector3)value);
                    break;
                case UniformType.Vector4:
                    shader.SetVector4(materialInfo[i].uniformInfo.uniformName, (Vector4)value);
                    break;
                case UniformType.Int:
                    shader.SetInt(materialInfo[i].uniformInfo.uniformName, (int)value);
                    break;
                case UniformType.Bool:
                    shader.SetBool(materialInfo[i].uniformInfo.uniformName, (bool)value);
                    break;
                case UniformType.Texture:
                    shader.UseTexture(materialInfo[i].uniformInfo.uniformName, (Texture)value, (TextureUnit)_textureUnit);
                    _textureUnit++;
                    break;
            }
        }


        shader.SetVector2("framesAmount", new Vector2(sprite.TexWidth / sprite.FrameWidth, sprite.TexHeight / sprite.FrameHeight));

        GL.BindBuffer(BufferTarget.ElementArrayBuffer, _EAO);
        GL.DrawElements(PrimitiveType.Triangles, 6, DrawElementsType.UnsignedInt, (IntPtr)0);

        for (int i = _textureUnit; i >= (int)TextureUnit.Texture1; i--)
            GLUtils.ClearTextureUnit((TextureUnit)i);
        

        GL.ActiveTexture(TextureUnit.Texture0);

        GL.BindVertexArray(0);
        GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);

        _lastShader = sprite.material.shader;
        _textureUnit = 0;

        GL.Viewport(0, 0, 1920, 1080); //TODO: remake this

    }

    public void OnFrameEnd()
    {
        _lastShader = null;
    }

    public void Init()
    {
        _VAO = GL.GenVertexArray();
        _EAO = GL.GenBuffer();
        _FBO = GL.GenFramebuffer();
        _texCoordsSSBO = GL.GenBuffer();

        GL.BindVertexArray(_VAO);

        GL.BindBuffer(BufferTarget.ShaderStorageBuffer, _texCoordsSSBO);
        GL.BindBufferBase(BufferRangeTarget.ShaderStorageBuffer, 1, _texCoordsSSBO);
        GL.BindBuffer(BufferTarget.ShaderStorageBuffer, 0);

        uint[] indices =
        {
          0,1,3,
          1,2,3
        };

        GL.BindBuffer(BufferTarget.ElementArrayBuffer, _EAO);
        GL.BufferData(BufferTarget.ElementArrayBuffer, indices.Length * sizeof(uint), indices, BufferUsageHint.StaticDraw);
        GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);

        GL.BindFramebuffer(FramebufferTarget.Framebuffer, _FBO);
        GL.DrawBuffers(1, new DrawBuffersEnum[] { DrawBuffersEnum.ColorAttachment0 });
        GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);

        GL.BindVertexArray(0);

        var materialTypes = Assembly.GetExecutingAssembly().GetTypes().Where(t => t.IsAssignableTo(typeof(Material)) && t != typeof(Material)).ToList();

        foreach (var type in materialTypes)
        {
            var fields = type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            List<MaterialFieldInfo> materialInfo = new();

            for (int i = 0; i < fields.Length; i++)
            {
                if ((fields[i].GetCustomAttribute(typeof(UniformAttribute)) is var attrbute) && attrbute == null)
                    continue;

                materialInfo.Add(new MaterialFieldInfo(fields[i], (UniformAttribute)attrbute));
            }

            _materialUnifromFields[type] = materialInfo;
        }
    }
}