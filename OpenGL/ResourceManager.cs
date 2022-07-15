﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StbImageSharp;

public enum SourceType
{
    Frag,
    Vert,
    Geom,
}

namespace Game
{
    static class Content
    {
        private static Dictionary<string, Texture> _textures = new();

        private static Dictionary<string, Shader> _shaders = new();

        private static Dictionary<string, string> _fragShaderSources = new();

        private static Dictionary<string, string> _vertShaderSources = new();

        private static Dictionary<string, string> _geomShaderSources = new();

        public static Shader ReloadShader(string name, string vertPath, string fragPath, string geometryPath = null)
        {
            string vertSource = File.ReadAllText(vertPath);
            string fragSource = File.ReadAllText(fragPath);
            string geometrySource = null;

            if (geometryPath != null)
                geometrySource = File.ReadAllText(geometryPath);

            Shader shader = Shader.Reload(_shaders[name], vertSource, fragSource, geometrySource);
            _shaders[name] = shader;

            return shader;
        }

        public static void LoadShaderSource(SourceType type, string path)
        {
            string source = File.ReadAllText(path);
            string name = Path.GetFileNameWithoutExtension(path);

            switch (type)
            {
                case SourceType.Vert:
                    _vertShaderSources[name] = source;
                    break;
                case SourceType.Frag:
                    _fragShaderSources[name] = source;
                    break;
                case SourceType.Geom:
                    _geomShaderSources[name] = source;
                    break;
            }
        }

        public static Shader RequestShader(string name, string localVertPath, string localFragPath, string localGeometryPath = null)
        {
            if (_shaders.ContainsKey(name))
                return _shaders[name];

            Shader shader = LoadShader(name, @"Content\Shaders\" + localVertPath, @"Content\Shaders\" + localFragPath, localGeometryPath == null ? null : @"Content\Shaders\" + localGeometryPath);
            _shaders[name] = shader;

            return shader;
        }

        public static Shader LoadShader(string name, string vertPath, string fragPath, string geometryPath = null)
        {
            string vertSource = File.ReadAllText(vertPath);
            string fragSource = File.ReadAllText(fragPath);
            string geometrySource = null;

            if (geometryPath != null)
                geometrySource = File.ReadAllText(geometryPath);

            Shader shader = Shader.Load(vertSource, fragSource, name, geometrySource);
            _shaders[name] = shader;

            return shader;
        }

        public static Texture LoadTexture(string name, string path)
        {
            ImageResult image;

#if DEBUG
            if (!File.Exists(path))
                throw new ArgumentException(@$"Texture with path ""{path}"" doesn't exsist!");


            using (var stream = File.OpenRead(path))
                try
                {
                    image = ImageResult.FromStream(stream, ColorComponents.RedGreenBlueAlpha);
                }
                catch { throw new ArgumentException($"Texture with path {path} couldn't be loaded!"); }
#else
            using (var stream = File.OpenRead(path))
                image = ImageResult.FromStream(stream, ColorComponents.RedGreenBlueAlpha);

#endif

            _textures[name] = Texture.Load(image);
            _textures[name].Path = path;

            return _textures[name];
        }

        public static byte[] LoadWave(Stream stream, out int channels, out int bits, out int rate)
        {
            using (BinaryReader reader = new(stream))
            {
                string signature = new string(reader.ReadChars(4));
                if (signature != "RIFF")
                    throw new NotSupportedException("Specified stream is not a wave file.");

                int riffChunkSize = reader.ReadInt32();

                string format = new string(reader.ReadChars(4));
                if (format != "WAVE")
                    throw new NotSupportedException("Specified stream is not a wave file.");

                string formatSignature = new string(reader.ReadChars(4));
                if (formatSignature != "fmt ")
                    throw new NotSupportedException("Specified wave file is not supported.");

                int formatChunkSize = reader.ReadInt32();
                int audioFormat = reader.ReadInt16();
                channels = reader.ReadInt16();
                rate = reader.ReadInt32();
                int byteRate = reader.ReadInt32();
                int blockAlign = reader.ReadInt16();
                bits = reader.ReadInt16();

                string dataSignature = new string(reader.ReadChars(4));
                if (dataSignature != "data")
                    throw new NotSupportedException("Specified wave file is not supported.");

                int dataChunkSize = reader.ReadInt32();

                return reader.ReadBytes((int)reader.BaseStream.Length);
            }
        }

        public static Shader GetShader(string name) => _shaders[name];

        public static string GetShaderSource(SourceType type, string fileName) =>
            type switch
            {
                SourceType.Frag => _fragShaderSources[fileName],
                SourceType.Vert => _vertShaderSources[fileName],
                SourceType.Geom => _geomShaderSources[fileName],
            };


        public static Texture GetTexture(string name)
        {
            if (!_textures.ContainsKey(name))
                return _textures["unknownTexture"];

            return _textures[name];
        }
    }
}