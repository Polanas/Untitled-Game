using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTK.Graphics.OpenGL4;

namespace Game;

class PreparedSpriteInfo
{
    public readonly List<float> spriteColors = new();

    public readonly List<float> texCoords = new();

    public readonly List<float> projection1 = new();

    public readonly List<float> projection2 = new();

    public readonly List<int> isShadowCaster = new();

    public float depth;

    public readonly Texture texture;

    public PreparedSpriteInfo(Texture texture)
    {
        this.texture = texture;
    }
}

class DrawCallsPerTextureInfo
{

    public readonly Dictionary<Texture, PreparedSpriteInfo> info = new();

    public readonly List<PreparedSpriteInfo> infoList = new();

    public bool Pixelated => _pixelated;

    private SharedData _sharedData;

    private bool _pixelated;

    public DrawCallsPerTextureInfo(SharedData sharedData, bool pixelated)
    {
        _sharedData = sharedData;
        _pixelated = pixelated;
    }

    public void Clear()
    {
        info.Clear();
        infoList.Clear();
    }

    public void AddDrawCall(Texture texture, Layer layer, Vector2 position, Vector3 color, float alpha, Vector2 size, float angle, float depth, float[] texCoords)
    {
        Matrix4 projection = _sharedData.renderData.cameraLayerProjections[layer];
        var spriteColor = new List<float> { color.X / 255, color.Y / 255, color.Z / 255, alpha };
        Matrix4 model = Maths.CreateTransformMatrix(position, size, angle);
        projection = model * projection;

        var projectionArray = new float[]
        {
              projection.Column0.X, projection.Column0.Y, projection.Column0.W,
              projection.Column1.X, projection.Column1.Y, projection.Column1.W,
        };

        if (!info.ContainsKey(texture))
            info.Add(texture, new(texture));

        info[texture].spriteColors.AddRange(spriteColor);
        info[texture].texCoords.AddRange(texCoords);
        info[texture].projection1.AddRange(new float[] { projectionArray[0], projectionArray[1], projectionArray[2], projectionArray[3] });
        info[texture].projection2.AddRange(new float[] { projectionArray[4], projectionArray[5] });
        info[texture].depth = depth;
    }

    public void AddDrawCall(Renderable renderable)
    {
        var sprite = renderable.sprite;
        var texture = sprite.material != null && sprite.material.isApplying ? sprite.material.texture : sprite.Texture;

        Matrix4 projection = _sharedData.renderData.cameraLayerProjections[sprite.layer];

        var spriteColor = new List<float> { sprite.color.X / 255, sprite.color.Y / 255, sprite.color.Z / 255, sprite.alpha };
        Vector2 position = _pixelated ? new Vector2(MathF.Floor(sprite.position.X) + 0.1f, MathF.Floor(sprite.position.Y) + 0.1f) : sprite.position;
        position += sprite.offset;

        Vector2 sizeMult = new Vector2(sprite.flippedHorizontally ? -1 : 1, sprite.flippedVertically ? -1 : 1);
        Matrix4 model = Maths.CreateTransformMatrix(position, sprite.size * sprite.scale * sizeMult, sprite.angle);
        projection = model * projection;

        var projectionArray = new float[]
        {
              projection.Column0.X, projection.Column0.Y, projection.Column0.W,
              projection.Column1.X, projection.Column1.Y, projection.Column1.W,
        };

        if (!info.ContainsKey(sprite.Texture))
            info.Add(texture, new(texture));

        info[texture].spriteColors.AddRange(spriteColor);
        info[texture].texCoords.AddRange(sprite.TexCoords);
        info[texture].projection1.AddRange(new float[] { projectionArray[0], projectionArray[1], projectionArray[2], projectionArray[3] });
        info[texture].projection2.AddRange(new float[] { projectionArray[4], projectionArray[5] });
        info[texture].depth = sprite.depth; // all sprites with the same texture have the same depth
        info[texture].isShadowCaster.Add(sprite.isShadowCaster ? 1 : 0);
    }

