using FDK;

namespace TJAPlayer3
{
	internal class CActResultImage : CActivity
	{
		// コンストラクタ
        /// <summary>
        /// リザルト画像を表示させるクラス。XG化するにあたって動画は廃止。
        /// また、中央の画像も表示する。(STAGE表示、STANDARD_CLASSICなど)
        /// </summary>
		public CActResultImage()
		{
			base.b活性化してない = true;
		}


		// メソッド

		public void tアニメを完了させる()
		{
			this.ct登場用.n現在の値 = this.ct登場用.n終了値;
		}


		// CActivity 実装

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

				base.OnManagedリソースの解放();
			}
		}
		public override unsafe int On進行描画()
		{
			if( base.b活性化してない )
			{
				return 0;
			}
			if( base.b初めての進行描画 )
			{
				this.ct登場用 = new CCounter( 0, 100, 5, TJAPlayer3.Timer );
				base.b初めての進行描画 = false;
			}
			this.ct登場用.t進行();

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
		private CTexture r表示するリザルト画像;
		private CTexture txリザルト画像;

		//-----------------
		#endregion
	}
}
