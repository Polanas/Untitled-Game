using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game;

class Sprite
{

    public Animation? Current => _current;

    public bool Finished => _finished;

    public int FrameWidth { get; private set; }

    public int FrameHeight { get; private set; }

    public int TexWidth => _texture.Width;

    public int TexHeight => _texture.Height;

    public int Frame
    {
        get => _frameCount;
        set
        {
            _frameCount = value;
            _frameInc = 0;

            if (_current == null)
                UpdateVertices();
        }
    }

    public int FramesAmount => (int)(_texture.Size.X / FrameWidth * (_texture.Size.Y / FrameHeight));

    public float[] TexCoords => _texCoords;

    public Texture Texture => _texture;

    public float X { get => position.X; set => position.X = value; }

    public float Y { get => position.Y; set => position.Y = value; }

    public Vector2 size;

    public float depth;

    public float angle;

    public bool flippedVertically;

    public bool flippedHorizontally;

    public bool isShadowCaster;

    public bool visible = true;

    public float scale = 1;

    public float alpha = 1;

    public Vector2 position;

    public Vector2 offset;

    public Layer layer;

    public Vector3 color = new Vector3(255);

    public Material material;

    private bool _finished;

    private List<Animation> _animations = new();

    private Animation? _current;

    private float[] _texCoords = new float[] {
                0,0,
                1,0,
                1,1,
                0,1
        };

    private Texture _texture;

    private float _frameInc;

    private int _lastFrame;

    private int _frameCount;

    public Sprite(string name, Vector2i? frameSize = null, int depth = 0, Layer layer = null)
    {
        _texture = Content.GetTexture(name);

        if (frameSize != null)
        {
            FrameWidth = frameSize.Value.X;
            FrameHeight = frameSize.Value.Y;
        }
        else
        {
            FrameWidth = _texture.Width;
            FrameHeight = _texture.Height;
        }

        size = new Vector2(FrameWidth, FrameHeight);

        this.depth = depth;
        this.layer = layer ?? Layer.Default;
    }

    public Sprite(Texture texture, Vector2i? frameSize = null, int depth = 0, Layer layer = null)
    {
        _texture = texture;

        if (frameSize != null)
        {
            FrameWidth = frameSize.Value.X;
            FrameHeight = frameSize.Value.Y;
        }
        else
        {
            FrameWidth = _texture.Width;
            FrameHeight = _texture.Height;
        }

        size = new Vector2(FrameWidth, FrameHeight);

        this.depth = depth;
        this.layer = layer ?? Layer.Default;
    }

    public void SetTexture(Texture texture) =>
        _texture = texture;

    //public Sprite Clone()
    //{
    //    Sprite result = new Sprite();

    //    result._color = _color;
    //    result._scale = _scale;
    //    result._texture = _texture;
    //    result._texCoords = _texCoords;
    //    result._size = _size;
    //    result._layer = _layer;
    //    result._frameInc = _frameInc;
    //    result._current = _current;
    //    result.FrameWidth = FrameWidth;
    //    result.FrameHeight = FrameHeight;
    //    result.Frame = Frame;
    //    result._depth = _depth;
    //    result._animations = _animations;
    //    result._alpha = _alpha;
    //    result._angle = _angle;
    //    result._offset = _offset;

    //    return result;
    //}

    public void UpdateFrame()
    {
        if (_current == null || _finished)
            return;

        _frameInc += 1f/60f;

        if (_frameInc >= 1f / (_current.Value.speed * 60f))
        {
            _frameInc = 0;
            _frameCount++;
        }

        if (_frameCount != _lastFrame)
        {

            if (_frameCount == _current.Value.frames.Length)
                if (_current.Value.loop)
                {
                    _frameCount = 0;
                }
                else
                {
                    _frameCount = _current.Value.frames.Length - 1;
                    _finished = true;
                }

            UpdateVertices();
        }

        _lastFrame = _frameCount;
    }

    public Sprite AddAnimation(string name, float speed, bool loop, int[] frames)
    {
        _animations.Add(new Animation(name, speed, loop, frames));
        return this;
    }

    public Sprite AddAnimation(string name, float speed, bool loop, int startIndex, int endIndex)
    {
        int framesCount = Math.Abs(endIndex - startIndex) + 1;
        int[] frames = new int[framesCount];

        for (int i = 0; i < framesCount; i++)
        {
            if (startIndex < endIndex)
                frames[i] = startIndex + i;
            else frames[i] = startIndex - i;
        }

        _animations.Add(new Animation(name, speed, loop, frames));
        return this;
    }

    public void SetAnimation(string name, bool saveFrame = false)
    {
        if (_current != null && _current.Value == name)
            return;

        int savedFrame;
        float savedFrameInc;

        foreach (var anim in _animations)
        {
            if (anim == name)
            {
                savedFrame = _frameCount;
                savedFrameInc = _frameInc;
                ResetCurrentAnimation();
                _current = new Animation?(anim);

                if (saveFrame)
                {
                    _frameCount = savedFrame;
                    _frameInc = savedFrameInc;
                }

                UpdateVertices();

                return;
            }
        }
    }

    public void ResetCurrentAnimation()
    {
        _frameCount = 0;
        _frameInc = 0;
        _lastFrame = 0;
        _finished = false;
    }

    private void UpdateVertices()
    {
        if (TexWidth < FrameWidth || TexHeight < FrameHeight)
            return;

        Vector2 pos;

        if (_current != null)
            pos = Maths.GetPosition(_current.Value.frames[_frameCount], TexWidth / FrameWidth);
        else pos = Maths.GetPosition(_frameCount, TexWidth / FrameWidth);

        _texCoords = new[]
    {
                pos.X * FrameWidth / TexWidth, pos.Y * FrameHeight / TexHeight,
                (pos.X+1) * FrameWidth / TexWidth, pos.Y * FrameHeight / TexHeight ,
                (pos.X+1) * FrameWidth / TexWidth, (pos.Y+1) * FrameHeight / TexHeight,
                pos.X * FrameWidth / TexWidth, (pos.Y+1) * FrameHeight / TexHeight,
            };
    }
}