using System;
using FDK;

namespace TJAPlayer3
{
	/// <summary>
	/// CAct演奏Drumsゲージ と CAct演奏Gutiarゲージ のbaseクラス。ダメージ計算やDanger/Failed判断もこのクラスで行う。
    /// 
    /// 課題
    /// _STAGE FAILED OFF時にゲージ回復を止める
    /// _黒→閉店までの差を大きくする。
	/// </summary>
	internal abstract class CAct演奏ゲージ共通 : CActivity
	{
		// CActivity 実装

		const double GAUGE_MAX = 100.0;
		const double GAUGE_INITIAL =  2.0 / 3;
		const double GAUGE_MIN = -0.1;
		const double GAUGE_ZERO = 0.0;
	
		public bool bRisky							// Riskyモードか否か
		{
			get;
			private set;
		}
		public int nRiskyTimes_Initial				// Risky初期値
		{
			get;
			private set;
		}
		public int nRiskyTimes						// 残Miss回数
		{
			get;
			private set;
		}
		public bool IsFailed( E楽器パート part )	// 閉店状態になったかどうか
		{
			if ( bRisky) {
				return ( nRiskyTimes <= 0 );
			}
			return this.db現在のゲージ値[ (int) part ] <= GAUGE_MIN;
		}

		public double dbゲージ値	// Drums専用
		{
			get
			{
				return this.db現在のゲージ値[ 0 ];
			}
			set
			{
				this.db現在のゲージ値[ 0 ] = value;
				if ( this.db現在のゲージ値[ 0 ] > GAUGE_MAX )
				{
					this.db現在のゲージ値[ 0 ] = GAUGE_MAX;
				}
			}
		}

