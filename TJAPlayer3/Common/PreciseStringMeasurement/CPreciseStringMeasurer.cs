using System.Collections.Generic;
using System.Drawing;

namespace TJAPlayer3
{
    internal static class CPreciseStringMeasurer
    {
        private static readonly Dictionary<MeasureStringPreciselyCacheKey, Rectangle> MeasureStringPreciselyCache =
            new Dictionary<MeasureStringPreciselyCacheKey, Rectangle>();

        //------------------------------------------------
        //使用:http://dobon.net/vb/dotnet/graphics/measurestring.html

	    /// <summary>
	    /// Graphics.DrawStringで文字列を描画した時の大きさと位置を正確に計測する
	    /// </summary>
	    /// <param name="g">文字列を描画するGraphics</param>
	    /// <param name="text">描画する文字列</param>
	    /// <param name="font">描画に使用するフォント</param>
	    /// <param name="proposedSize">これ以上大きいことはないというサイズ。
	    /// できるだけ小さくすること。</param>
	    /// <param name="stringFormat">描画に使用するStringFormat</param>
	    /// <returns>文字列が描画される範囲。
	    /// 見つからなかった時は、Rectangle.Empty。</returns>
	    internal static Rectangle MeasureStringPrecisely(Graphics g,
	        string text, Font font, Size proposedSize, StringFormat stringFormat)
	    {
	        var measureStringPreciselyCacheKey = new MeasureStringPreciselyCacheKey(text, font, proposedSize, stringFormat.Alignment);
	        if (!MeasureStringPreciselyCache.TryGetValue(measureStringPreciselyCacheKey, out var result))
	        {
	            result = MeasureStringPreciselyUncached(g, text, font, proposedSize, stringFormat);
	            MeasureStringPreciselyCache.Add(measureStringPreciselyCacheKey, result);
	        }

	        return result;
	    }

	    private static Rectangle MeasureStringPreciselyUncached(Graphics g,
            string text, Font font, Size proposedSize, StringFormat stringFormat)
        {
            //解像度を引き継いで、Bitmapを作成する
            using (var bmp = new DirectBitmap(proposedSize.Width, proposedSize.Height))
            {
                Graphics bmpGraphics = Graphics.FromImage(bmp.Bitmap);
                //Graphicsのプロパティを引き継ぐ
                bmpGraphics.TextRenderingHint = g.TextRenderingHint;
                bmpGraphics.TextContrast = g.TextContrast;
                bmpGraphics.PixelOffsetMode = g.PixelOffsetMode;
                //文字列の描かれていない部分の色を取得する
                int backColorArgb = bmp.GetPixelArgb(0, 0);
                //実際にBitmapに文字列を描画する
                bmpGraphics.DrawString(text, font, Brushes.Black,
                    new RectangleF(0f, 0f, proposedSize.Width, proposedSize.Height),
                    stringFormat);
                bmpGraphics.Dispose();
                //文字列が描画されている範囲を計測する
                return MeasureForegroundArea(bmp, backColorArgb);
            }
        }

        /// <summary>
        /// 指定されたBitmapで、backColor以外の色が使われている範囲を計測する
        /// </summary>
        private static Rectangle MeasureForegroundArea(DirectBitmap bmp, int backColorArgb)
        {
            int maxWidth = bmp.Width;
            int maxHeight = bmp.Height;

            //左側の空白部分を計測する
            int leftPosition = -1;
            for (int x = 0; x < maxWidth; x++)
            {
                for (int y = 0; y < maxHeight; y++)
                {
                    //違う色を見つけたときは、位置を決定する
                    if (bmp.GetPixelArgb(x, y) != backColorArgb)
                    {
                        leftPosition = x;
                        break;
                    }
                }
                if (0 <= leftPosition)
                {
                    break;
                }
            }
            //違う色が見つからなかった時
            if (leftPosition < 0)
            {
                return Rectangle.Empty;
            }

            //右側の空白部分を計測する
            int rightPosition = -1;
            for (int x = maxWidth - 1; leftPosition < x; x--)
            {
                for (int y = 0; y < maxHeight; y++)
                {
                    if (bmp.GetPixelArgb(x, y) != backColorArgb)
                    {
                        rightPosition = x;
                        break;
                    }
                }
                if (0 <= rightPosition)
                {
                    break;
                }
            }
            if (rightPosition < 0)
            {
                rightPosition = leftPosition;
            }

            //上の空白部分を計測する
            int topPosition = -1;
            for (int y = 0; y < maxHeight; y++)
            {
                for (int x = leftPosition; x <= rightPosition; x++)
                {
                    if (bmp.GetPixelArgb(x, y) != backColorArgb)
                    {
                        topPosition = y;
                        break;
                    }
                }
                if (0 <= topPosition)
                {
                    break;
                }
            }
            if (topPosition < 0)
            {
                return Rectangle.Empty;
            }

            //下の空白部分を計測する
            int bottomPosition = -1;
            for (int y = maxHeight - 1; topPosition < y; y--)
            {
                for (int x = leftPosition; x <= rightPosition; x++)
                {
                    if (bmp.GetPixelArgb(x, y) != backColorArgb)
                    {
                        bottomPosition = y;
                        break;
                    }
                }
                if (0 <= bottomPosition)
                {
                    break;
                }
            }
            if (bottomPosition < 0)
            {
                bottomPosition = topPosition;
            }

            //結果を返す
            return new Rectangle(leftPosition, topPosition,
                rightPosition - leftPosition, bottomPosition - topPosition);
        }
    }
}
