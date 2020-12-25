﻿using System;
using System.IO;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Diagnostics;
using System.Linq;
using TJAPlayer3.Common;

namespace TJAPlayer3
{
	/// <summary>
	/// プライベートフォントでの描画を扱うクラス。
	/// </summary>
	/// <exception cref="FileNotFoundException">フォントファイルが見つからない時に例外発生</exception>
	/// <exception cref="ArgumentException">スタイル指定不正時に例外発生</exception>
	/// <remarks>
	/// 簡単な使い方
	/// CPrivateFont prvFont = new CPrivateFont( CSkin.Path( @"Graphics\fonts\mplus-1p-bold.ttf" ), 36 );	// プライベートフォント
	/// とか
	/// CPrivateFont prvFont = new CPrivateFont( new FontFamily("MS UI Gothic"), 36, FontStyle.Bold );		// システムフォント
	/// とかした上で、
	/// Bitmap bmp = prvFont.DrawPrivateFont( "ABCDE", Color.
	/// , Color.Black );							// フォント色＝白、縁の色＝黒の例。縁の色は省略可能
	/// とか
	/// Bitmap bmp = prvFont.DrawPrivateFont( "ABCDE", Color.White, Color.Black, Color.Yellow, Color.OrangeRed ); // 上下グラデーション(Yellow→OrangeRed)
	/// とかして、
	/// CTexture ctBmp = CDTXMania.tテクスチャの生成( bmp, false );
	/// ctBMP.t2D描画( ～～～ );
	/// で表示してください。
	/// 
	/// 注意点
	/// 任意のフォントでのレンダリングは結構負荷が大きいので、なるべｋなら描画フレーム毎にフォントを再レンダリングするようなことはせず、
	/// 一旦レンダリングしたものを描画に使い回すようにしてください。
	/// また、長い文字列を与えると、返されるBitmapも横長になります。この横長画像をそのままテクスチャとして使うと、
	/// 古いPCで問題を発生させやすいです。これを回避するには、一旦Bitmapとして取得したのち、256pixや512pixで分割して
	/// テクスチャに定義するようにしてください。
	/// </remarks>

	#region In拡張子
	public static class StringExtensions
	{
		public static bool In(this string str, params string[] param)
		{
			return param.Contains(str);
		}
	}
	#endregion

	public class CPrivateFont : IDisposable
	{
		#region [ コンストラクタ ]
		public CPrivateFont(FontFamily fontfamily, int pt, FontStyle style)
		{
			Initialize(null, fontfamily, pt, style);
		}
		public CPrivateFont(FontFamily fontfamily, int pt)
		{
			Initialize(null, fontfamily, pt, FontStyle.Regular);
		}
		public CPrivateFont(string fontpath, int pt, FontStyle style)
		{
			Initialize(fontpath, null, pt, style);
		}
		public CPrivateFont(string fontpath, int pt)
		{
			Initialize(fontpath, null, pt, FontStyle.Regular);
		}
		public CPrivateFont()
		{
			//throw new ArgumentException("CPrivateFont: 引数があるコンストラクタを使用してください。");
		}
		#endregion

