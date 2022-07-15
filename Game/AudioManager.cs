using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Audio.OpenAL;
using OpenTK.Graphics.OpenGL;
using System.Collections;
using Coroutines;

namespace Game;

class Sound
{

    public readonly int buffer;

    public readonly int channels;

    public readonly int bitsPerSample;

    public readonly int sampleRate;

    public readonly byte[] data;

    public Sound(int buffer, int channels, int bitsPerSample, int sampleRate, byte[] data, int source)
    {
        this.buffer = buffer;
        this.channels = channels;
        this.bitsPerSample = bitsPerSample;
        this.sampleRate = sampleRate;
        this.data = data;
    }
}

class AudioManager
{

    private ALContext _context;

    private ALDevice _device;

    private Dictionary<string, Sound> _sounds = new();

    private CoroutineRunner _runner = new();

    private int[] _sources = new int[20];

    private int _lastSourceIndex;

    public unsafe AudioManager()
    {
        _device = ALC.OpenDevice(null);
        _context = ALC.CreateContext(_device, (int*)null);
        ALC.MakeContextCurrent(_context);

        for (int i = 0; i < _sources.Length; i++)
            _sources[i] = AL.GenSource();
    }

    public void Add(string path)
    {
        byte[] soundData;
        int channels, bits, rate;

        using (FileStream stream = new(path, FileMode.Open))
            soundData = Content.LoadWave(stream, out channels, out bits, out rate);

        var name = Path.GetFileNameWithoutExtension(path);
        _sounds.Add(name, new Sound(AL.GenBuffer(), channels, bits, rate, soundData, AL.GenSource()));
        var sound = _sounds[name];

        AL.BufferData(sound.buffer, GetSoundFormat(sound.channels, sound.bitsPerSample), ref sound.data[0], sound.data.Length, sound.sampleRate);
    }

    public void Play(string name, float volume = 1) //TODO: make this system better (more details in trello)
    {
        if (!_sounds.ContainsKey(name))
        {
#if DEBUG
            throw new ArgumentException($"Error: sound with name {name} not found!");
#endif

            return;
        }

        var sound = _sounds[name];

        _lastSourceIndex++;

        if (_lastSourceIndex == _sources.Length)
            _lastSourceIndex = 0;

        AL.GetSource(_sources[_lastSourceIndex], ALGetSourcei.SourceState, out int state);

        if ((ALSourceState)state != ALSourceState.Playing)
            AL.SourceStop(_sources[_lastSourceIndex]);

        AL.Source(_sources[_lastSourceIndex], ALSourcei.Buffer, sound.buffer);
        AL.Source(_sources[_lastSourceIndex], ALSourcef.Gain, volume);

        AL.SourcePlay(_sources[_lastSourceIndex]);
    }

    private ALFormat GetSoundFormat(int channels, int bits)
    {
        return channels switch
        {
            1 => bits == 8 ? ALFormat.Mono8 : ALFormat.Mono16,
            2 => bits == 8 ? ALFormat.Stereo8 : ALFormat.Stereo16,
            _ => throw new NotSupportedException("The specified sound format is not supported."),
        };
    }

    ~AudioManager()
    {
        ALC.DestroyContext(_context);
        ALC.CloseDevice(_device);
    }
}