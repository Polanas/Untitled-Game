﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StbImageSharp;

namespace Game
{
    static class ResourceManager
    {
        private static Dictionary<string, Texture> _textures = new();

        private static Dictionary<string, Shader> _shaders = new();


        public static Shader ReloadShader(string name, string vertPath, string fragPath, string geometryPath = null)
        {
            string vertSource = File.ReadAllText(vertPath);
            string fragSource = File.ReadAllText(fragPath);
            string geometrySource = null;

            if (geometryPath != null)
                geometrySource = File.ReadAllText(geometryPath);

            _shaders[name] = Shader.Reload(_shaders[name], vertSource, fragSource, geometrySource);
            return _shaders[name];
        }

        public static Shader LoadShader(string name, string vertPath, string fragPath, string geometryPath = null)
        {
            string vertSource = File.ReadAllText(vertPath);
            string fragSource = File.ReadAllText(fragPath);
            string geometrySource = null;

            if (geometryPath != null)
                geometrySource = File.ReadAllText(geometryPath);

            _shaders[name] = Shader.Load(vertSource, fragSource, name, geometrySource);
            return _shaders[name];
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

        public static Texture GetTexture(string name)
        {
            if (!_textures.ContainsKey(name))
                return _textures["unknownTexture"];


            return _textures[name];
        }
    }
}