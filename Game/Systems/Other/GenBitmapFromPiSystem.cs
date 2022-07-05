using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Threading.Tasks;
using StbImageSharp;
using OpenTK.Graphics.OpenGL;

namespace Game;

class GenBitmapFromPiSystem : MySystem
{

    private const float _PiToBytesRatio = 999f / 255f;

    public override void Init(EcsSystems systems)
    {
        base.Init(systems);

        byte[] data;

        using (Stream stream = File.Open(@"C:\Users\ivanh\Downloads\one-million.txt", FileMode.Open))
        using (BinaryReader reader = new(stream))
        {
            data = new byte[reader.BaseStream.Length];
            reader.Read(data, 0, (int)reader.BaseStream.Length);
        }

        string PiSeparated = Encoding.Default.GetString(data);
        string[] lines = PiSeparated.Split('\n');
        var Pi = string.Join("", lines);
        int length = Pi.Length;
        Bitmap map;

        using (LockedBitmap lockedMap = new(330, 330))
        {
            for (int y = 0; y < lockedMap.Height; y++)
            {
                for (int x = 0; x < lockedMap.Width; x++)
                {
                    int index = (y * lockedMap.Width + x) * 9;
                    DrawPixel(lockedMap, x, y, Pi.Substring(index, 9));
                }
            }

            map = lockedMap.Bitmap;

            map.Save(@"C:\Users\ivanh\Downloads\test.png", System.Drawing.Imaging.ImageFormat.Png);
        }
    }

    private void DrawPixel(LockedBitmap map, int x, int y, string bytes)
    {
        (float r, float g, float b) = GetPixel(bytes);
        map.SetPixel(x, y, (byte)(r), (byte)(g), (byte)(b));
    }

    private (float r, float g, float b) GetPixel(string bytes)
    {
        var r = float.Parse(bytes.Substring(0, 3));
        var g = float.Parse(bytes.Substring(3, 3));
        var b = float.Parse(bytes.Substring(6, 3));

        return (r, g, b);
    }
}
