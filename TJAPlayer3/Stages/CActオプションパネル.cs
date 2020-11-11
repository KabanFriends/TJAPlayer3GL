using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using FDK;

namespace TJAPlayer3
{
	internal class CActオプションパネル : CActivity
	{
		// CActivity 実装

		public override void On非活性化()
		{
			if( !base.b活性化してない )
			{
				TJAPlayer3.t安全にDisposeする( ref this.txオプションパネル );
				base.On非活性化();
			}
		}
		public override void OnManagedリソースの作成()
		{
			if( !base.b活性化してない )
			{
				//this.txオプションパネル = TJAPlayer3.tテクスチャの生成( CSkin.Path( @"Graphics\Screen option panels.png" ), false );
				base.OnManagedリソースの作成();
			}
		}
		public override int On進行描画()
		{
			return 0;
		}

		
		// その他

		#region [ private ]
		//-----------------
		private readonly Rectangle[] rcComboPos = new Rectangle[] { new Rectangle( 0x30, 0x48, 0x18, 12 ), new Rectangle( 0x30, 60, 0x18, 12 ), new Rectangle( 0x30, 0x30, 0x18, 12 ), new Rectangle( 0x18, 0x48, 0x18, 12 ) };
		private readonly Rectangle[] rcDark = new Rectangle[] { new Rectangle( 0x18, 0, 0x18, 12 ), new Rectangle( 0x18, 12, 0x18, 12 ), new Rectangle( 0x18, 0x54, 0x18, 12 ) };
		private readonly Rectangle[] rcHS = new Rectangle[] {
			new Rectangle( 0, 0, 0x18, 12 ),		// OFF
			new Rectangle( 0, 12, 0x18, 12 ),		// Hidden
			new Rectangle( 0, 0x18, 0x18, 12 ),		// Sudden
			new Rectangle( 0, 0x24, 0x18, 12 ),		// H/S
			new Rectangle(0x60, 0x54, 0x18, 12 ),	// Semi-Invisible
			new Rectangle( 120, 0x54, 0x18, 12 )	// Full-Invisible
		};
		private readonly Rectangle[] rcLeft = new Rectangle[] { new Rectangle( 0x60, 0x48, 0x18, 12 ), new Rectangle( 120, 0x48, 0x18, 12 ) };
		private readonly Rectangle[] rcLight = new Rectangle[] { new Rectangle( 120, 0x30, 0x18, 12 ), new Rectangle( 120, 60, 0x18, 12 ) };
		private readonly Rectangle[] rcPosition = new Rectangle[] {
			new Rectangle(  0, 48, 24, 12 ),		// P-A
			new Rectangle(  0, 60, 24, 12 ),		// P-B
			new Rectangle(  0, 72, 24, 12 ),		// P-B
			new Rectangle( 24, 72, 24, 12 )			// OFF
		};
		private readonly Rectangle[] rcRandom = new Rectangle[] { new Rectangle( 0x48, 0x30, 0x18, 12 ), new Rectangle( 0x48, 60, 0x18, 12 ), new Rectangle( 0x48, 0x48, 0x18, 12 ), new Rectangle( 0x48, 0x54, 0x18, 12 ) };
		private readonly Rectangle[] rcReverse = new Rectangle[] { new Rectangle( 0x18, 0x18, 0x18, 12 ), new Rectangle( 0x18, 0x24, 0x18, 12 ) };
		private readonly Rectangle[] rcTight = new Rectangle[] { new Rectangle( 0x60, 0x30, 0x18, 12 ), new Rectangle( 0x60, 60, 0x18, 12 ) };
		private readonly Rectangle[] rc譜面スピード = new Rectangle[] { new Rectangle( 0x30, 0, 0x18, 12 ), new Rectangle( 0x30, 12, 0x18, 12 ), new Rectangle( 0x30, 0x18, 0x18, 12 ), new Rectangle( 0x30, 0x24, 0x18, 12 ), new Rectangle( 0x48, 0, 0x18, 12 ), new Rectangle( 0x48, 12, 0x18, 12 ), new Rectangle( 0x48, 0x18, 0x18, 12 ), new Rectangle( 0x48, 0x24, 0x18, 12 ), new Rectangle( 0x60, 0, 0x18, 12 ), new Rectangle( 0x60, 12, 0x18, 12 ), new Rectangle( 0x60, 0x18, 0x18, 12 ), new Rectangle( 0x60, 0x24, 0x18, 12 ), new Rectangle( 120, 0, 0x18, 12 ), new Rectangle( 120, 12, 0x18, 12 ), new Rectangle( 120, 0x18, 0x18, 12 ), new Rectangle( 120, 0x24, 0x18, 12 ) };
		private CTexture txオプションパネル;
		//-----------------
		#endregion
	}
}