		protected void Initialize(string fontpath, FontFamily fontfamily, int pt, FontStyle style)
		{
			this._pfc = null;
			this._fontfamily = null;
			this._font = null;
			this._pt = pt;
			this._rectStrings = new Rectangle(0, 0, 0, 0);
			this._ptOrigin = new Point(0, 0);
			this.bDispose完了済み = false;

			if (fontfamily != null)
			{
				this._fontfamily = fontfamily;
			}
			else
			{
				try
				{
					this._pfc = new System.Drawing.Text.PrivateFontCollection();    //PrivateFontCollectionオブジェクトを作成する
					this._pfc.AddFontFile(fontpath);                                //PrivateFontCollectionにフォントを追加する
					_fontfamily = _pfc.Families[0];
				}
				catch (System.IO.FileNotFoundException)
				{
					Trace.TraceWarning($"プライベートフォントの追加に失敗しました({fontpath})。代わりに{FontUtilities.FallbackFontName}の使用を試みます。");
					//throw new FileNotFoundException( "プライベートフォントの追加に失敗しました。({0})", Path.GetFileName( fontpath ) );
					//return;
					_fontfamily = null;
				}
			}

			// 指定されたフォントスタイルが適用できない場合は、フォント内で定義されているスタイルから候補を選んで使用する
			// 何もスタイルが使えないようなフォントなら、例外を出す。
			if (_fontfamily != null)
			{
				if (!_fontfamily.IsStyleAvailable(style))
				{
					FontStyle[] FS = { FontStyle.Regular, FontStyle.Bold, FontStyle.Italic, FontStyle.Underline, FontStyle.Strikeout };
					style = FontStyle.Regular | FontStyle.Bold | FontStyle.Italic | FontStyle.Underline | FontStyle.Strikeout;  // null非許容型なので、代わりに全盛をNGワードに設定
					foreach (FontStyle ff in FS)
					{
						if (this._fontfamily.IsStyleAvailable(ff))
						{
							style = ff;
							Trace.TraceWarning("フォント{0}へのスタイル指定を、{1}に変更しました。", Path.GetFileName(fontpath), style.ToString());
							break;
						}
					}
					if (style == (FontStyle.Regular | FontStyle.Bold | FontStyle.Italic | FontStyle.Underline | FontStyle.Strikeout))
					{
						Trace.TraceWarning("フォント{0}は適切なスタイル{1}を選択できませんでした。", Path.GetFileName(fontpath), style.ToString());
					}
				}
				//this._font = new Font(this._fontfamily, pt, style);			//PrivateFontCollectionの先頭のフォントのFontオブジェクトを作成する
				float emSize = pt * 96.0f / 72.0f;
				this._font = new Font(this._fontfamily, emSize, style, GraphicsUnit.Pixel); //PrivateFontCollectionの先頭のフォントのFontオブジェクトを作成する
																							//HighDPI対応のため、pxサイズで指定
			}
			else
			// フォントファイルが見つからなかった場合 (MS UI Gothicを代わりに指定する)
			{
				try
				{
					_fontfamily = new FontFamily(FontUtilities.FallbackFontName);
					float emSize = pt * 96.0f / 72.0f;
					_font = new Font(_fontfamily, emSize, style, GraphicsUnit.Pixel);
					Trace.TraceInformation($"{FontUtilities.FallbackFontName}を代わりに指定しました。");
				}
				catch (Exception e)
				{
					throw new FileNotFoundException($"プライベートフォントの追加に失敗し、{FontUtilities.FallbackFontName}での代替処理にも失敗しました。({Path.GetFileName(fontpath)})", e);
				}
			}
		}

		[Flags]
		public enum DrawMode
		{
			Normal,
			Edge,
			Gradation,
			Vertical
		}

		#region [ DrawPrivateFontのオーバーロード群 ]
		/// <summary>
		/// 文字列を描画したテクスチャを返す
		/// </summary>
		/// <param name="drawstr">描画文字列</param>
		/// <param name="fontColor">描画色</param>
		/// <returns>描画済テクスチャ</returns>
		public Bitmap DrawPrivateFont(string drawstr, Color fontColor)
		{
			return DrawPrivateFont(drawstr, DrawMode.Normal, fontColor, Color.White, Color.White, Color.White);
		}

		/// <summary>
		/// 文字列を描画したテクスチャを返す
		/// </summary>
		/// <param name="drawstr">描画文字列</param>
		/// <param name="fontColor">描画色</param>
		/// <param name="edgeColor">縁取色</param>
		/// <returns>描画済テクスチャ</returns>
		public Bitmap DrawPrivateFont(string drawstr, Color fontColor, Color edgeColor)
		{
			return DrawPrivateFont(drawstr, DrawMode.Edge, fontColor, edgeColor, Color.White, Color.White);
		}

