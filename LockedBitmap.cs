using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Imaging;

namespace Game;

unsafe class LockedBitmap : IDisposable
{
    public Bitmap Bitmap
    {
        get
        {
            return _bitmap;
        }
    }

    public int Width { get; private set; }

    public int Height { get; private set; }

    public byte[] Data
    {
        get
        {
            byte[] data = new byte[Width * Height * 4];

            for (int y = 0; y < _bitmap.Height; y++)
            {
                byte* row = (byte*)_bitmapData.Scan0 + (y * _bitmapData.Stride);

                for (int x = 0; x < _bitmap.Width; x++)
                {
                    int index = (y * Height + x) * 4;
                    data[index + 2] = row[x * 4];
                    data[index + 1] = row[x * 4 + 1];
                    data[index] = row[x * 4 + 2];
                    data[index + 3] = row[x * 4 + 3];
                }
            }

            return data;
        }
    }

    public bool Empty
    {
        get
        {
            for (int y = 0; y < _bitmap.Height; y++)
            {
                byte* row = (byte*)_bitmapData.Scan0 + (y * _bitmapData.Stride);

                for (int x = 0; x < _bitmap.Width; x++)
                    for (int i = 0; i < 4; i++)
                    {
                        int value = row[x * 4 + i];

                        if (value != 0)
                            return false;
                    }
            }

            return true;
        }
    }

    private Bitmap _bitmap;

    private BitmapData _bitmapData;

    public static LockedBitmap FromBitmap(Bitmap map, System.Drawing.Rectangle rect, PixelFormat format)
    {
        if (rect.X < 0
            || rect.Y < 0
            || rect.X + rect.Width > map.Width
            || rect.Y + rect.Height > map.Height)
            return null;

        return new LockedBitmap(map.Clone(rect, format));
    }

    public LockedBitmap(string fileName)
    {
        _bitmap = new Bitmap(fileName);

        Width = _bitmap.Width;
        Height = _bitmap.Height;

        Lock();
    }

    public LockedBitmap(Bitmap map)
    {
        _bitmap = map;

        Width = _bitmap.Width;
        Height = _bitmap.Height;

        Lock();
    }

    public LockedBitmap(int width, int height)
    {
        _bitmap = new Bitmap(width, height);

        Width = _bitmap.Width;
        Height = _bitmap.Height;

        Lock();
    }

    public bool CompareTo(LockedBitmap[] maps)
    {
        for (int i = 0; i < maps.Length; i++)
            if (CompareTo(maps[i]))
                return true;

        return false;
    }

    public bool CompareTo(LockedBitmap[] maps, out int index)
    {
        index = -1;

        for (int i = 0; i < maps.Length; i++)
            if (CompareTo(maps[i]))
            {
                index = i;
                return true;
            }

        return false;
    }


    public bool CompareTo(LockedBitmap map)
    {
        if (_bitmap.Width != map.Width ||
            _bitmap.Height != map.Height)
            return false;

        for (int y = 0; y < _bitmap.Height; y++)
        {
            byte* row = (byte*)_bitmapData.Scan0 + (y * _bitmapData.Stride);
            byte* row1 = (byte*)map._bitmapData.Scan0 + (y * map._bitmapData.Stride);

            for (int x = 0; x < _bitmap.Width; x++)
                for (int i = 0; i < 4; i++)
                    if (row[x * 4 + i] != row1[x * 4 + i])
                        return false;
        }

        return true;
    }

    public void SetPixel(int x, int y, Color color)
    {
        byte* row = (byte*)_bitmapData.Scan0 + (y * _bitmapData.Stride);
        int startIndex = x * 4;

        row[startIndex] = color.B;
        row[startIndex + 1] = color.G;
        row[startIndex + 2] = color.R;
        row[startIndex + 3] = color.A;
    }

    public void SetPixel(int x, int y, byte r, byte g, byte b, byte a = 255)
    {
        byte* row = (byte*)_bitmapData.Scan0 + (y * _bitmapData.Stride);
        int startIndex = x * 4;

        row[startIndex] = b;
        row[startIndex + 1] = g;
        row[startIndex + 2] = r;
        row[startIndex + 3] = a;
    }

    public Color GetPixel(int x, int y)
    {
        byte a, r, g, b;
        byte* row = (byte*)_bitmapData.Scan0 + (y * _bitmapData.Stride);
        int startIndex = x * 4;

        b = row[startIndex];
        g = row[startIndex + 1];
        r = row[startIndex + 2];
        a = row[startIndex + 3];

        return Color.FromArgb(a, r, g, b);
    }

    private void Lock() =>
        _bitmapData = _bitmap.LockBits(new System.Drawing.Rectangle(0, 0, _bitmap.Width, _bitmap.Height), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);

    private void Unlock() =>
        _bitmap.UnlockBits(_bitmapData);

    /// <summary>
    /// (It actually unlocks itm not disposes)
    /// </summary>
    public void Dispose() =>
            Unlock();
}
