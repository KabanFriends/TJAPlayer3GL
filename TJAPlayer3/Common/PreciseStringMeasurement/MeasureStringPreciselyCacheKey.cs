using System;
using System.Drawing;

namespace TJAPlayer3
{
    internal sealed class MeasureStringPreciselyCacheKey : IEquatable<MeasureStringPreciselyCacheKey>
    {
        private readonly string _text;
        private readonly Font _font;
        private readonly Size _size;
        private readonly StringAlignment _alignment;

        public MeasureStringPreciselyCacheKey(string text, Font font, Size size, StringAlignment alignment)
        {
            _text = text;
            _font = font;
            _size = size;
            _alignment = alignment;
        }

        public bool Equals(MeasureStringPreciselyCacheKey other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return string.Equals(_text, other._text) && _font.Equals(other._font) && _size.Equals(other._size) && _alignment == other._alignment;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj is MeasureStringPreciselyCacheKey other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = _text.GetHashCode();
                hashCode = (hashCode * 397) ^ _font.GetHashCode();
                hashCode = (hashCode * 397) ^ _size.GetHashCode();
                hashCode = (hashCode * 397) ^ (int) _alignment;
                return hashCode;
            }
        }

        public static bool operator ==(MeasureStringPreciselyCacheKey left, MeasureStringPreciselyCacheKey right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(MeasureStringPreciselyCacheKey left, MeasureStringPreciselyCacheKey right)
        {
            return !Equals(left, right);
        }
    }
}