		/// <summary>
		/// 文字列を描画したテクスチャを返す
		/// </summary>
		/// <param name="drawstr">描画文字列</param>
		/// <param name="fontColor">描画色</param>
		/// <param name="gradationTopColor">グラデーション 上側の色</param>
		/// <param name="gradationBottomColor">グラデーション 下側の色</param>
		/// <returns>描画済テクスチャ</returns>
		public Bitmap DrawPrivateFont(string drawstr, Color fontColor, Color gradationTopColor, Color gradataionBottomColor)
		{
			return DrawPrivateFont(drawstr, DrawMode.Gradation, fontColor, Color.White, gradationTopColor, gradataionBottomColor);
		}

		/// <summary>
		/// 文字列を描画したテクスチャを返す
		/// </summary>
		/// <param name="drawstr">描画文字列</param>
		/// <param name="fontColor">描画色</param>
		/// <param name="edgeColor">縁取色</param>
		/// <param name="gradationTopColor">グラデーション 上側の色</param>
		/// <param name="gradationBottomColor">グラデーション 下側の色</param>
		/// <returns>描画済テクスチャ</returns>
		public Bitmap DrawPrivateFont(string drawstr, Color fontColor, Color edgeColor, Color gradationTopColor, Color gradataionBottomColor)
		{
			return DrawPrivateFont(drawstr, DrawMode.Edge | DrawMode.Gradation, fontColor, edgeColor, gradationTopColor, gradataionBottomColor);
		}
		#endregion


