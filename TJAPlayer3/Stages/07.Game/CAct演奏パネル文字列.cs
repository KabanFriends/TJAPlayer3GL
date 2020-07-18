using System;
using System.Drawing;
using System.Diagnostics;
using FDK;
using TJAPlayer3.Common;

namespace TJAPlayer3
{
	internal class CAct演奏パネル文字列 : CActivity
	{

		// コンストラクタ

		public CAct演奏パネル文字列()
		{
			base.b活性化してない = true;
		}


        // メソッド

        /// <summary>
        /// 右上の曲名、曲数表示の更新を行います。
        /// </summary>
        /// <param name="songName">曲名</param>
        /// <param name="genreName">ジャンル名</param>
        /// <param name="stageText">曲数</param>
        public void SetPanelString(string songName, string genreName, string stageText = null)
		{
			if( base.b活性化してる )
			{
				TJAPlayer3.t安全にDisposeする(ref this.txPanel);
				if( (songName != null ) && (songName.Length > 0 ) )
				{
					try
					{
					    using (var bmpSongTitle = pfMusicName.DrawPrivateFont(songName, TJAPlayer3.Skin.Game_MusicName_ForeColor, TJAPlayer3.Skin.Game_MusicName_BackColor))
					    {
					        this.txMusicName = TJAPlayer3.tテクスチャの生成( bmpSongTitle, false );
					    }
                        if (txMusicName != null)
                        {
                            this.txMusicName.vc拡大縮小倍率.X = TJAPlayer3.GetSongNameXScaling(ref txMusicName);
                        }
                    
                        Bitmap bmpDiff;
                        string strDiff = "";
                        if (TJAPlayer3.Skin.eDiffDispMode == E難易度表示タイプ.n曲目に表示)
                        {
                            switch (TJAPlayer3.stage選曲.n確定された曲の難易度)
                            {
                                case 0:
                                    strDiff = "かんたん ";
                                    break;
                                case 1:
                                    strDiff = "ふつう ";
                                    break;
                                case 2:
                                    strDiff = "むずかしい ";
                                    break;
                                case 3:
                                    strDiff = "おに ";
                                    break;
                                case 4:
                                    strDiff = "えでぃと ";
                                    break;
                                default:
                                    strDiff = "おに ";
                                    break;
                            }
                            bmpDiff = pfMusicName.DrawPrivateFont(strDiff + stageText, TJAPlayer3.Skin.Game_StageText_ForeColor, TJAPlayer3.Skin.Game_StageText_BackColor );
                        }
                        else
                        {
                            bmpDiff = pfMusicName.DrawPrivateFont(stageText, TJAPlayer3.Skin.Game_StageText_ForeColor, TJAPlayer3.Skin.Game_StageText_BackColor );
                        }

                        using (bmpDiff)
                        {
                            TJAPlayer3.t安全にDisposeする(ref tx難易度とステージ数);
                            tx難易度とステージ数 = TJAPlayer3.tテクスチャの生成(bmpDiff, false);
                        }
					}
					catch( CTextureCreateFailedException e )
					{
						Trace.TraceError( e.ToString() );
						Trace.TraceError( "パネル文字列テクスチャの生成に失敗しました。" );
						this.txPanel = null;
					}
				}

			    this.txGENRE?.Dispose();
                var genreTextureFileName = CStrジャンルtoStr.ForTextureFileName( genreName );
			    this.txGENRE = genreTextureFileName == null ? null : TJAPlayer3.Tx.TxCGen(genreTextureFileName);

			    this.ct進行用 = new CCounter( 0, 2000, 2, TJAPlayer3.Timer );
			}
		}

        public void t歌詞テクスチャを生成する( string str歌詞 )
        {
            using (var bmpleric = this.pf歌詞フォント.DrawPrivateFont( str歌詞, TJAPlayer3.Skin.Game_Lyric_ForeColor, TJAPlayer3.Skin.Game_Lyric_BackColor ))
            {
                this.tx歌詞テクスチャ = TJAPlayer3.tテクスチャの生成( bmpleric, false );
            }
        }

        /// <summary>
        /// レイヤー管理のため、On進行描画から分離。
        /// </summary>
        public void t歌詞テクスチャを描画する()
        {
            tx歌詞テクスチャ?.t2D描画(TJAPlayer3.app.Device, TJAPlayer3.Skin.Game_Lyric_XY[0] , TJAPlayer3.Skin.Game_Lyric_XY[1], TJAPlayer3.Skin.GameLyricHorizontalReferencePoint);
        }

		// CActivity 実装