    public void AddDrawCall(Sprite sprite)
    {
        Matrix4 projection = _sharedData.renderData.cameraLayerProjections[sprite.layer];
        var texture = sprite.material != null && sprite.material.isApplying ? sprite.material.texture : sprite.Texture;

        var spriteColor = new List<float> { sprite.color.X / 255, sprite.color.Y / 255, sprite.color.Z / 255, sprite.alpha };
        Vector2 position = _pixelated ? (new Vector2(MathF.Floor(sprite.position.X) + 0.1f, MathF.Floor(sprite.position.Y) + 0.1f)) : sprite.position;
        position += sprite.offset;

        Vector2 sizeMult = new Vector2(sprite.flippedHorizontally ? -1 : 1, sprite.flippedVertically ? -1 : 1);
        Matrix4 model = Maths.CreateTransformMatrix(position, sprite.size * sprite.scale * sizeMult, sprite.angle);
        projection = model * projection;

        var projectionArray = new float[]
        {
              projection.Column0.X, projection.Column0.Y, projection.Column0.W,
              projection.Column1.X, projection.Column1.Y, projection.Column1.W,
        };

        if (!info.ContainsKey(texture))
            info.Add(texture, new(texture));

        info[texture].spriteColors.AddRange(spriteColor);
        info[texture].texCoords.AddRange(sprite.TexCoords);
        info[texture].projection1.AddRange(new float[] { projectionArray[0], projectionArray[1], projectionArray[2], projectionArray[3] });
        info[texture].projection2.AddRange(new float[] { projectionArray[4], projectionArray[5] });
        info[texture].depth = sprite.depth;
        info[texture].isShadowCaster.Add(sprite.isShadowCaster ? 1 : 0);
    }
}

class DrawCallsPerFrameInfo
{

    public readonly Dictionary<Layer, DrawCallsPerTextureInfo> drawCallsPerTextureInfos = new();

    public readonly Dictionary<DrawCallsPerTextureInfo, Layer> drawCallsPerTextureInfosInv = new();

    public readonly List<DrawCallsPerTextureInfo> drawCallsPerTextureInfosList = new();

    public DrawCallsPerFrameInfo(SharedData sharedData)
    {
        foreach (var layer in sharedData.renderData.layersList)
        {
            DrawCallsPerTextureInfo drawCallsPerTextureInfo = new(sharedData, layer.pixelated);
            drawCallsPerTextureInfos[layer] = drawCallsPerTextureInfo;

            drawCallsPerTextureInfosInv[drawCallsPerTextureInfo] = layer;
        }

        var values = drawCallsPerTextureInfos.Values;

        foreach (var info in values)
            drawCallsPerTextureInfosList.Add(info);
    }

    public void Clear()
    {
        foreach (var info in drawCallsPerTextureInfosList)
            info.Clear();
    }
}

class RenderSpritesSystem : RenderSystem
{

    private int _FBO;

    private int _colorVBO;

    private int _texCoordSSBO;

    private int _projection1VBO;

    private int _projection2VBO;

    private int _depthVBO;

    private int _isShadowCasterVBO;

    private Shader _pixelatedShader;

    private Texture _rectangleTexture;

    private DrawCallsPerFrameInfo _perFrameInfo;

    private EcsPool<Drawable> _drawablePool = default;

    private float[] _defaultTexCoords = new float[] {
                0,0,
                1,0,
                1,1,
                0,1
    };

    private MaterialRenderer _materialRenderer;

    private Comparer<PreparedSpriteInfo> _comparer = Comparer<PreparedSpriteInfo>.Create((x, y) => x.depth.CompareTo(y.depth));

