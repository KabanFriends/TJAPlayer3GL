using System.Drawing;
using System.Runtime.InteropServices;
using FDK;

namespace TJAPlayer3
{
    internal class CAct演奏DrumsMtaiko : CActivity
    {
        public CAct演奏DrumsMtaiko()
        {
            base.b活性化してない = true;
        }

        public override void On活性化()
        {
			for( int i = 0; i < 16; i++ )
			{
				STパッド状態 stパッド状態 = new STパッド状態();
				stパッド状態.n明るさ = 0;
				this.stパッド状態[ i ] = stパッド状態;
			}
            base.On活性化();
        }

        public override void OnManagedリソースの作成()
        {
            this.ctレベルアップダウン = new CCounter[ 4 ];
            this.After = new int[ 4 ];
            this.Before = new int[ 4 ];
            for( int i = 0; i < 4; i++ )
            {
                this.ctレベルアップダウン[ i ] = new CCounter();
            }

            base.OnManagedリソースの作成();
        }

        public override void OnManagedリソースの解放()
        {
            this.ctレベルアップダウン = null;

            base.OnManagedリソースの解放();
        }

        public override int On進行描画()
        {
            if( base.b初めての進行描画 )
			{
				this.nフラッシュ制御タイマ = FDK.CSound管理.rc演奏用タイマ.n現在時刻;
				base.b初めての進行描画 = false;
            }
            
            long num = FDK.CSound管理.rc演奏用タイマ.n現在時刻;
			if( num < this.nフラッシュ制御タイマ )
			{
				this.nフラッシュ制御タイマ = num;
			}
			while( ( num - this.nフラッシュ制御タイマ ) >= 20 )
			{
				for( int j = 0; j < 16; j++ )
				{
					if( this.stパッド状態[ j ].n明るさ > 0 )
					{
						this.stパッド状態[ j ].n明るさ--;
					}
				}
				this.nフラッシュ制御タイマ += 20;
		    }

            TJAPlayer3.Tx.Taiko_Background[0]?.t2D描画( TJAPlayer3.app.Device, TJAPlayer3.Skin.Game_Taiko_Background_X[0], TJAPlayer3.Skin.Game_Taiko_Background_Y[0] );

            if ( TJAPlayer3.stage演奏ドラム画面.bDoublePlay )
            {
                TJAPlayer3.Tx.Taiko_Background[1]?.t2D描画( TJAPlayer3.app.Device, TJAPlayer3.Skin.Game_Taiko_Background_X[1], TJAPlayer3.Skin.Game_Taiko_Background_Y[1]);
            }
            
            if(TJAPlayer3.Tx.Taiko_Base != null )
            {
                TJAPlayer3.Tx.Taiko_Base.t2D描画( TJAPlayer3.app.Device, TJAPlayer3.Skin.Game_Taiko_X[0], TJAPlayer3.Skin.Game_Taiko_Y[0]);
                if( TJAPlayer3.stage演奏ドラム画面.bDoublePlay )
                    TJAPlayer3.Tx.Taiko_Base.t2D描画( TJAPlayer3.app.Device, TJAPlayer3.Skin.Game_Taiko_X[1], TJAPlayer3.Skin.Game_Taiko_Y[1]);
            }

            if( TJAPlayer3.Tx.Taiko_Don_Left != null && TJAPlayer3.Tx.Taiko_Don_Right != null && TJAPlayer3.Tx.Taiko_Ka_Left != null && TJAPlayer3.Tx.Taiko_Ka_Right != null )
            {
                var taikoKaRightTextureSize = TJAPlayer3.Tx.Taiko_Ka_Right.szテクスチャサイズ;
                var halfWidth = taikoKaRightTextureSize.Width / 2;
                var leftDrawingArea = new Rectangle( 0, 0, halfWidth, taikoKaRightTextureSize.Height);
                var rightDrawingArea = new Rectangle(halfWidth, 0, halfWidth, taikoKaRightTextureSize.Height);

                TJAPlayer3.Tx.Taiko_Ka_Left.Opacity = this.stパッド状態[0].n明るさ * 73;
                TJAPlayer3.Tx.Taiko_Ka_Right.Opacity = this.stパッド状態[1].n明るさ * 73;
                TJAPlayer3.Tx.Taiko_Don_Left.Opacity = this.stパッド状態[2].n明るさ * 73;
                TJAPlayer3.Tx.Taiko_Don_Right.Opacity = this.stパッド状態[3].n明るさ * 73;

                var p1LeftX = TJAPlayer3.Skin.Game_Taiko_X[0];
                var p1RightX = p1LeftX + halfWidth;
                var p1Y = TJAPlayer3.Skin.Game_Taiko_Y[0];

                TJAPlayer3.Tx.Taiko_Ka_Left.t2D描画( TJAPlayer3.app.Device, p1LeftX, p1Y, leftDrawingArea );
                TJAPlayer3.Tx.Taiko_Ka_Right.t2D描画( TJAPlayer3.app.Device, p1RightX, p1Y, rightDrawingArea );
                TJAPlayer3.Tx.Taiko_Don_Left.t2D描画( TJAPlayer3.app.Device, p1LeftX, p1Y, leftDrawingArea );
                TJAPlayer3.Tx.Taiko_Don_Right.t2D描画( TJAPlayer3.app.Device, p1RightX, p1Y, rightDrawingArea );

                TJAPlayer3.Tx.Taiko_Ka_Left.Opacity = this.stパッド状態[4].n明るさ * 73;
                TJAPlayer3.Tx.Taiko_Ka_Right.Opacity = this.stパッド状態[5].n明るさ * 73;
                TJAPlayer3.Tx.Taiko_Don_Left.Opacity = this.stパッド状態[6].n明るさ * 73;
                TJAPlayer3.Tx.Taiko_Don_Right.Opacity = this.stパッド状態[7].n明るさ * 73;

                var p2LeftX = TJAPlayer3.Skin.Game_Taiko_X[1];
                var p2RightX = p2LeftX + halfWidth;
                var p2Y = TJAPlayer3.Skin.Game_Taiko_Y[1];

                TJAPlayer3.Tx.Taiko_Ka_Left.t2D描画( TJAPlayer3.app.Device, p2LeftX, p2Y, leftDrawingArea );
                TJAPlayer3.Tx.Taiko_Ka_Right.t2D描画( TJAPlayer3.app.Device, p2RightX, p2Y, rightDrawingArea );
                TJAPlayer3.Tx.Taiko_Don_Left.t2D描画( TJAPlayer3.app.Device, p2LeftX, p2Y, leftDrawingArea );
                TJAPlayer3.Tx.Taiko_Don_Right.t2D描画( TJAPlayer3.app.Device, p2RightX, p2Y, rightDrawingArea );
            }

            for ( int i = 0; i < TJAPlayer3.ConfigIni.nPlayerCount; i++ )
            {
                if( !this.ctレベルアップダウン[ i ].b停止中 )
                {
                    this.ctレベルアップダウン[ i ].t進行();
                    if( this.ctレベルアップダウン[ i ].b終了値に達した ) {
                        this.ctレベルアップダウン[ i ].t停止();
                    }
                }
                if( ( this.ctレベルアップダウン[ i ].b進行中 && ( TJAPlayer3.Tx.Taiko_LevelUp != null && TJAPlayer3.Tx.Taiko_LevelDown != null ) ) && !TJAPlayer3.ConfigIni.bNoInfo )
                {
                    //2017.08.21 kairera0467 t3D描画に変更。
                    float fScale = 1.0f;
                    int nAlpha = 255;
                    float[] fY = new float[] { 206, -206, 0, 0 };
                    if( this.ctレベルアップダウン[ i ].n現在の値 >= 0 && this.ctレベルアップダウン[ i ].n現在の値 <= 20 )
                    {
                        nAlpha = 60;
                        fScale = 1.14f;
                    }
                    else if( this.ctレベルアップダウン[ i ].n現在の値 >= 21 && this.ctレベルアップダウン[ i ].n現在の値 <= 40 )
                    {
                        nAlpha = 60;
                        fScale = 1.19f;
                    }
                    else if( this.ctレベルアップダウン[ i ].n現在の値 >= 41 && this.ctレベルアップダウン[ i ].n現在の値 <= 60 )
                    {
                        nAlpha = 220;
                        fScale = 1.23f;
                    }
                    else if( this.ctレベルアップダウン[ i ].n現在の値 >= 61 && this.ctレベルアップダウン[ i ].n現在の値 <= 80 )
                    {
                        nAlpha = 230;
                        fScale = 1.19f;
                    }
                    else if( this.ctレベルアップダウン[ i ].n現在の値 >= 81 && this.ctレベルアップダウン[ i ].n現在の値 <= 100 )
                    {
                        nAlpha = 240;
                        fScale = 1.14f;
                    }
                    else if( this.ctレベルアップダウン[ i ].n現在の値 >= 101 && this.ctレベルアップダウン[ i ].n現在の値 <= 120 )
                    {
                        nAlpha = 255;
                        fScale = 1.04f;
                    }
                    else
                    {
                        nAlpha = 255;
                        fScale = 1.0f;
                    }

                    SlimDX.Matrix mat = SlimDX.Matrix.Identity;
                    mat *= SlimDX.Matrix.Scaling( fScale, fScale, 1.0f );
                    mat *= SlimDX.Matrix.Translation( -329, fY[ i ], 0 );
                    if( this.After[ i ] - this.Before[ i ] >= 0 )
                    {
                        //レベルアップ
                        TJAPlayer3.Tx.Taiko_LevelUp.Opacity = nAlpha;
                        TJAPlayer3.Tx.Taiko_LevelUp.t3D描画( TJAPlayer3.app.Device, mat );
                    }
                    else
                    {
                        TJAPlayer3.Tx.Taiko_LevelDown.Opacity = nAlpha;
                        TJAPlayer3.Tx.Taiko_LevelDown.t3D描画( TJAPlayer3.app.Device, mat );
                    }
                }
            }

            for( int i = 0; i < TJAPlayer3.ConfigIni.nPlayerCount; i++ )
            {

                if (TJAPlayer3.Tx.Course_Symbol.Length > TJAPlayer3.stage選曲.n確定された曲の難易度)
                {
                    TJAPlayer3.Tx.Course_Symbol[TJAPlayer3.stage選曲.n確定された曲の難易度]?.t2D描画(TJAPlayer3.app.Device,
                        TJAPlayer3.Skin.Game_CourseSymbol_X[i],
                        TJAPlayer3.Skin.Game_CourseSymbol_Y[i]
                    );
                }

                if (TJAPlayer3.ConfigIni.ShinuchiMode && TJAPlayer3.Tx.Course_Symbol.Length > (int)Difficulty.Total)
                {
                    TJAPlayer3.Tx.Course_Symbol[(int) Difficulty.Total]?.t2D描画(TJAPlayer3.app.Device,
                        TJAPlayer3.Skin.Game_CourseSymbol_X[i],
                        TJAPlayer3.Skin.Game_CourseSymbol_Y[i]
                    );
                }
            }

            TJAPlayer3.Tx.Taiko_NamePlate[0]?.t2D描画(TJAPlayer3.app.Device, TJAPlayer3.Skin.Game_Taiko_NamePlate_X[0], TJAPlayer3.Skin.Game_Taiko_NamePlate_Y[0]);
            if(TJAPlayer3.stage演奏ドラム画面.bDoublePlay)
            {
                TJAPlayer3.Tx.Taiko_NamePlate[1]?.t2D描画(TJAPlayer3.app.Device, TJAPlayer3.Skin.Game_Taiko_NamePlate_X[1], TJAPlayer3.Skin.Game_Taiko_NamePlate_Y[1]);
            }

            TJAPlayer3.Tx.Taiko_PlayerNumber[0]?.t2D描画(TJAPlayer3.app.Device, TJAPlayer3.Skin.Game_Taiko_PlayerNumber_X[0], TJAPlayer3.Skin.Game_Taiko_PlayerNumber_Y[0]);
            if (TJAPlayer3.stage演奏ドラム画面.bDoublePlay)
            {
                TJAPlayer3.Tx.Taiko_PlayerNumber[1]?.t2D描画(TJAPlayer3.app.Device, TJAPlayer3.Skin.Game_Taiko_PlayerNumber_X[1], TJAPlayer3.Skin.Game_Taiko_PlayerNumber_Y[1]);
            }

            return base.On進行描画();
        }

