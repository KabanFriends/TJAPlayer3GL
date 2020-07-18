using FDK;
using TJAPlayer3.Common;

namespace TJAPlayer3
{
	internal class CActResultSongBar : CActivity
	{
		// コンストラクタ

		public CActResultSongBar()
		{
			base.b活性化してない = true;
		}


		// メソッド

		public void tアニメを完了させる()
		{
			this.ct登場用.n現在の値 = this.ct登場用.n終了値;
		}


		// CActivity 実装

        public override void On活性化()
        {
            var fontFamily = FontUtilities.GetFontFamilyOrFallback(TJAPlayer3.ConfigIni.FontName);

            // After performing calibration, inform the player that
            // calibration has been completed, rather than
            // displaying the song title as usual.
			var title = TJAPlayer3.IsPerformingCalibration
                ? $"Calibration complete. InputAdjustTime is now {TJAPlayer3.ConfigIni.nInputAdjustTimeMs}ms"
                : TJAPlayer3.DTX.TITLE;

            using (var pfMusicName = new CPrivateFastFont(fontFamily, TJAPlayer3.Skin.Result_MusicName_FontSize))
            using (var bmpSongTitle = pfMusicName.DrawPrivateFont(title, TJAPlayer3.Skin.Result_MusicName_ForeColor, TJAPlayer3.Skin.Result_MusicName_BackColor))

            {
                txMusicName = TJAPlayer3.tテクスチャの生成(bmpSongTitle, false);
                txMusicName.vc拡大縮小倍率.X = TJAPlayer3.GetSongNameXScaling(ref txMusicName);
            }

            using (var pfStageText = new CPrivateFastFont(fontFamily, TJAPlayer3.Skin.Result_StageText_FontSize))
            using (var bmpStageText = pfStageText.DrawPrivateFont(TJAPlayer3.Skin.Game_StageText, TJAPlayer3.Skin.Result_StageText_ForeColor, TJAPlayer3.Skin.Result_StageText_BackColor))
            {
                txStageText = TJAPlayer3.tテクスチャの生成(bmpStageText, false);
            }

            base.On活性化();
        }

		public override void On非活性化()
		{
			if( this.ct登場用 != null )
			{
				this.ct登場用 = null;
			}
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
			if( !base.b活性化してない )
			{
                TJAPlayer3.t安全にDisposeする(ref this.txMusicName);
                TJAPlayer3.t安全にDisposeする(ref this.txStageText);
                base.OnManagedリソースの解放();
			}
		}
		public override int On進行描画()
		{
			if( base.b活性化してない )
			{
				return 0;
			}
			if( base.b初めての進行描画 )
			{
				this.ct登場用 = new CCounter( 0, 270, 4, TJAPlayer3.Timer );
				base.b初めての進行描画 = false;
			}
			this.ct登場用.t進行();

            this.txMusicName.t2D描画(TJAPlayer3.app.Device, TJAPlayer3.Skin.Result_MusicName_XY[0], TJAPlayer3.Skin.Result_MusicName_XY[1], TJAPlayer3.Skin.ResultMusicNameHorizontalReferencePoint);

            if(TJAPlayer3.stage選曲.n確定された曲の難易度 != (int)Difficulty.Dan)
            {
                this.txStageText.t2D描画(TJAPlayer3.app.Device, TJAPlayer3.Skin.Result_StageText_XY[0], TJAPlayer3.Skin.Result_StageText_XY[1], TJAPlayer3.Skin.ResultStageTextHorizontalReferencePoint);
            }


			if( !this.ct登場用.b終了値に達した )
			{
				return 0;
			}
			return 1;
		}


		// その他

		#region [ private ]
		//-----------------
		private CCounter ct登場用;

        private CTexture txMusicName;
        private CTexture txStageText;
        //-----------------
		#endregion
	}
}
