﻿using System.Drawing;
using FDK;

namespace TJAPlayer3
{
	internal sealed class CActFIFOResult : CActivity
	{
		// メソッド

        public void tフェードアウト開始()
        {
            mode = EFIFOモード.フェードアウト;
            counter = new CCounter(0, 500, 2, TJAPlayer3.Timer);
            SetResultFadeInTextureOpaque();
        }

        public void tフェードイン開始()
        {
            mode = EFIFOモード.フェードイン;
            counter = new CCounter(0, 100, 5, TJAPlayer3.Timer);
            SetResultFadeInTextureOpaque();
        }

        private static void SetResultFadeInTextureOpaque()
        {
            var txResultFadeIn = TJAPlayer3.Tx.Result_FadeIn;
            if (txResultFadeIn != null)
            {
                txResultFadeIn.Opacity = 255;
            }
        }

        public void tフェードイン完了()		// #25406 2011.6.9 yyagi
		{
			this.counter.n現在の値 = this.counter.n終了値;
		}

		// CActivity 実装

		public override void On非活性化()
		{
			if( !base.b活性化してない )
			{
                //CDTXMania.tテクスチャの解放( ref this.tx幕 );
				base.On非活性化();
			}
		}
		public override void OnManagedリソースの作成()
		{
			if( !base.b活性化してない )
			{
				//this.tx幕 = CDTXMania.tテクスチャの生成( CSkin.Path( @"Graphics\8_background_mask.png" ) );
				base.OnManagedリソースの作成();
			}
		}
		public override int On進行描画()
		{
			if( base.b活性化してない || ( this.counter == null ) )
			{
				return 0;
			}
			this.counter.t進行();

			// Size clientSize = CDTXMania.app.Window.ClientSize;	// #23510 2010.10.31 yyagi: delete as of no one use this any longer.
			if (TJAPlayer3.Tx.Result_FadeIn != null)
			{
                if( this.mode == EFIFOモード.フェードアウト )
                {
                    int y =  this.counter.n現在の値 >= 360 ? 360 : this.counter.n現在の値;
                    TJAPlayer3.Tx.Result_FadeIn.t2D描画( TJAPlayer3.app.Device, 0, this.counter.n現在の値 >= 360 ? 0 : -360 + y, new Rectangle( 0, 0, 1280, 380 ) );
                    TJAPlayer3.Tx.Result_FadeIn.t2D描画( TJAPlayer3.app.Device, 0, 720 - y, new Rectangle( 0, 380, 1280, 360 ) );
                }
                else
                {
                    TJAPlayer3.Tx.Result_FadeIn.Opacity = (((100 - this.counter.n現在の値) * 0xff) / 100);
                    TJAPlayer3.Tx.Result_FadeIn.t2D描画( TJAPlayer3.app.Device, 0, 0, new Rectangle( 0, 0, 1280, 360 ) );
                    TJAPlayer3.Tx.Result_FadeIn.t2D描画( TJAPlayer3.app.Device, 0, 360, new Rectangle( 0, 380, 1280, 360 ) );
                }


			}
            if( this.mode == EFIFOモード.フェードアウト )
            {
			    if( this.counter.n現在の値 != 500 )
			    {
				    return 0;
			    }
            }
            else if( this.mode == EFIFOモード.フェードイン )
            {
			    if( this.counter.n現在の値 != 100 )
			    {
				    return 0;
			    }
            }
			return 1;
		}


		// その他

		#region [ private ]
		//-----------------
		private CCounter counter;
		private EFIFOモード mode;
        //private CTexture tx幕;
		//-----------------
		#endregion
	}
}