        public void tMtaikoEvent( int nChannel, int nHand, int nPlayer )
        {
            if( !TJAPlayer3.ConfigIni.b太鼓パートAutoPlay )
            {
                switch( nChannel )
                {
                    case 0x11:
                    case 0x13:
                    case 0x15:
                    case 0x16:
                    case 0x17:
                        {
                            this.stパッド状態[ 2 + nHand + ( 4 * nPlayer ) ].n明るさ = 8;
                        }
                        break;
                    case 0x12:
                    case 0x14:
                        {
                            this.stパッド状態[ nHand + ( 4 * nPlayer ) ].n明るさ = 8;
                        }
                        break;

                }
            }
            else
            {
                switch( nChannel )
                {
                    case 0x11:
                    case 0x15:
                    case 0x16:
                    case 0x17:
                        {
                            this.stパッド状態[ 2 + nHand + ( 4 * nPlayer ) ].n明るさ = 8;
                        }
                        break;
                            
                    case 0x13:
                        {
                            this.stパッド状態[ 2 + ( 4 * nPlayer ) ].n明るさ = 8;
                            this.stパッド状態[ 3 + ( 4 * nPlayer ) ].n明るさ = 8;
                        }
                        break;

                    case 0x12:
                        {
                            this.stパッド状態[ nHand + ( 4 * nPlayer ) ].n明るさ = 8;
                        }
                        break;

                    case 0x14:
                        {
                            this.stパッド状態[ 0 + ( 4 * nPlayer ) ].n明るさ = 8;
                            this.stパッド状態[ 1 + ( 4 * nPlayer ) ].n明るさ = 8;
                        }
                        break;
                }
            }

        }

        public void tBranchEvent( int Before, int After, int player )
        {
            if( After != Before )
                this.ctレベルアップダウン[ player ] = new CCounter( 0, 1000, 1, TJAPlayer3.Timer );

            this.After[ player ] = After;
            this.Before[ player ] = Before;
        }


        #region[ private ]
        //-----------------
        //構造体
        [StructLayout(LayoutKind.Sequential)]
        private struct STパッド状態
        {
            public int n明るさ;
        }

        private readonly STパッド状態[] stパッド状態 = new STパッド状態[ 4 * 4 ];

        private long nフラッシュ制御タイマ;

        //譜面分岐
        private CCounter[] ctレベルアップダウン;
        private int[] After;
        private int[] Before;
        //-----------------
        #endregion

    }
}
　
