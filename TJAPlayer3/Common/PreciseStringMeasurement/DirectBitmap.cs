using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace TJAPlayer3
{
    internal sealed class DirectBitmap : IDisposable
    {
        private readonly int[] _bits;
        private readonly GCHandle _bitsHandle;

        private bool _disposed;

        public Bitmap Bitmap { get; }
        public int Height { get; }
        public int Width { get; }

        public DirectBitmap(int width, int height)
        {
            Width = width;
            Height = height;
            _bits = new int[width * height];
            _bitsHandle = GCHandle.Alloc(_bits, GCHandleType.Pinned);
            Bitmap = new Bitmap(width, height, width * 4, PixelFormat.Format32bppPArgb, _bitsHandle.AddrOfPinnedObject());
        }

        public int GetPixelArgb(int x, int y)
        {
            int index = x + (y * Width);
            return _bits[index];
        }

        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;
            Bitmap.Dispose();
            _bitsHandle.Free();
        }
    }
}