        /// <summary>
		/// ゲージの初期化
		/// </summary>
		/// <param name="nRiskyTimes_Initial_">Riskyの初期値(0でRisky未使用)</param>
		public void Init(int nRiskyTimes_InitialVal )		// ゲージ初期化
		{
            //ダメージ値の計算は太鼓の達人譜面Wikiのものを参考にしました。
            if (TJAPlayer3.ConfigIni.eGaugeMode == EGaugeMode.Hard || TJAPlayer3.ConfigIni.eGaugeMode == EGaugeMode.ExHard)
            {
                for (int i = 0; i < 4; i++)
                {
                    this.db現在のゲージ値[i] = 100.0;
                }

                this.dbゲージ値 = 100.0;
            }
            else if (TJAPlayer3.ConfigIni.eGaugeMode == EGaugeMode.Groove)
            {
                for (int i = 0; i < 4; i++)
                {
                    this.db現在のゲージ値[i] = 22.0;
                }

                this.dbゲージ値 = 22.0;
            }
            else
            {
                for (int i = 0; i < 4; i++)
                {
                    this.db現在のゲージ値[i] = 0;
                }

                this.dbゲージ値 = 0;
            }
            //ゲージのMAXまでの最低コンボ数を計算
            float dbGaugeMaxComboValue = 0;
            float[] dbGaugeMaxComboValue_branch = new float[3];
            float dbDamageRate = 2.0f;

            if( nRiskyTimes_InitialVal > 0 )
            {
                this.bRisky = true;
                this.nRiskyTimes = TJAPlayer3.ConfigIni.nRisky;
                this.nRiskyTimes_Initial = TJAPlayer3.ConfigIni.nRisky;
            }

            switch( TJAPlayer3.DTX.LEVELtaiko[TJAPlayer3.stage選曲.n確定された曲の難易度] )
            {
                case 1:
                case 2:
                case 3:
                case 4:
                case 5:
                case 6:
                case 7:
                    {
                        if (TJAPlayer3.DTX.bチップがある.Branch)
                        {
                            dbGaugeMaxComboValue = TJAPlayer3.DTX.nノーツ数[ 3 ] * ( this.fGaugeMaxRate[ 0 ] / 100.0f );
                            for (int i = 0; i < 3; i++)
                            {
                                dbGaugeMaxComboValue_branch[i] = TJAPlayer3.DTX.nノーツ数_Branch[i] * (this.fGaugeMaxRate[0] / 100.0f);
                            }
                            dbDamageRate = 0.625f;
                        }
                        else
                        {
                            dbGaugeMaxComboValue = TJAPlayer3.DTX.nノーツ数[ 3 ] * ( this.fGaugeMaxRate[ 0 ] / 100.0f );
                            dbDamageRate = 0.625f;
                        }
                        break;
                    }


                case 8:
                    {
                        if (TJAPlayer3.DTX.bチップがある.Branch)
                        {
                            dbGaugeMaxComboValue = TJAPlayer3.DTX.nノーツ数[ 3 ] * ( this.fGaugeMaxRate[ 1 ] / 100.0f );
                            for( int i = 0; i < 3; i++ )
                            {
                                dbGaugeMaxComboValue_branch[i] = TJAPlayer3.DTX.nノーツ数_Branch[i] * (this.fGaugeMaxRate[1] / 100.0f);
                            }
                            dbDamageRate = 0.625f;
                        }
                        else
                        {
                            dbGaugeMaxComboValue = TJAPlayer3.DTX.nノーツ数[ 3 ] * ( this.fGaugeMaxRate[ 1 ] / 100.0f );
                            dbDamageRate = 0.625f;
                        }
                        break;
                    }

                case 9:
                case 10:
                    {
                        if (TJAPlayer3.DTX.bチップがある.Branch)
                        {
                            dbGaugeMaxComboValue = TJAPlayer3.DTX.nノーツ数[ 3 ] * ( this.fGaugeMaxRate[ 2 ] / 100.0f );
                            for( int i = 0; i < 3; i++ )
                            {
                                dbGaugeMaxComboValue_branch[i] = TJAPlayer3.DTX.nノーツ数_Branch[i] * (this.fGaugeMaxRate[2] / 100.0f);
                            }
                        }
                        else
                        {
                            dbGaugeMaxComboValue = TJAPlayer3.DTX.nノーツ数[ 3 ] * ( this.fGaugeMaxRate[ 2 ] / 100.0f );
                        }
                        break;
                    }

                default:
                    {
                        if (TJAPlayer3.DTX.bチップがある.Branch)
                        {
                            dbGaugeMaxComboValue = TJAPlayer3.DTX.nノーツ数[ 3 ] * ( this.fGaugeMaxRate[ 2 ] / 100.0f );
                            for( int i = 0; i < 3; i++ )
                            {
                                dbGaugeMaxComboValue_branch[i] = TJAPlayer3.DTX.nノーツ数_Branch[i] * (this.fGaugeMaxRate[2] / 100.0f);
                            }
                        }
                        else
                        {
                            dbGaugeMaxComboValue = TJAPlayer3.DTX.nノーツ数[ 3 ] * ( this.fGaugeMaxRate[ 2 ] / 100.0f );
                        }
                        break;
                    }

            }

            double nGaugeRankValue = 0D;
            double[] nGaugeRankValue_branch = new double[] { 0D, 0D, 0D };
            if (TJAPlayer3.DTX.GaugeIncreaseMode == GaugeIncreaseMode.Normal)
            {
                nGaugeRankValue =  Math.Floor( 10000.0f / dbGaugeMaxComboValue);
                for (int i = 0; i < 3; i++ )
                {
                    nGaugeRankValue_branch[i] = Math.Floor( 10000.0f / dbGaugeMaxComboValue_branch[i]);
                }
            }
            else
            {
                nGaugeRankValue = 10000.0f / dbGaugeMaxComboValue;
                for (int i = 0; i < 3; i++)
                {
                    nGaugeRankValue_branch[i] = 10000.0f / dbGaugeMaxComboValue_branch[i];
                }
            }

            //ゲージ値計算
            //実機に近い計算

            //計算結果がInfintyだった場合も考える。2020.04.21.akasoko26
            #region [ 計算結果がInfintyだった場合も考えて ]
            float fIsDontInfinty = 0.4f;//適当に0.4で
            float[] fAddVolume = new float[] { 1.0f, 0.5f, dbDamageRate };

            for (int i = 0; i < 3; i++)
            {
                for (int l = 0; l < 3; l++)
                {
                    if (!double.IsInfinity(nGaugeRankValue_branch[i] / 100.0f))//値がInfintyかチェック
                    {
                        fIsDontInfinty = (float)(nGaugeRankValue_branch[i] / 100.0f);
                        this.dbゲージ増加量_Branch[i, l] = fIsDontInfinty * fAddVolume[l];
                    }
                }
            }
            for (int i = 0; i < 3; i++)
            {
                for (int l = 0; l < 3; l++)
                {
                    if (double.IsInfinity(nGaugeRankValue_branch[i] / 100.0f))//値がInfintyかチェック
                    {
                        //Infintyだった場合はInfintyではない値 * 3.0をしてその値を利用する。
                        this.dbゲージ増加量_Branch[i, l] = (fIsDontInfinty * fAddVolume[l]) * 3f;
                    }
                }
            }
            #endregion


            this.dbゲージ増加量[0] = (float)nGaugeRankValue / 100.0f;
            this.dbゲージ増加量[1] = (float)(nGaugeRankValue / 100.0f) * 0.5f;
            this.dbゲージ増加量[2] = (float)(nGaugeRankValue / 100.0f) * dbDamageRate;

            //2015.03.26 kairera0467 計算を初期化時にするよう修正。

            #region ゲージの丸め処理
            var increase = new float[] { dbゲージ増加量[0], dbゲージ増加量[1], dbゲージ増加量[2] };
            var increaseBranch = new float[3, 3];
            for (int i = 0; i < 3; i++)
            {
                increaseBranch[i, 0] = dbゲージ増加量_Branch[i, 0];
                increaseBranch[i, 1] = dbゲージ増加量_Branch[i, 1];
                increaseBranch[i, 2] = dbゲージ増加量_Branch[i, 0];
            }
            switch (TJAPlayer3.DTX.GaugeIncreaseMode)
            {
                case GaugeIncreaseMode.Normal:
                case GaugeIncreaseMode.Floor:
                    // 切り捨て
                    for (int i = 0; i < 3; i++)
                    {
                        increase[i] = (float)Math.Truncate(increase[i] * 10000.0f) / 10000.0f;
                    }
                    for (int i = 0; i < 3; i++)
                    {
                        increaseBranch[i, 0] = (float)Math.Truncate(increaseBranch[i, 0] * 10000.0f) / 10000.0f;
                        increaseBranch[i, 1] = (float)Math.Truncate(increaseBranch[i, 1] * 10000.0f) / 10000.0f;
                        increaseBranch[i, 2] = (float)Math.Truncate(increaseBranch[i, 2] * 10000.0f) / 10000.0f;
                    }
                    break;
                case GaugeIncreaseMode.Round:
                    // 四捨五入
                    for (int i = 0; i < 3; i++)
                    {
                        increase[i] = (float)Math.Round(increase[i] * 10000.0f) / 10000.0f;
                    }
                    for (int i = 0; i < 3; i++)
                    {
                        increaseBranch[i, 0] = (float)Math.Round(increaseBranch[i, 0] * 10000.0f) / 10000.0f;
                        increaseBranch[i, 1] = (float)Math.Round(increaseBranch[i, 1] * 10000.0f) / 10000.0f;
                        increaseBranch[i, 2] = (float)Math.Round(increaseBranch[i, 2] * 10000.0f) / 10000.0f;
                    }
                    break;
                case GaugeIncreaseMode.Ceiling:
                    // 切り上げ
                    for (int i = 0; i < 3; i++)
                    {
                        increase[i] = (float)Math.Ceiling(increase[i] * 10000.0f) / 10000.0f;
                    }
                    for (int i = 0; i < 3; i++)
                    {
                        increaseBranch[i, 0] = (float)Math.Ceiling(increaseBranch[i, 0] * 10000.0f) / 10000.0f;
                        increaseBranch[i, 1] = (float)Math.Ceiling(increaseBranch[i, 1] * 10000.0f) / 10000.0f;
                        increaseBranch[i, 2] = (float)Math.Ceiling(increaseBranch[i, 2] * 10000.0f) / 10000.0f;
                    }
                    break;
                case GaugeIncreaseMode.NotFix:
                default:
                    // 丸めない
                    break;
            }

            for (int i = 0; i < 3; i++)
            {
                dbゲージ増加量[i] = increase[i];
            }
            for (int i = 0; i < 3; i++)
            {
                dbゲージ増加量_Branch[i, 0] = increaseBranch[i, 0];
                dbゲージ増加量_Branch[i, 1] = increaseBranch[i, 1];
                dbゲージ増加量_Branch[i, 2] = increaseBranch[i, 2];
            }
            #endregion
        }