		/// <summary>
		/// 文字列を描画したテクスチャを返す(メイン処理)
		/// </summary>
		/// <param name="rectDrawn">描画された領域</param>
		/// <param name="ptOrigin">描画文字列</param>
		/// <param name="drawstr">描画文字列</param>
		/// <param name="drawmode">描画モード</param>
		/// <param name="fontColor">描画色</param>
		/// <param name="edgeColor">縁取色</param>
		/// <param name="gradationTopColor">グラデーション 上側の色</param>
		/// <param name="gradationBottomColor">グラデーション 下側の色</param>
		/// <returns>描画済テクスチャ</returns>
		protected Bitmap DrawPrivateFont(string drawstr, DrawMode drawmode, Color fontColor, Color edgeColor, Color gradationTopColor, Color gradationBottomColor)
		{
			if (this._fontfamily == null || drawstr == null || drawstr == "" || drawstr == " " || drawstr == "　")
			{
				// nullを返すと、その後bmp→texture処理や、textureのサイズを見て__の処理で全部例外が発生することになる。
				// それは非常に面倒なので、最小限のbitmapを返してしまう。
				// まずはこの仕様で進めますが、問題有れば(上位側からエラー検出が必要であれば)例外を出したりエラー状態であるプロパティを定義するなり検討します。
				if (drawstr != "")
				{
					Trace.TraceWarning("DrawPrivateFont()の入力不正。最小値のbitmapを返します。");
				}
				_rectStrings = new Rectangle(0, 0, 0, 0);
				_ptOrigin = new Point(0, 0);
				return new Bitmap(1, 1);
			}

			// 描画サイズを測定する
			Size stringSize;
			using (Bitmap bmptmp = new Bitmap(1, 1))
			{
				using (Graphics gtmp = Graphics.FromImage(bmptmp))
				{
					using (
					StringFormat sf = new StringFormat()
					{
						LineAlignment = StringAlignment.Far, // 画面下部（垂直方向位置）
						Alignment = StringAlignment.Center,  // 画面中央（水平方向位置）     
						FormatFlags = StringFormatFlags.NoWrap, // どんなに長くて単語の区切りが良くても改行しない (AioiLight)
						Trimming = StringTrimming.None, // どんなに長くてもトリミングしない (AioiLight)
					})
					{
						//float to int
						SizeF fstringSize = gtmp.MeasureString(drawstr, this._font, new PointF(0, 0), sf);
						stringSize = new Size((int)fstringSize.Width, (int)fstringSize.Height);
						stringSize.Height = _font.Height;
						stringSize.Width += 10; //2015.04.01 kairera0467 ROTTERDAM NATIONの描画サイズがうまくいかんので。
					}
				}
			}

			bool bEdge = ((drawmode & DrawMode.Edge) == DrawMode.Edge);
			bool bGradation = ((drawmode & DrawMode.Gradation) == DrawMode.Gradation);

			// 縁取りの縁のサイズは、とりあえずフォントの大きさの(1/SkinConfig)とする
			int nEdgePt = (bEdge) ? (10 * _pt / TJAPlayer3.Skin.Font_Edge_Ratio) : 0; //SkinConfigにて設定可能に(rhimm)

			//取得した描画サイズを基に、描画先のbitmapを作成する
			Bitmap bmp = new Bitmap(stringSize.Width + nEdgePt * 2, stringSize.Height + nEdgePt * 2);
			bmp.MakeTransparent();

			using (Graphics g = Graphics.FromImage(bmp))
			{
				g.SmoothingMode = SmoothingMode.HighQuality;

				// レイアウト枠
				Rectangle r = new Rectangle(0, 0, stringSize.Width + nEdgePt * 2 + (TJAPlayer3.Skin.Text_Correction_XY[0] * stringSize.Width / 100), stringSize.Height + nEdgePt * 2 + (TJAPlayer3.Skin.Text_Correction_XY[0] * stringSize.Height / 100));

				if (bEdge)    // 縁取り有りの描画
				{
					using (StringFormat sf = new StringFormat()
					{
						LineAlignment = StringAlignment.Far, // 画面下部（垂直方向位置）
						Alignment = StringAlignment.Center,  // 画面中央（水平方向位置）     
						FormatFlags = StringFormatFlags.NoWrap, // どんなに長くて単語の区切りが良くても改行しない (AioiLight)
						Trimming = StringTrimming.None, // どんなに長くてもトリミングしない (AioiLight)
					})
					{
						// DrawPathで、ポイントサイズを使って描画するために、DPIを使って単位変換する
						// (これをしないと、単位が違うために、小さめに描画されてしまう)
						float sizeInPixels = _font.SizeInPoints * g.DpiY / 72;  // 1 inch = 72 points

						GraphicsPath gp = new GraphicsPath();
						gp.AddString(drawstr, this._fontfamily, (int)this._font.Style, sizeInPixels, r, sf);

						// 縁取りを描画する
						Pen p = new Pen(edgeColor, nEdgePt);
						p.LineJoin = LineJoin.Round;
						g.DrawPath(p, gp);

						// 塗りつぶす
						Brush br;
						if (bGradation)
						{
							br = new LinearGradientBrush(r, gradationTopColor, gradationBottomColor, LinearGradientMode.Vertical);
						}
						else
						{
							br = new SolidBrush(fontColor);
						}
						g.FillPath(br, gp);

						if (br != null) br.Dispose(); br = null;
						if (p != null) p.Dispose(); p = null;
						if (gp != null) gp.Dispose(); gp = null;
					}

				}
				else
				{
					// 縁取りなしの描画
					g.DrawString(drawstr, _font, new SolidBrush(fontColor), new PointF(0, 0));
				}
				_rectStrings = new Rectangle(0, 0, stringSize.Width, stringSize.Height);
				_ptOrigin = new Point(nEdgePt * 2, nEdgePt * 2);
			}

			return bmp;
		}