    private void Render()
    {
        Texture layerTexture;

        foreach (var spritesInfo in _perFrameInfo.drawCallsPerTextureInfosList)
        {
            layerTexture = _perFrameInfo.drawCallsPerTextureInfosInv[spritesInfo].ScreenTexture;

            GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0, TextureTarget.Texture2D, layerTexture, 0);

            //if (!spritesInfo.Pixelated)
            //{
            //    GL.DrawBuffer(DrawBufferMode.ColorAttachment0);
            //    GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment1, TextureTarget.Texture2D, 0, 0);
            //}
            //else
            //{
            //    GL.DrawBuffers(2, new DrawBuffersEnum[] { DrawBuffersEnum.ColorAttachment0, DrawBuffersEnum.ColorAttachment1 });
            //    GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment1, TextureTarget.Texture2D, sharedData.renderData.shadowCastersTexture, 0);
            //}

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            var currentShader = spritesInfo.Pixelated ? _pixelatedShader : shader;

            currentShader.Use();
            GL.BindVertexArray(VAO);

            if (spritesInfo.Pixelated)
                GL.Viewport(0, 0, 512, 512);

            float[] texCoords;
            float[] colors;
            float[] projection1;
            float[] projection2;
            float[] depths;
            int[] isShadowCaster;

            var sortedSpritesInfo = spritesInfo.info.Values.ToList();
            sortedSpritesInfo.Sort(_comparer);

            float lastDepth = 0;
            Texture texture;

            for (int i = 0; i < sortedSpritesInfo.Count; i++)
            {
                texture = sortedSpritesInfo[i].texture;

                texture.Use();

                texCoords = spritesInfo.info[texture].texCoords.ToArray();
                colors = spritesInfo.info[texture].spriteColors.ToArray();
                projection1 = spritesInfo.info[texture].projection1.ToArray();
                projection2 = spritesInfo.info[texture].projection2.ToArray();
                isShadowCaster = spritesInfo.info[texture].isShadowCaster.ToArray();

                depths = new float[colors.Length / 4];
                for (int j = 0; j < depths.Length; j++)
                {
                    depths[j] = lastDepth;
                    lastDepth += 0.0001f;
                }

                GL.BindBuffer(BufferTarget.ShaderStorageBuffer, _texCoordSSBO);
                GL.BufferData(BufferTarget.ShaderStorageBuffer, texCoords.Length * sizeof(float), texCoords, BufferUsageHint.DynamicDraw);

                GL.BindBuffer(BufferTarget.ArrayBuffer, _colorVBO);
                GL.BufferData(BufferTarget.ArrayBuffer, colors.Length * sizeof(float), colors, BufferUsageHint.DynamicDraw);

                GL.BindBuffer(BufferTarget.ArrayBuffer, _projection1VBO);
                GL.BufferData(BufferTarget.ArrayBuffer, projection1.Length * sizeof(float), projection1, BufferUsageHint.DynamicDraw);

                GL.BindBuffer(BufferTarget.ArrayBuffer, _projection2VBO);
                GL.BufferData(BufferTarget.ArrayBuffer, projection2.Length * sizeof(float), projection2, BufferUsageHint.DynamicDraw);

                GL.BindBuffer(BufferTarget.ArrayBuffer, _depthVBO);
                GL.BufferData(BufferTarget.ArrayBuffer, depths.Length * sizeof(float), depths, BufferUsageHint.DynamicDraw);

                //  if (spritesInfo.Pixelated)
                //  {
                //     GL.BindBuffer(BufferTarget.ArrayBuffer, _isShadowCasterVBO);
                //     GL.BufferData(BufferTarget.ArrayBuffer, isShadowCaster.Length * sizeof(int), isShadowCaster, BufferUsageHint.DynamicDraw);
                //  }

                GL.BindBuffer(BufferTarget.ElementArrayBuffer, EAO);
                GL.DrawElementsInstanced(PrimitiveType.Triangles, 6, DrawElementsType.UnsignedInt, (IntPtr)0, colors.Length / 4);

                GL.BindBuffer(BufferTarget.ShaderStorageBuffer, 0);
            }

            if (spritesInfo.Pixelated)
                GL.Viewport(new System.Drawing.Rectangle(0, 0, MyGameWindow.ScreenSize.X, MyGameWindow.ScreenSize.Y));
        }

    }

    public void DrawSprite(Sprite sprite)
    {
        if (sprite.material != null && sprite.material.isApplying)
            _materialRenderer.Render(sprite);

        _perFrameInfo.drawCallsPerTextureInfos[sprite.layer].AddDrawCall(sprite);
    }

    public void DrawTexture(Texture texture, Layer layer, Vector2 position, Vector2 size, Vector3 color, float depth = 0, float alpha = 1, float angle = 0) =>
        _perFrameInfo.drawCallsPerTextureInfos[layer].AddDrawCall(texture, layer, position, color, alpha, size, angle, depth, _defaultTexCoords);

    public override void Run(EcsSystems systems)
    {
        var renderables = world.GetPool<Renderable>();
        var transforms = world.GetPool<Transform>();

        foreach (int entity in world.Filter<Renderable>().End())
        {
            ref var renderable = ref renderables.Get(entity);

            if (renderable.sprite == null || renderable.sprite.layer == null || !renderable.sprite.visible)
                continue;

            var sprite = renderable.sprite;

            if (world.HasComponent<Tag>(entity) && world.GetComponent<Tag>(entity) == "player")
            {

            }

            if (transforms.Has(entity))
            {
                ref var transform = ref transforms.Get(entity);

                sprite.position = transform.position;
                sprite.angle = transform.angle;
            }

            sprite.UpdateFrame();

            if (sprite.material != null && sprite.material.isApplying)
                _materialRenderer.Render(sprite);

            DrawCallsPerTextureInfo perTextureInfo = _perFrameInfo.drawCallsPerTextureInfos[sprite.layer];
            perTextureInfo.AddDrawCall(renderable);
        }

        var rectPool = world.GetPool<DrawableRectangle>();
        foreach (var entity in world.Filter<DrawableRectangle>().Inc<Drawable>().End())
        {
            ref var rectangle = ref rectPool.Get(entity);
            _perFrameInfo.drawCallsPerTextureInfos[sharedData.renderData.layers["rectangle"]].AddDrawCall(_rectangleTexture,
                                                   sharedData.renderData.layers["rectangle"],
                                                   rectangle.position,
                                                   rectangle.color,
                                                   1f,
                                                   rectangle.size,
                                                   rectangle.angle,
                                                   0,
                                                   _defaultTexCoords);

            ref var drawable = ref _drawablePool.Get(entity);
            drawable.wereDrawn = true;
        }

        var linePool = world.GetPool<DrawableLine>();
        foreach (var entity in world.Filter<DrawableLine>().Inc<Drawable>().End())
        {
            var line = linePool.Get(entity);
            var angle = -Maths.AngleBetweenPoints(line.endPoint, line.startPoint);
            _perFrameInfo.drawCallsPerTextureInfos[sharedData.renderData.layers["rectangle"]].AddDrawCall(_rectangleTexture,
                                                   sharedData.renderData.layers["rectangle"],
                                                   (line.startPoint + line.endPoint) / 2,
                                                   line.color,
                                                   1f,
                                                   new Vector2((line.startPoint - line.endPoint).Length, line.thickness),
                                                   angle,
                                                   0,
                                                   _defaultTexCoords);

            ref var drawable = ref _drawablePool.Get(entity);
            drawable.wereDrawn = true;
        }

        GL.BindFramebuffer(FramebufferTarget.Framebuffer, _FBO);

        Render();

        GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
        GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);

        _perFrameInfo.Clear();

        _materialRenderer.OnFrameEnd();
    }

    public override void Init(EcsSystems systems)
    {
        base.Init(systems);

        shader = Content.GetShader("renderDefault");
        _pixelatedShader = Content.GetShader("renderPixelated");

        _perFrameInfo = new(sharedData);
        _drawablePool = world.GetPool<Drawable>();

        VAO = GL.GenVertexArray();
        EAO = GL.GenBuffer();
        _texCoordSSBO = GL.GenBuffer();
        _colorVBO = GL.GenBuffer();
        _projection1VBO = GL.GenBuffer();
        _projection2VBO = GL.GenBuffer();
        _depthVBO = GL.GenBuffer();
        _FBO = GL.GenFramebuffer();
        //  _isShadowCasterVBO = GL.GenBuffer();

        GL.BindVertexArray(VAO);

        uint[] indices =
        {
          0,1,3,
          1,2,3
        };

        GL.BindBuffer(BufferTarget.ElementArrayBuffer, EAO);
        GL.BufferData(BufferTarget.ElementArrayBuffer, indices.Length * sizeof(uint), indices, BufferUsageHint.StaticDraw);

        GL.BindBuffer(BufferTarget.ShaderStorageBuffer, _texCoordSSBO);
        GL.BindBufferBase(BufferRangeTarget.ShaderStorageBuffer, 0, _texCoordSSBO);
        GL.BindBuffer(BufferTarget.ShaderStorageBuffer, 0);

        GL.BindBuffer(BufferTarget.ArrayBuffer, _colorVBO);
        GL.EnableVertexAttribArray(1);
        GL.VertexAttribPointer(1, 4, VertexAttribPointerType.Float, false, 4 * sizeof(float), 0);
        GL.VertexBindingDivisor(1, 1);

        GL.BindBuffer(BufferTarget.ArrayBuffer, _projection1VBO);
        GL.EnableVertexAttribArray(2);
        GL.VertexAttribPointer(2, 4, VertexAttribPointerType.Float, false, 4 * sizeof(float), 0);
        GL.VertexBindingDivisor(2, 1);

        GL.BindBuffer(BufferTarget.ArrayBuffer, _projection2VBO);
        GL.EnableVertexAttribArray(3);
        GL.VertexAttribPointer(3, 2, VertexAttribPointerType.Float, false, 2 * sizeof(float), 0);
        GL.VertexBindingDivisor(3, 1);

        GL.BindBuffer(BufferTarget.ArrayBuffer, _depthVBO);
        GL.EnableVertexAttribArray(4);
        GL.VertexAttribPointer(4, 1, VertexAttribPointerType.Float, false, 1 * sizeof(float), 0);
        GL.VertexBindingDivisor(4, 1);

        //GL.BindBuffer(BufferTarget.ArrayBuffer, _isShadowCasterVBO);
        //GL.EnableVertexAttribArray(5);
        //GL.VertexAttribPointer(5, 1, VertexAttribPointerType.Int, false, 1 * sizeof(int), 0);
        //GL.VertexBindingDivisor(5, 1);

        GL.BindBuffer(BufferTarget.ArrayBuffer, 0);

        GL.BindFramebuffer(FramebufferTarget.Framebuffer, _FBO);
        GL.DrawBuffer(DrawBufferMode.ColorAttachment0);
        GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);

        GL.BindVertexArray(0);

        sharedData.renderData.shadowCastersTexture = Texture.LoadEmpty(new Vector2i(512), TextureUnit.Texture3);
        _rectangleTexture = Content.GetTexture("rectangleTexture");

        _materialRenderer = new();
    }
}