        #region [ DAMAGE ]
#if true       // DAMAGELEVELTUNING
        #region [ DAMAGELEVELTUNING ]
        // ----------------------------------
        public float[ , ] fDamageGaugeDelta = {			// #23625 2011.1.10 ickw_284: tuned damage/recover factors
			// drums,   guitar,  bass
			{  0.004f,  0.006f,  0.006f,  0.004f },
			{  0.002f,  0.003f,  0.003f,  0.002f },
			{  0.000f,  0.000f,  0.000f,  0.000f },
			{ -0.020f, -0.030f,	-0.030f, -0.020f },
			{ -0.050f, -0.050f, -0.050f, -0.050f }
		};
		public float[] fDamageLevelFactor = {
			0.5f, 1.0f, 1.5f
		};

        public float[] dbゲージ増加量 = new float[ 3 ];

        //譜面レベル, 判定
        public float[,] dbゲージ増加量_Branch = new float[3, 3];


        public float[] fGaugeMaxRate = 
        {
            70.7f,//1～7
            70f,  //8
            75.0f //9～10
        };//おおよその値。

        // ----------------------------------
        #endregion
#endif

        public void Damage(CDTX.ECourse eHitCourse, E楽器パート screenmode, E楽器パート part, E判定 e今回の判定, int player)
        {
			float fDamage;
            //現在のコースを当てるのではなくヒットしたノーツのコースを当ててあげる.2020.04.21.akasoko26
            var nコース = eHitCourse;


#if true  // DAMAGELEVELTUNING
            switch ( e今回の判定 )
			{
				case E判定.Perfect:
				case E判定.Great:
                    {
                        if (TJAPlayer3.ConfigIni.eGaugeMode == EGaugeMode.Hard || TJAPlayer3.ConfigIni.eGaugeMode == EGaugeMode.ExHard)
                        {
                            fDamage = 0.24f;
                        }
                        else if (TJAPlayer3.ConfigIni.eGaugeMode == EGaugeMode.Groove)
                        { 
                            if (TJAPlayer3.DTX.bチップがある.Branch)
                            {
                                fDamage = this.dbゲージ増加量_Branch[(int)nコース, 0];
                            }
                            else
                                fDamage = 4.5f * this.dbゲージ増加量[0];
                        }       
                        else
                        {
                            if (TJAPlayer3.DTX.bチップがある.Branch)
                            {
                                fDamage = this.dbゲージ増加量_Branch[(int)nコース, 0];
                            }
                            else
                                fDamage = this.dbゲージ増加量[0];
                        }
                    }
                    break;
				case E判定.Good:
                    {
                        if (TJAPlayer3.ConfigIni.eGaugeMode == EGaugeMode.Hard || TJAPlayer3.ConfigIni.eGaugeMode == EGaugeMode.ExHard)
                        {
                            fDamage = 0f;
                        }
                        else if (TJAPlayer3.ConfigIni.eGaugeMode == EGaugeMode.Groove)
                        {
                            if (TJAPlayer3.DTX.bチップがある.Branch)
                            {
                                fDamage = this.dbゲージ増加量_Branch[(int)nコース, 2];
                            }
                            else
                                fDamage = 4.5f * this.dbゲージ増加量[1];
                        }
                        else
                        {
                            if (TJAPlayer3.DTX.bチップがある.Branch)
                            {
                                fDamage = this.dbゲージ増加量_Branch[(int)nコース, 2];
                            }
                            else
                                fDamage = this.dbゲージ増加量[1];
                        }
                    }
					break;
				case E判定.Poor:
                    {
                        if (TJAPlayer3.ConfigIni.eGaugeMode == EGaugeMode.ExHard)
                        {
                            fDamage = -10.0f;
                        }
                        else if (TJAPlayer3.ConfigIni.eGaugeMode == EGaugeMode.Hard)
                        {
                            if (this.db現在のゲージ値[player] < 30)
                            { fDamage = -2.5f; }
                            else { fDamage = -5.0f; }
                        }
                        else if (TJAPlayer3.ConfigIni.eGaugeMode == EGaugeMode.Groove)
                        {
                            fDamage = 2.0f;
                        }
                        else
                        {
                            if (TJAPlayer3.DTX.bチップがある.Branch)
                            {
                                fDamage = this.dbゲージ増加量_Branch[(int)nコース, 0];
                            }
                            else
                                fDamage = this.dbゲージ増加量[2];
                        }

                        if (fDamage >= 0)
                        {
                            fDamage = -fDamage;
                        }
                        if (this.bRisky)
                        {
                            this.nRiskyTimes--;
                        }

                    }
                    break;
                case E判定.Miss:
                    {
                        if (TJAPlayer3.ConfigIni.eGaugeMode == EGaugeMode.ExHard)
                        {
                            fDamage = -18.0f;
                        }
                        else if (TJAPlayer3.ConfigIni.eGaugeMode == EGaugeMode.Hard)
                        {
                            if (this.db現在のゲージ値[player] < 30)
                                 { fDamage = -4.5f; }
                            else { fDamage = -9.0f; }

                        }
                        else if(TJAPlayer3.ConfigIni.eGaugeMode == EGaugeMode.Groove)
                        {
                            fDamage = -6.0f;
                        }
                        else
                        {
                            if (TJAPlayer3.DTX.bチップがある.Branch)
                            {
                                fDamage = this.dbゲージ増加量_Branch[(int)nコース, 0];
                            }
                            else
                                fDamage = this.dbゲージ増加量[2];
                        }

                        if (fDamage >= 0)
                        {
                            fDamage = -fDamage;
                        }
                        if (this.bRisky)
                        {
                            this.nRiskyTimes--;
                        }

                    }

					break;



				default:
                    {
                        if( player == 0 ? TJAPlayer3.ConfigIni.b太鼓パートAutoPlay : TJAPlayer3.ConfigIni.b太鼓パートAutoPlay2P )
                        {
                            if( TJAPlayer3.DTX.bチップがある.Branch )
                            {
                                fDamage = this.dbゲージ増加量_Branch[(int)nコース, 0 ];
                            }
                            else
					            fDamage = this.dbゲージ増加量[ 0 ];
                        }
                        else
                            fDamage = 0;
					    break;
                    }


			}
#else													// before applying #23625 modifications
			switch (e今回の判定)
			{
				case E判定.Perfect:
					fDamage = ( part == E楽器パート.DRUMS ) ? 0.01 : 0.015;
					break;

				case E判定.Great:
					fDamage = ( part == E楽器パート.DRUMS ) ? 0.006 : 0.009;
					break;

				case E判定.Good:
					fDamage = ( part == E楽器パート.DRUMS ) ? 0.002 : 0.003;
					break;

				case E判定.Poor:
					fDamage = ( part == E楽器パート.DRUMS ) ? 0.0 : 0.0;
					break;

				case E判定.Miss:
					fDamage = ( part == E楽器パート.DRUMS ) ? -0.035 : -0.035;
					switch( CDTXMania.ConfigIni.eダメージレベル )
					{
						case Eダメージレベル.少ない:
							fDamage *= 0.6;
							break;

						case Eダメージレベル.普通:
							fDamage *= 1.0;
							break;

						case Eダメージレベル.大きい:
							fDamage *= 1.6;
							break;
					}
					break;

				default:
					fDamage = 0.0;
					break;
			}
#endif
            

            this.db現在のゲージ値[ player ] = Math.Round(this.db現在のゲージ値[ player ] + fDamage, 5, MidpointRounding.ToEven);

            if (this.db現在のゲージ値[player] >= 100.0)
                this.db現在のゲージ値[player] = 100.0;
            else if (this.db現在のゲージ値[player] <= 0.0)
                this.db現在のゲージ値[player] = 0.0;

            //CDTXMania.stage演奏ドラム画面.nGauge = fDamage;

        }

        public virtual void Start(int nLane, E判定 judge, int player)
        {
        }

		//-----------------
		#endregion

		public double[] db現在のゲージ値 = new double[ 4 ];
        protected CCounter ct炎;
        protected CCounter ct虹アニメ;
	    protected CCounter ct虹透明度;
        protected CTexture[] txゲージ虹 = new CTexture[ 12 ];
        protected CTexture[] txゲージ虹2P = new CTexture[ 12 ];
    }
}　
