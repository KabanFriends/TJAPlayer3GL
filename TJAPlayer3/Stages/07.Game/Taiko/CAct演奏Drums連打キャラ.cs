using System;
using System.Runtime.InteropServices;
using FDK;

namespace TJAPlayer3
{
	internal class CAct演奏Drums連打キャラ : CActivity
	{
		// コンストラクタ

		public CAct演奏Drums連打キャラ()
		{
			base.b活性化してない = true;
		}
		
		
		// メソッド
        public virtual void Start( int player )
		{
            //if( CDTXMania.Tx.Effects_Roll[0] != null )
            //{
            //    int[] arXseed = new int[] { 56, -10, 200, 345, 100, 451, 600, 260, -30, 534, 156, 363 };
            //    for (int i = 0; i < 1; i++)
            //    {
            //        for (int j = 0; j < 64; j++)
            //        {
            //            if (!this.st連打キャラ[j].b使用中)
            //            {
            //                this.st連打キャラ[j].b使用中 = true;
            //                if(this.nTex枚数 <= 1) this.st連打キャラ[j].nColor = 0;
            //                else this.st連打キャラ[j].nColor = CDTXMania.Random.Next( 0, this.nTex枚数 - 1);
            //                this.st連打キャラ[j].ct進行 = new CCounter( 0, 1000, 4, CDTXMania.Timer); // カウンタ

            //                //位置生成(β版)
            //                int nXseed = CDTXMania.Random.Next(12);
            //                this.st連打キャラ[ j ].fX開始点 = arXseed[ nXseed ];
            //                this.st連打キャラ[j].fX = arXseed[ nXseed ];
            //                this.st連打キャラ[j].fY = 720;
            //                this.st連打キャラ[j].fX加速度 = 5/2;
            //                this.st連打キャラ[j].fY加速度 = 5/2;
            //                break;
            //            }
            //        }
            //    }
            //}
            for (int i = 0; i < 128; i++)
            {
                if(!RollCharas[i].IsUsing)
                {
                    RollCharas[i].IsUsing = true;
                    RollCharas[i].Type = random.Next(0, TJAPlayer3.Skin.Game_Effect_Roll_Ptn);
                    RollCharas[i].OldValue = 0;
                    RollCharas[i].Counter = new CCounter(0, 5000, 1, TJAPlayer3.Timer);
                    if (TJAPlayer3.stage演奏ドラム画面.bDoublePlay)
                    {
                        switch (player)
                        {
                            case 0:
                                RollCharas[i].X = TJAPlayer3.Skin.Game_Effect_Roll_StartPoint_1P_X[random.Next(0, TJAPlayer3.Skin.Game_Effect_Roll_StartPoint_1P_X.Length)];
                                RollCharas[i].Y = TJAPlayer3.Skin.Game_Effect_Roll_StartPoint_1P_Y[random.Next(0, TJAPlayer3.Skin.Game_Effect_Roll_StartPoint_1P_Y.Length)];
                                RollCharas[i].XAdd = TJAPlayer3.Skin.Game_Effect_Roll_Speed_1P_X[random.Next(0, TJAPlayer3.Skin.Game_Effect_Roll_Speed_1P_X.Length)];
                                RollCharas[i].YAdd = TJAPlayer3.Skin.Game_Effect_Roll_Speed_1P_Y[random.Next(0, TJAPlayer3.Skin.Game_Effect_Roll_Speed_1P_Y.Length)];
                                break;
                            case 1:
                                RollCharas[i].X = TJAPlayer3.Skin.Game_Effect_Roll_StartPoint_2P_X[random.Next(0, TJAPlayer3.Skin.Game_Effect_Roll_StartPoint_2P_X.Length)];
                                RollCharas[i].Y = TJAPlayer3.Skin.Game_Effect_Roll_StartPoint_2P_Y[random.Next(0, TJAPlayer3.Skin.Game_Effect_Roll_StartPoint_2P_Y.Length)];
                                RollCharas[i].XAdd = TJAPlayer3.Skin.Game_Effect_Roll_Speed_2P_X[random.Next(0, TJAPlayer3.Skin.Game_Effect_Roll_Speed_2P_X.Length)];
                                RollCharas[i].YAdd = TJAPlayer3.Skin.Game_Effect_Roll_Speed_2P_Y[random.Next(0, TJAPlayer3.Skin.Game_Effect_Roll_Speed_2P_Y.Length)];
                                break;
                            default:
                                return;
                        }
                    }
                    else
                    {
                        RollCharas[i].X = TJAPlayer3.Skin.Game_Effect_Roll_StartPoint_X[random.Next(0, TJAPlayer3.Skin.Game_Effect_Roll_StartPoint_X.Length)];
                        RollCharas[i].Y = TJAPlayer3.Skin.Game_Effect_Roll_StartPoint_Y[random.Next(0, TJAPlayer3.Skin.Game_Effect_Roll_StartPoint_Y.Length)];
                        RollCharas[i].XAdd = TJAPlayer3.Skin.Game_Effect_Roll_Speed_X[random.Next(0, TJAPlayer3.Skin.Game_Effect_Roll_Speed_X.Length)];
                        RollCharas[i].YAdd = TJAPlayer3.Skin.Game_Effect_Roll_Speed_Y[random.Next(0, TJAPlayer3.Skin.Game_Effect_Roll_Speed_Y.Length)];
                    }
                    break;
                }
            }

		}