		/// <summary>
		/// 文字列を描画したテクスチャを返す(メイン処理)
		/// </summary>
		/// <param name="rectDrawn">描画された領域</param>
		/// <param name="ptOrigin">描画文字列</param>
		/// <param name="drawstr">描画文字列</param>
		/// <param name="drawmode">描画モード</param>
		/// <param name="fontColor">描画色</param>
		/// <param name="edgeColor">縁取色</param>
		/// <param name="gradationTopColor">グラデーション 上側の色</param>
		/// <param name="gradationBottomColor">グラデーション 下側の色</param>
		/// <returns>描画済テクスチャ</returns>
		protected Bitmap DrawPrivateFont_V(string drawstr, Color fontColor, Color edgeColor, bool bVertical)
		{
			if (this._fontfamily == null || drawstr == null || drawstr == "" || drawstr == " " || drawstr == "　")
			{
				// nullを返すと、その後bmp→texture処理や、textureのサイズを見て__の処理で全部例外が発生することになる。
				// それは非常に面倒なので、最小限のbitmapを返してしまう。
				// まずはこの仕様で進めますが、問題有れば(上位側からエラー検出が必要であれば)例外を出したりエラー状態であるプロパティを定義するなり検討します。
				if (drawstr != "")
				{
					Trace.TraceWarning("DrawPrivateFont()の入力不正。最小値のbitmapを返します。");
				}
				_rectStrings = new Rectangle(0, 0, 0, 0);
				_ptOrigin = new Point(0, 0);
				return new Bitmap(1, 1);
			}

			if (this._font == null)
			{
				this._font = new Font(TJAPlayer3.ConfigIni.FontName, 28);//this._font==nullの例外が発生したので追記(Mr-Ojii)
			}

			drawstr = drawstr.Replace("・", "．");

			string[] strName = new string[drawstr.Length];
			for (int i = 0; i < drawstr.Length; i++) strName[i] = drawstr.Substring(i, 1);

			#region[ キャンバスの大きさ予測 ]
			//大きさを計算していく。
			int nHeight = 0;
			for (int i = 0; i < strName.Length; i++)
			{
				Size strSize;
				using (Bitmap bmptmp = new Bitmap(1, 1))
				{
					using (Graphics gtmp = Graphics.FromImage(bmptmp))
					{
						using (
						StringFormat sf = new StringFormat()
						{
							LineAlignment = StringAlignment.Far, // 画面下部（垂直方向位置）
							Alignment = StringAlignment.Center,  // 画面中央（水平方向位置）     
							FormatFlags = StringFormatFlags.NoWrap, // どんなに長くて単語の区切りが良くても改行しない (AioiLight)
							Trimming = StringTrimming.None, // どんなに長くてもトリミングしない (AioiLight)
						})
						{
							//float to int
							SizeF fstringSize = gtmp.MeasureString(strName[i], this._font, new PointF(0, 0), sf);
							strSize = new Size((int)fstringSize.Width, (int)fstringSize.Height);
						}
					}
				}
				strSize.Height = _font.Height;

				//stringformatは最初にやっていてもいいだろう。
				using (StringFormat sFormat = new StringFormat()
				{
					LineAlignment = StringAlignment.Center, // 画面下部（垂直方向位置）
					Alignment = StringAlignment.Center, // 画面中央（水平方向位置）
				})
				{

					//できるだけ正確な値を計算しておきたい...!
					using (Bitmap bmpDummy = new Bitmap(150, 150))//とりあえず150
					{
						using (Graphics gCal = Graphics.FromImage(bmpDummy))
						{

							Rectangle rect正確なサイズ = this.MeasureStringPrecisely(gCal, strName[i], this._font, strSize, sFormat);
							int n余白サイズ = strSize.Height - rect正確なサイズ.Height;

							//Rectangle rect = new Rectangle( 0, -n余白サイズ + 2, 46, ( strSize.Height + 16 )); 2020.05.04 Mr-Ojii 使ってないから、コメント化。

							if (strName[i] == "ー" || strName[i] == "-" || strName[i] == "～" || strName[i] == "<" || strName[i] == ">" || strName[i] == "(" || strName[i] == ")" || strName[i] == "「" || strName[i] == "」" || strName[i] == "[" || strName[i] == "]")
							{
								nHeight += (rect正確なサイズ.Width) + 4;
							}
							else if (strName[i] == "_")
							{ nHeight += (rect正確なサイズ.Height) + 6; }
							else if (strName[i] == " ")
							{ nHeight += (12); }
							else { nHeight += (rect正確なサイズ.Height) + 10; }
						}
					}
				}
			}
			#endregion

			Bitmap bmpCambus = new Bitmap(46, nHeight);
			using (Graphics Gcambus = Graphics.FromImage(bmpCambus))
			{
				//キャンバス作成→1文字ずつ作成してキャンバスに描画という形がよさそうかな?
				int nNowPos = 0;
				int nAdded = 0;
				int nEdge補正X = 0;
				int nEdge補正Y = 0;
				if (this._pt < 18)
					nAdded -= 2;

				for (int i = 0; i < strName.Length; i++)
				{
					Size strSize;
					using (Bitmap bmptmp = new Bitmap(1, 1))
					{
						using (Graphics gtmp = Graphics.FromImage(bmptmp))
						{
							using (
							StringFormat sf = new StringFormat()
							{
								LineAlignment = StringAlignment.Far, // 画面下部（垂直方向位置）
								Alignment = StringAlignment.Center,  // 画面中央（水平方向位置）     
								FormatFlags = StringFormatFlags.NoWrap, // どんなに長くて単語の区切りが良くても改行しない (AioiLight)
								Trimming = StringTrimming.None, // どんなに長くてもトリミングしない (AioiLight)
							})
							{
								//float to int
								SizeF fstringSize = gtmp.MeasureString(strName[i], this._font, new PointF(0, 0), sf);
								strSize = new Size((int)fstringSize.Width, (int)fstringSize.Height);
							}
						}
					}
					strSize.Height = _font.Height;

					//stringformatは最初にやっていてもいいだろう。
					StringFormat sFormat = new StringFormat()
					{
						LineAlignment = StringAlignment.Center, // 画面下部（垂直方向位置）
						Alignment = StringAlignment.Near,   // 画面中央（水平方向位置）
					};

					//できるだけ正確な値を計算しておきたい...!
					Graphics gCal = Graphics.FromImage(new Bitmap(150, 150));//とりあえず150 2020.05.04　Mr-Ojii 一回変数に格納する必要がないと判断したため、まとめた。
					Rectangle rect正確なサイズ = this.MeasureStringPrecisely(gCal, strName[i], this._font, strSize, sFormat);
					int n余白サイズ = strSize.Height - rect正確なサイズ.Height;

					Bitmap bmpV = new Bitmap((rect正確なサイズ.Width + 12) + nAdded, (rect正確なサイズ.Height) + 12);
					bmpV.MakeTransparent();

					Graphics gV = Graphics.FromImage(bmpV);
					gV.SmoothingMode = SmoothingMode.HighQuality;

					if (TJAPlayer3.Skin.SongSelect_CorrectionX_Chara != null && TJAPlayer3.Skin.SongSelect_CorrectionX_Chara_Value != null)
					{
						int Xindex = Array.IndexOf(TJAPlayer3.Skin.SongSelect_CorrectionX_Chara, strName[i]);
						if (-1 < Xindex && Xindex < TJAPlayer3.Skin.SongSelect_CorrectionX_Chara_Value.Length && strName[i].In(TJAPlayer3.Skin.SongSelect_CorrectionX_Chara))
						{
							nEdge補正X = TJAPlayer3.Skin.SongSelect_CorrectionX_Chara_Value[Xindex];
						}
						else
						{
							if (-1 < Xindex && TJAPlayer3.Skin.SongSelect_CorrectionX_Chara_Value.Length <= Xindex && strName[i].In(TJAPlayer3.Skin.SongSelect_CorrectionX_Chara))
							{
								nEdge補正X = TJAPlayer3.Skin.SongSelect_CorrectionX_Chara_Value[0];
							}
							else
							{
								nEdge補正X = 0;
							}
						}
					}

					if (TJAPlayer3.Skin.SongSelect_CorrectionY_Chara != null && TJAPlayer3.Skin.SongSelect_CorrectionY_Chara_Value != null)
					{
						int Yindex = Array.IndexOf(TJAPlayer3.Skin.SongSelect_CorrectionY_Chara, strName[i]);
						if (-1 < Yindex && Yindex < TJAPlayer3.Skin.SongSelect_CorrectionY_Chara_Value.Length && strName[i].In(TJAPlayer3.Skin.SongSelect_CorrectionY_Chara))
						{
							nEdge補正Y = TJAPlayer3.Skin.SongSelect_CorrectionY_Chara_Value[Yindex];
						}
						else
						{
							if (-1 < Yindex && TJAPlayer3.Skin.SongSelect_CorrectionY_Chara_Value.Length <= Yindex && strName[i].In(TJAPlayer3.Skin.SongSelect_CorrectionY_Chara))
							{
								nEdge補正Y = TJAPlayer3.Skin.SongSelect_CorrectionY_Chara_Value[0];
							}
							else
							{
								nEdge補正Y = 0;
							}
						}
					}

					//X座標、Y座標それぞれについて、SkinConfig内でズレを直したい文字を , で区切って列挙して、
					//補正値を記入することで、特定のそれらの文字について一括で座標をずらす。
					//現時点では補正値をX,Y各座標について1個ずつしか取れない（複数対1）ので、
					//文字を列挙して、同じ数だけそれぞれの文字の補正値を記入できるような枠組をつくりたい。（20181205 rhimm）←実装済み //2020.05.04 Mr-Ojii 文字ごとに補正をかけられるように。「,」区切りで書けるように。

					Rectangle rect = new Rectangle(-3 - nAdded + (nEdge補正X * _pt / 100), -rect正確なサイズ.Y - 2 + (nEdge補正Y * _pt / 100), (strSize.Width + 12), (strSize.Height + 11));
					//Rectangle rect = new Rectangle( 0, -rect正確なサイズ.Y - 2, 36, rect正確なサイズ.Height + 10);

					// DrawPathで、ポイントサイズを使って描画するために、DPIを使って単位変換する
					// (これをしないと、単位が違うために、小さめに描画されてしまう)
					float sizeInPixels = _font.SizeInPoints * gV.DpiY / 72f;  // 1 inch = 72 points

					GraphicsPath gpV = new GraphicsPath();
					gpV.AddString(strName[i], this._fontfamily, (int)this._font.Style, sizeInPixels, rect, sFormat);

					// 縁取りを描画する
					//int nEdgePt = (_pt / 3); // 縁取りをフォントサイズ基準に変更
					float nEdgePt = (10f * _pt / TJAPlayer3.Skin.Font_Edge_Ratio_Vertical); // SkinConfigにて設定可能に(rhimm)
					Pen pV = new Pen(edgeColor, nEdgePt);
					pV.LineJoin = LineJoin.Round;
					gV.DrawPath(pV, gpV);

					// 塗りつぶす
					Brush brV = new SolidBrush(fontColor);

					gV.FillPath(brV, gpV);

					if (brV != null) brV.Dispose();
					if (pV != null) pV.Dispose();
					if (gpV != null) gpV.Dispose();
					if (gV != null) gV.Dispose();

					int n補正 = 0;
					int nY補正 = 0;

					if (strName[i] == "ー" || strName[i] == "-" || strName[i] == "～")
					{
						bmpV.RotateFlip(RotateFlipType.Rotate90FlipNone);
						n補正 = 2;
						if (this._pt < 20)
							n補正 = 0;
						//nNowPos = nNowPos - 2;
					}
					else if (strName[i] == "<" || strName[i] == ">" || strName[i] == "(" || strName[i] == ")" || strName[i] == "[" || strName[i] == "]" || strName[i] == "」" || strName[i] == "）" || strName[i] == "』")
					{
						bmpV.RotateFlip(RotateFlipType.Rotate90FlipNone);
						n補正 = 2;
						if (this._pt < 20)
						{
							n補正 = 0;
							//nNowPos = nNowPos - 4;
						}
					}
					else if (strName[i] == "「" || strName[i] == "（" || strName[i] == "『")
					{
						bmpV.RotateFlip(RotateFlipType.Rotate90FlipNone);
						n補正 = 2;
						if (this._pt < 20)
						{
							n補正 = 2;
							//nNowPos = nNowPos;
						}
					}
					else if (strName[i] == "・")
					{
						n補正 = -8;
						if (this._pt < 20)
						{
							n補正 = -8;
							//nNowPos = nNowPos;
						}
					}
					else if (strName[i] == ".")
					{
						n補正 = 8;
						if (this._pt < 20)
						{
							n補正 = 8;
							//nNowPos = nNowPos;
						}
					}
					else if (strName[i].In(TJAPlayer3.Skin.SongSelect_Rotate_Chara))
					{
						bmpV.RotateFlip(RotateFlipType.Rotate90FlipNone);
					}
					//個別の文字に関して、カンマで区切ってSkinConfigに記入したものを回転させる(20181205 rhimm)
					else if (strName[i] == " ")
						nNowPos += 10;

					if (i == 0)
					{
						nNowPos = 4;
					}
					Gcambus.DrawImage(bmpV, (bmpCambus.Width / 2) - (bmpV.Width / 2) + n補正, nNowPos + nY補正);
					nNowPos += bmpV.Size.Height - 6;

					if (bmpV != null) bmpV.Dispose();
					if (gCal != null) gCal.Dispose();

					_rectStrings = new Rectangle(0, 0, strSize.Width, strSize.Height);
					_ptOrigin = new Point(6 * 2, 6 * 2);
				}
			}

			return bmpCambus;
		}

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
		public Rectangle MeasureStringPrecisely(Graphics g,
			string text, Font font, Size proposedSize, StringFormat stringFormat)
		{
			//解像度を引き継いで、Bitmapを作成する
			Bitmap bmp = new Bitmap(proposedSize.Width, proposedSize.Height, g);
			//BitmapのGraphicsを作成する
			Graphics bmpGraphics = Graphics.FromImage(bmp);
			//Graphicsのプロパティを引き継ぐ
			bmpGraphics.TextRenderingHint = g.TextRenderingHint;
			bmpGraphics.TextContrast = g.TextContrast;
			bmpGraphics.PixelOffsetMode = g.PixelOffsetMode;
			//文字列の描かれていない部分の色を取得する
			Color backColor = bmp.GetPixel(0, 0);
			//実際にBitmapに文字列を描画する

			//  Debug.Print("Font=" + font.ToString());
			bmpGraphics.DrawString(text, font, Brushes.Black,
				new RectangleF(0f, 0f, proposedSize.Width, proposedSize.Height),
				stringFormat);

			bmpGraphics.Dispose();
			//文字列が描画されている範囲を計測する
			Rectangle resultRect = MeasureForegroundArea(bmp, backColor);
			bmp.Dispose();

			return resultRect;
		}

