using System.Runtime.InteropServices;

namespace FDK
{
	public static class BitmapUtil
	{
		[StructLayout( LayoutKind.Sequential, Pack = 1 )]
		public struct BITMAPINFOHEADER
		{
			public const int BI_RGB = 0;
			public uint biSize構造体のサイズ;
			public int biWidthビットマップの幅dot;
			public int biHeightビットマップの高さdot;
			public ushort biPlanes面の数;
			public ushort biBitCount;
			public uint biCompression圧縮形式;
			public uint biSizeImage画像イメージのサイズ;
			public int biXPelsPerMete水平方向の解像度;
			public int biYPelsPerMeter垂直方向の解像度;
			public uint biClrUsed色テーブルのインデックス数;
			public uint biClrImportant表示に必要な色インデックスの数;
		}
	}
}
