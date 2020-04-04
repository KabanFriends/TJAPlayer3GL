using System;
using System.IO;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Diagnostics;
using FDK.ExtensionMethods;
using TJAPlayer3.Common;

namespace TJAPlayer3
{
    public class CPrivateFont : IDisposable
	{
		#region [ コンストラクタ ]
		public CPrivateFont( FontFamily fontfamily, int pt )
		{
			Initialize( null, fontfamily, pt, FontStyle.Regular );
		}

		protected CPrivateFont()
		{
			//throw new ArgumentException("CPrivateFont: 引数があるコンストラクタを使用してください。");
		}
		#endregion

		protected void Initialize( string fontpath, FontFamily fontfamily, int pt, FontStyle style )
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
					this._pfc = new System.Drawing.Text.PrivateFontCollection();	//PrivateFontCollectionオブジェクトを作成する
					this._pfc.AddFontFile(fontpath);								//PrivateFontCollectionにフォントを追加する
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
					style = FontStyle.Regular | FontStyle.Bold | FontStyle.Italic | FontStyle.Underline | FontStyle.Strikeout;	// null非許容型なので、代わりに全盛をNGワードに設定
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
				this._font = new Font(this._fontfamily, emSize, style, GraphicsUnit.Pixel);	//PrivateFontCollectionの先頭のフォントのFontオブジェクトを作成する
				//HighDPI対応のため、pxサイズで指定
			}
			else
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
		/// <param name="edgeColor">縁取色</param>
		/// <returns>描画済テクスチャ</returns>
		public Bitmap DrawPrivateFont( string drawstr, Color fontColor, Color edgeColor )
		{
			return DrawPrivateFont( drawstr, DrawMode.Edge, fontColor, edgeColor, Color.White, Color.White );
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
		protected Bitmap DrawPrivateFont( string drawstr, DrawMode drawmode, Color fontColor, Color edgeColor, Color gradationTopColor, Color gradationBottomColor )
		{
			if ( this._fontfamily == null || _font == null || drawstr == null || drawstr == "" )
			{
				// nullを返すと、その後bmp→texture処理や、textureのサイズを見て__の処理で全部例外が発生することになる。
				// それは非常に面倒なので、最小限のbitmapを返してしまう。
				// まずはこの仕様で進めますが、問題有れば(上位側からエラー検出が必要であれば)例外を出したりエラー状態であるプロパティを定義するなり検討します。
				if ( drawstr != "" )
				{
					Trace.TraceWarning( "DrawPrivateFont()の入力不正。最小値のbitmapを返します。" );
				}
				_rectStrings = new Rectangle( 0, 0, 0, 0 );
				_ptOrigin = new Point( 0, 0 );
				return new Bitmap(1, 1);
			}
			bool bEdge =      ( ( drawmode & DrawMode.Edge      ) == DrawMode.Edge );
			bool bGradation = ( ( drawmode & DrawMode.Gradation ) == DrawMode.Gradation );

            // 縁取りの縁のサイズは、とりあえずフォントの大きさの1/4とする
            //int nEdgePt = (bEdge)? _pt / 4 : 0;
            //int nEdgePt = (bEdge) ? (_pt / 3) : 0; // 縁取りが少なすぎるという意見が多かったため変更。 (AioiLight)
            int nEdgePt = (bEdge) ? (10 * _pt / TJAPlayer3.Skin.Font_Edge_Ratio) : 0; //SkinConfigにて設定可能に(rhimm)

            // 描画サイズを測定する
            Size stringSize = System.Windows.Forms.TextRenderer.MeasureText( drawstr, this._font, new Size( int.MaxValue, int.MaxValue ),
				System.Windows.Forms.TextFormatFlags.NoPrefix |
				System.Windows.Forms.TextFormatFlags.NoPadding
			);
            stringSize.Width += 10; //2015.04.01 kairera0467 ROTTERDAM NATIONの描画サイズがうまくいかんので。

			//取得した描画サイズを基に、描画先のbitmapを作成する
			Bitmap bmp = new Bitmap( stringSize.Width + nEdgePt * 2, stringSize.Height + nEdgePt * 2 );
			bmp.MakeTransparent();
			Graphics g = Graphics.FromImage( bmp );
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;

			StringFormat sf = new StringFormat();
			sf.LineAlignment = StringAlignment.Far;	// 画面下部（垂直方向位置）
			sf.Alignment = StringAlignment.Center;	// 画面中央（水平方向位置）     
            sf.FormatFlags = StringFormatFlags.NoWrap; // どんなに長くて単語の区切りが良くても改行しない (AioiLight)
            sf.Trimming = StringTrimming.None; // どんなに長くてもトリミングしない (AioiLight)
			// レイアウト枠
			Rectangle r = new Rectangle( 0, 0, stringSize.Width + nEdgePt * 2 + (TJAPlayer3.Skin.Text_Correction_XY[0] * stringSize.Width / 100), stringSize.Height + nEdgePt * 2 + (TJAPlayer3.Skin.Text_Correction_XY[1] * stringSize.Height / 100));

			if ( bEdge )	// 縁取り有りの描画
			{
				// DrawPathで、ポイントサイズを使って描画するために、DPIを使って単位変換する
				// (これをしないと、単位が違うために、小さめに描画されてしまう)
				float sizeInPixels = _font.SizeInPoints * g.DpiY / 72;  // 1 inch = 72 points

				System.Drawing.Drawing2D.GraphicsPath gp = new System.Drawing.Drawing2D.GraphicsPath();
				gp.AddString( drawstr, this._fontfamily, (int) this._font.Style, sizeInPixels, r, sf );

				// 縁取りを描画する
				Pen p = new Pen( edgeColor, nEdgePt );
				p.LineJoin = System.Drawing.Drawing2D.LineJoin.Round;
				g.DrawPath( p, gp );

				// 塗りつぶす
				Brush br;
				if ( bGradation )
				{
					br = new LinearGradientBrush( r, gradationTopColor, gradationBottomColor, LinearGradientMode.Vertical );
				}
				else
				{
					br = new SolidBrush( fontColor );
				}
				g.FillPath( br, gp );

				if ( br != null ) br.Dispose(); br = null;
				if ( p != null ) p.Dispose(); p = null;
				if ( gp != null ) gp.Dispose(); gp = null;
			}
			else
			{
				// 縁取りなしの描画
				System.Windows.Forms.TextRenderer.DrawText( g, drawstr, _font, new Point( 0, 0 ), fontColor );
			}
#if debug表示
			g.DrawRectangle( new Pen( Color.White, 1 ), new Rectangle( 1, 1, stringSize.Width-1, stringSize.Height-1 ) );
			g.DrawRectangle( new Pen( Color.Green, 1 ), new Rectangle( 0, 0, bmp.Width - 1, bmp.Height - 1 ) );
#endif
			_rectStrings = new Rectangle( 0, 0, stringSize.Width, stringSize.Height );
			_ptOrigin = new Point( nEdgePt * 2, nEdgePt * 2 );
			

			#region [ リソースを解放する ]
			if ( sf != null )	sf.Dispose();	sf = null;
			if ( g != null )	g.Dispose();	g = null;
			#endregion

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
		protected Bitmap DrawPrivateFont_V( string drawstr, Color fontColor, Color edgeColor, bool bVertical )
		{
			if ( this._fontfamily == null || _font == null || drawstr == null || drawstr == "" )
			{
				// nullを返すと、その後bmp→texture処理や、textureのサイズを見て__の処理で全部例外が発生することになる。
				// それは非常に面倒なので、最小限のbitmapを返してしまう。
				// まずはこの仕様で進めますが、問題有れば(上位側からエラー検出が必要であれば)例外を出したりエラー状態であるプロパティを定義するなり検討します。
				if ( drawstr != "" )
				{
					Trace.TraceWarning( "DrawPrivateFont()の入力不正。最小値のbitmapを返します。" );
				}
				_rectStrings = new Rectangle( 0, 0, 0, 0 );
				_ptOrigin = new Point( 0, 0 );
				return new Bitmap(1, 1);
			}

            //StreamWriter stream = stream = new StreamWriter("Test.txt", false);

            //try
            //{
            //    stream = new StreamWriter("Test.txt", false);
            //}
            //catch (Exception ex)
            //{
            //    stream.Close();
            //    stream = new StreamWriter("Test.txt", false);
            //}

            string[] strName =  new string[ drawstr.Length ];
            for( int i = 0; i < drawstr.Length; i++ ) strName[i] = drawstr.Substring(i, 1);

            #region[ キャンバスの大きさ予測 ]
            //大きさを計算していく。
		    Bitmap bmpDummy = new Bitmap( 1, 1 );
		    Graphics gCal = Graphics.FromImage( bmpDummy );
            int nHeight = 0;
            for( int i = 0; i < strName.Length; i++ )
            {
                Size strSize = System.Windows.Forms.TextRenderer.MeasureText( strName[ i ], this._font, new Size( int.MaxValue, int.MaxValue ),
				System.Windows.Forms.TextFormatFlags.NoPrefix |
				System.Windows.Forms.TextFormatFlags.NoPadding );

                //stringformatは最初にやっていてもいいだろう。
			    StringFormat sFormat = new StringFormat();
			    sFormat.LineAlignment = StringAlignment.Center;	// 画面下部（垂直方向位置）
			    sFormat.Alignment = StringAlignment.Center;	// 画面中央（水平方向位置）


                //できるだけ正確な値を計算しておきたい...!
                Rectangle rect正確なサイズ = CPreciseStringMeasurer.MeasureStringPrecisely( gCal, strName[ i ], this._font, strSize, sFormat );
                int n余白サイズ = strSize.Height - rect正確なサイズ.Height;

                Rectangle rect = new Rectangle( 0, -n余白サイズ + 2, 46, ( strSize.Height + 16 ));

                if( strName[ i ] == "ー" || strName[ i ] == "-" || strName[ i ] == "～" || strName[ i ] == "<" || strName[ i ] == ">" || strName[ i ] == "(" || strName[ i ] == ")" || strName[ i ] == "「" || strName[ i ] == "」" || strName[ i ] == "[" || strName[ i ] == "]" )
                {
                    nHeight += ( rect正確なサイズ.Width ) + 4;
                }
                else if( strName[ i ] == "_" ){ nHeight += ( rect正確なサイズ.Height ) + 6;  }
                else if( strName[ i ] == " " )
                { nHeight += ( 12 ); }
                else { nHeight += ( rect正確なサイズ.Height ) + 10; }

                //stream.WriteLine( "文字の大きさ{0},大きさ合計{1}", ( rect正確なサイズ.Height ) + 6, nHeight );
                
            }
            #endregion

            Bitmap bmpCambus = new Bitmap( 46, nHeight );
            Graphics Gcambus = Graphics.FromImage( bmpCambus );

            //キャンバス作成→1文字ずつ作成してキャンバスに描画という形がよさそうかな?
            int nNowPos = 0;
            int nAdded = 0;
            int nEdge補正X = 0;
            int nEdge補正Y = 0;
            if (this._pt < 18)
                nAdded = nAdded - 2;

            for (int i = 0; i < strName.Length; i++)
            {
                Size strSize = System.Windows.Forms.TextRenderer.MeasureText(strName[i], this._font, new Size(int.MaxValue, int.MaxValue),
                System.Windows.Forms.TextFormatFlags.NoPrefix |
                System.Windows.Forms.TextFormatFlags.NoPadding);

                //stringformatは最初にやっていてもいいだろう。
                StringFormat sFormat = new StringFormat();
                sFormat.LineAlignment = StringAlignment.Center; // 画面下部（垂直方向位置）
                sFormat.Alignment = StringAlignment.Near;	// 画面中央（水平方向位置）

                //できるだけ正確な値を計算しておきたい...!
                Rectangle rect正確なサイズ = CPreciseStringMeasurer.MeasureStringPrecisely(gCal, strName[i], this._font, strSize, sFormat);
                int n余白サイズ = strSize.Height - rect正確なサイズ.Height;

                //Bitmap bmpV = new Bitmap( 36, ( strSize.Height + 12 ) - 6 );

                Bitmap bmpV = new Bitmap((rect正確なサイズ.Width + 12) + nAdded, (rect正確なサイズ.Height) + 12);

                bmpV.MakeTransparent();
                Graphics gV = Graphics.FromImage(bmpV);
                gV.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;

                //SongSelect_Correction*_Charaの何番目に当該の文字があるかを取得(20181222 rhimm)
                int IndexX = Array.IndexOf(TJAPlayer3.Skin.SongSelect_CorrectionX_Chara, strName[i]);
                int IndexY = Array.IndexOf(TJAPlayer3.Skin.SongSelect_CorrectionY_Chara, strName[i]);

                //取得した*_Charaの配列上の位置にある*_Chara_Valueの値で補正
                //補正文字の数に比べて補正値の数が足りない時、配列の一番最後の補正値で足りない分の文字を補正
                //例えば　補正文字あ,い,う,え,おに対して補正値が10,13,15の3つだった時、
                //あ　は補正値10、い　は補正値13、　う,え,お　は補正値15　となるようにする(20181222 rhimm)

                if (-1 < IndexX && IndexX < TJAPlayer3.Skin.SongSelect_CorrectionX_Chara_Value.Length && strName[i].In(TJAPlayer3.Skin.SongSelect_CorrectionX_Chara))
                {
                    nEdge補正X = TJAPlayer3.Skin.SongSelect_CorrectionX_Chara_Value[IndexX];
                }
                else if(-1 < IndexX && TJAPlayer3.Skin.SongSelect_CorrectionX_Chara_Value.Length <= IndexX && strName[i].In(TJAPlayer3.Skin.SongSelect_CorrectionX_Chara))
                {
                    nEdge補正X = TJAPlayer3.Skin.SongSelect_CorrectionX_Chara_Value[TJAPlayer3.Skin.SongSelect_CorrectionX_Chara_Value.Length - 1];
                }
                else
                {
                    nEdge補正X = 0;
                }

                if (-1 < IndexY && IndexY < TJAPlayer3.Skin.SongSelect_CorrectionY_Chara_Value.Length && strName[i].In(TJAPlayer3.Skin.SongSelect_CorrectionY_Chara))
                {
                    nEdge補正Y = TJAPlayer3.Skin.SongSelect_CorrectionY_Chara_Value[IndexY];
                }
                else if (-1 < IndexY && TJAPlayer3.Skin.SongSelect_CorrectionY_Chara_Value.Length <= IndexY && strName[i].In(TJAPlayer3.Skin.SongSelect_CorrectionY_Chara))
                {
                    nEdge補正Y = TJAPlayer3.Skin.SongSelect_CorrectionY_Chara_Value[TJAPlayer3.Skin.SongSelect_CorrectionY_Chara_Value.Length - 1];
                }
                else
                {
                    nEdge補正Y = 0;
                }
                //X座標、Y座標それぞれについて、SkinConfig内でズレを直したい文字を , で区切って列挙して、
                //補正値を記入することで、特定のそれらの文字について一括で座標をずらす。
                //現時点では補正値をX,Y各座標について1個ずつしか取れない（複数対1）ので、
                //文字を列挙して、同じ数だけそれぞれの文字の補正値を記入できるような枠組をつくりたい。（20181205 rhimm）

                Rectangle rect = new Rectangle(-3 - nAdded + (nEdge補正X * _pt / 100), -rect正確なサイズ.Y - 2 + (nEdge補正Y * _pt / 100), (strSize.Width + 12), (strSize.Height + 12));
                //Rectangle rect = new Rectangle( 0, -rect正確なサイズ.Y - 2, 36, rect正確なサイズ.Height + 10);

                // DrawPathで、ポイントサイズを使って描画するために、DPIを使って単位変換する
                // (これをしないと、単位が違うために、小さめに描画されてしまう)
                float sizeInPixels = _font.SizeInPoints * gV.DpiY / 72;  // 1 inch = 72 points

                System.Drawing.Drawing2D.GraphicsPath gpV = new System.Drawing.Drawing2D.GraphicsPath();
                gpV.AddString(strName[i], this._fontfamily, (int)this._font.Style, sizeInPixels, rect, sFormat);

                // 縁取りを描画する
                //int nEdgePt = (_pt / 3); // 縁取りをフォントサイズ基準に変更
                int nEdgePt = (10 * _pt / TJAPlayer3.Skin.Font_Edge_Ratio_Vertical); // SkinConfigにて設定可能に(rhimm)
                Pen pV = new Pen(edgeColor, nEdgePt);
                pV.LineJoin = System.Drawing.Drawing2D.LineJoin.Round;
                gV.DrawPath(pV, gpV);

                // 塗りつぶす
                Brush brV;
                {
                    brV = new SolidBrush(fontColor);
                }
                gV.FillPath(brV, gpV);

                if (brV != null) brV.Dispose(); brV = null;
                if (pV != null) pV.Dispose(); pV = null;
                if (gpV != null) gpV.Dispose(); gpV = null;
                if (gV != null) gV.Dispose(); gV = null;

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


                //else if( strName[ i ] == "_" )
                //    nNowPos = nNowPos + 20;
                else if( strName[ i ] == " " )
                    nNowPos = nNowPos + 10;


                //bmpV.Save( "String" + i.ToString() + ".png" );


                if( i == 0 )
                {
                    nNowPos = 4;
                }
                Gcambus.DrawImage( bmpV, (bmpCambus.Width / 2) - (bmpV.Width / 2) + n補正, nNowPos + nY補正 );
                nNowPos += bmpV.Size.Height - 6;

                if( bmpV != null ) bmpV.Dispose(); bmpV = null;

                //bmpCambus.Save( "test.png" );
                //if( this._pt < 20 )
                //    bmpCambus.Save( "test_S.png" );

			    _rectStrings = new Rectangle( 0, 0, strSize.Width, strSize.Height );
			    _ptOrigin = new Point( 6 * 2, 6 * 2 );


                //stream.WriteLine( "黒無しサイズ{0},余白{1},黒あり予測サイズ{2},ポ↑ジ↓{3}",rect正確なサイズ.Height, n余白サイズ, rect正確なサイズ.Height + 8, nNowPos );
                
            }
            //stream.Close();

            if( Gcambus != null ) Gcambus.Dispose();

		    //念のため解放
		    bmpDummy.Dispose();
		    gCal.Dispose();

			//return bmp;
            return bmpCambus;
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