		// CActivity 実装

		public override void On活性化()
		{
            //for (int i = 0; i < 64; i++)
            //{
            //    this.st連打キャラ[i] = new ST連打キャラ();
            //    this.st連打キャラ[i].b使用中 = false;
            //    this.st連打キャラ[i].ct進行 = new CCounter();
            //}
            for (int i = 0; i < 128; i++)
            {
                RollCharas[i] = new RollChara();
                RollCharas[i].IsUsing = false;
                RollCharas[i].Counter = new CCounter();
            }
            // SkinConfigで指定されたいくつかの変数からこのクラスに合ったものに変換していく

            base.On活性化();
		}
		public override void On非活性化()
		{
            //for (int i = 0; i < 64; i++)
            //{
            //    this.st連打キャラ[i].ct進行 = null;
            //}
            for (int i = 0; i < 128; i++)
            {
                RollCharas[i].Counter = null;
            }
			base.On非活性化();
		}
		public override void OnManagedリソースの作成()
		{
			if( !base.b活性化してない )
			{
                //this.nTex枚数 = 4;
                //this.txChara = new CTexture[ this.nTex枚数 ];

                //for (int i = 0; i < this.nTex枚数; i++)
                //{
                //    this.txChara[ i ] = CDTXMania.tテクスチャの生成( CSkin.Path( @"Graphics\RollEffect\00\" + i.ToString() + ".png" ) );
                //}
				base.OnManagedリソースの作成();
			}
		}
		public override void OnManagedリソースの解放()
		{
			if( !base.b活性化してない )
			{
        //        for (int i = 0; i < this.nTex枚数; i++)
        //        {
				    //CDTXMania.tテクスチャの解放( ref this.txChara[ i ] );
        //        }
				base.OnManagedリソースの解放();
			}
		}

        public override int On進行描画()
        {
            if (base.b活性化してない)
            {
                return 0;
            }

            for (int i = 0; i < 128; i++)
            {
                var rollChara = RollCharas[i];

                if (!rollChara.IsUsing)
                {
                    continue;
                }

                rollChara.OldValue = rollChara.Counter.n現在の値;
                rollChara.Counter.t進行();
                if (rollChara.Counter.b終了値に達した)
                {
                    rollChara.Counter.t停止();
                    rollChara.IsUsing = false;
                }

                for (int l = rollChara.OldValue; l < rollChara.Counter.n現在の値; l++)
                {
                    rollChara.X += rollChara.XAdd;
                    rollChara.Y += rollChara.YAdd;
                }

                var txRollCharaEffect = TJAPlayer3.Tx.Effects_Roll[rollChara.Type];

                if (txRollCharaEffect == null)
                {
                    continue;
                }

                txRollCharaEffect.t2D描画(TJAPlayer3.app.Device, rollChara.X, rollChara.Y);
                // 画面外にいたら描画をやめさせる
                if (rollChara.X < 0 - txRollCharaEffect.szテクスチャサイズ.Width || rollChara.X > 1280)
                {
                    rollChara.Counter.t停止();
                    rollChara.IsUsing = false;
                }

                if (rollChara.Y < 0 - txRollCharaEffect.szテクスチャサイズ.Height || rollChara.Y > 720)
                {
                    rollChara.Counter.t停止();
                    rollChara.IsUsing = false;
                }
            }

            return 0;
        }


        // その他

		#region [ private ]
		//-----------------
        //private CTexture[] txChara;
        private int nTex枚数;

        [StructLayout(LayoutKind.Sequential)]
        private struct ST連打キャラ
        {
            public int nColor;
            public bool b使用中;
            public CCounter ct進行;
            public int n前回のValue;
            public float fX;
            public float fY;
            public float fX開始点;
            public float fY開始点;
            public float f進行方向; //進行方向 0:左→右 1:左下→右上 2:右→左
            public float fX加速度;
            public float fY加速度;
        }
        private ST連打キャラ[] st連打キャラ = new ST連打キャラ[64];

        [StructLayout(LayoutKind.Sequential)]
        private struct RollChara
        {
            public CCounter Counter;
            public int Type;
            public bool IsUsing;
            public float X;
            public float Y;
            public float XAdd;
            public float YAdd;
            public int OldValue;
        }

        private RollChara[] RollCharas = new RollChara[128];

        private Random random = new Random();

        private int[,] StartPoint;
        private int[,] StartPoint_1P;
        private int[,] StartPoint_2P;
        private float[,] Speed;
        private float[,] Speed_1P;
        private float[,] Speed_2P;
        private int CharaPtn;
        //-----------------
        #endregion
    }
}