		/// <summary>
		/// 指定されたBitmapで、backColor以外の色が使われている範囲を計測する
		/// </summary>
		private Rectangle MeasureForegroundArea(Bitmap bmp, Color backColor)
		{
			int backColorArgb = backColor.ToArgb();
			int maxWidth = bmp.Width;
			int maxHeight = bmp.Height;

			//左側の空白部分を計測する
			int leftPosition = -1;
			for (int x = 0; x < maxWidth; x++)
			{
				for (int y = 0; y < maxHeight; y++)
				{
					//違う色を見つけたときは、位置を決定する
					if (bmp.GetPixel(x, y).ToArgb() != backColorArgb)
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
					if (bmp.GetPixel(x, y).ToArgb() != backColorArgb)
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
					if (bmp.GetPixel(x, y).ToArgb() != backColorArgb)
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
					if (bmp.GetPixel(x, y).ToArgb() != backColorArgb)
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


		//------------------------------------------------

		/// <summary>
		/// 最後にDrawPrivateFont()した文字列の描画領域を取得します。
		/// </summary>
		public Rectangle RectStrings
		{
			get
			{
				return _rectStrings;
			}
			protected set
			{
				_rectStrings = value;
			}
		}
		public Point PtOrigin
		{
			get
			{
				return _ptOrigin;
			}
			protected set
			{
				_ptOrigin = value;
			}
		}

		#region [ IDisposable 実装 ]
		//-----------------
		public void Dispose()
		{
			if (!this.bDispose完了済み)
			{
				if (this._font != null)
				{
					this._font.Dispose();
					this._font = null;
				}
				if (this._pfc != null)
				{
					this._pfc.Dispose();
					this._pfc = null;
				}

				this.bDispose完了済み = true;
			}
		}

		//-----------------
		#endregion
		#region [ private ]
		//-----------------
		protected bool bDispose完了済み;
		protected Font _font;

		private System.Drawing.Text.PrivateFontCollection _pfc;
		private FontFamily _fontfamily;
		private int _pt;
		private Rectangle _rectStrings;
		private Point _ptOrigin;
		//-----------------
		#endregion
	}
}