		public override void On活性化()
		{
            this.pfMusicName = new CPrivateFastFont(FontUtilities.GetFontFamilyOrFallback(TJAPlayer3.ConfigIni.FontName), TJAPlayer3.Skin.Game_MusicName_FontSize);
            this.pf歌詞フォント = new CPrivateFastFont(FontUtilities.GetFontFamilyOrFallback(TJAPlayer3.Skin.Game_Lyric_FontName), TJAPlayer3.Skin.Game_Lyric_FontSize);

			this.txPanel = null;
			this.ct進行用 = new CCounter();
            this.bFirst = true;
			base.On活性化();
		}
		public override void On非活性化()
		{
			this.ct進行用 = null;
			base.On非活性化();
		}
		public override void OnManagedリソースの作成()
		{
			if( !base.b活性化してない )
			{
				base.OnManagedリソースの作成();
			}
		}

        public override void OnManagedリソースの解放()
        {
            if (!b活性化してない)
            {
                TJAPlayer3.t安全にDisposeする(ref txPanel);
                TJAPlayer3.t安全にDisposeする(ref txMusicName);
                TJAPlayer3.t安全にDisposeする(ref txGENRE);
                TJAPlayer3.t安全にDisposeする(ref txPanel);
                TJAPlayer3.t安全にDisposeする(ref tx歌詞テクスチャ);
                TJAPlayer3.t安全にDisposeする(ref pfMusicName);
                TJAPlayer3.t安全にDisposeする(ref pf歌詞フォント);
                TJAPlayer3.t安全にDisposeする(ref tx難易度とステージ数);
                base.OnManagedリソースの解放();
            }
        }

		public override int On進行描画()
		{
			throw new InvalidOperationException( "t進行描画(x,y)のほうを使用してください。" );
		}
		public void t進行描画()
        {
            if (TJAPlayer3.stage演奏ドラム画面.actDan.IsAnimating)
            {
                return;
            }

            if (!base.b活性化してない)
            {
                if(this.b初めての進行描画)
                {
                    b初めての進行描画 = false;
                }

                this.ct進行用.t進行Loop();
                if( this.bFirst )
                {
                    this.ct進行用.n現在の値 = 300;
                }
                if( this.txGENRE != null )
                    this.txGENRE.t2D描画(TJAPlayer3.app.Device, TJAPlayer3.Skin.Game_Genre_XY[0], TJAPlayer3.Skin.Game_Genre_XY[1] );

                if( TJAPlayer3.Skin.b現在のステージ数を表示しない )
                {
                    if( this.txMusicName != null )
                    {
                        float fRate = 660.0f / this.txMusicName.szテクスチャサイズ.Width;
                        if (this.txMusicName.szテクスチャサイズ.Width <= 660.0f)
                            fRate = 1.0f;
                        this.txMusicName.vc拡大縮小倍率.X = fRate;
                        this.txMusicName.t2D描画(TJAPlayer3.app.Device, TJAPlayer3.Skin.Game_MusicName_XY[0], TJAPlayer3.Skin.Game_MusicName_XY[1], TJAPlayer3.Skin.GameMusicNameHorizontalReferencePoint);
                    }
                }
                else
                {
                    #region[ 透明度制御 ]

                    if (ct進行用.n現在の値 < 745)
                    {
                        bFirst = false;
                    }

                    var opacity = 255;
                    if (ct進行用.n現在の値 < 745)
                    {
                        opacity = 255;
                    }
                    else if (ct進行用.n現在の値 >= 745 && ct進行用.n現在の値 < 1000)
                    {
                        opacity = 255 - (ct進行用.n現在の値 - 745);
                    }
                    else if (ct進行用.n現在の値 >= 1000 && ct進行用.n現在の値 <= 1745)
                    {
                        opacity = 0;
                    }
                    else if (ct進行用.n現在の値 >= 1745)
                    {
                        opacity = ct進行用.n現在の値 - 1745;
                    }

                    if (txMusicName != null)
                    {
                        txMusicName.Opacity = opacity;
                    }

                    if (txGENRE != null)
                    {
                        txGENRE.Opacity = opacity;
                    }

                    if (tx難易度とステージ数 != null)
                    {
                        tx難易度とステージ数.Opacity = 255 - opacity;
                    }

                    #endregion

                    txMusicName?.t2D描画(TJAPlayer3.app.Device, TJAPlayer3.Skin.Game_MusicName_XY[0], TJAPlayer3.Skin.Game_MusicName_XY[1], TJAPlayer3.Skin.GameMusicNameHorizontalReferencePoint);

                    tx難易度とステージ数?.t2D描画(TJAPlayer3.app.Device, TJAPlayer3.Skin.Game_MusicName_XY[0], TJAPlayer3.Skin.Game_MusicName_XY[1], TJAPlayer3.Skin.GameMusicNameHorizontalReferencePoint);
                }
            }
		}


		// その他

		#region [ private ]
		//-----------------
		private CCounter ct進行用;

		private CTexture txPanel;
        private bool bFirst;

        private CTexture txMusicName;
        private CTexture tx難易度とステージ数;
        private CTexture txGENRE;
        private CTexture tx歌詞テクスチャ;
        private CPrivateFastFont pfMusicName;
        private CPrivateFastFont pf歌詞フォント;
		//-----------------
		#endregion
	}
}
　
