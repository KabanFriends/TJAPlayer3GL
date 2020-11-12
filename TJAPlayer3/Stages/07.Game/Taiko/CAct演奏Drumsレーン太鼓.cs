﻿using System.Drawing;
using OpenTK;
using FDK;

namespace TJAPlayer3
{
    internal class CAct演奏Drumsレーン太鼓 : CActivity
    {
        /// <summary>
        /// レーンを描画するクラス。
        /// 
        /// 
        /// </summary>
        public CAct演奏Drumsレーン太鼓()
        {
            base.b活性化してない = true;
        }

        public override void On活性化()
        {
            for (int i = 0; i < 4; i++)
            {
                this.st状態[i].ct進行 = new CCounter();
                this.stBranch[i].ct分岐アニメ進行 = new CCounter();
                this.stBranch[i].nフラッシュ制御タイマ = -1;
                this.stBranch[i].nBranchレイヤー透明度 = 0;
                this.stBranch[i].nBranch文字透明度 = 0;
                this.stBranch[i].nY座標 = 0;
            }
            this.ctゴーゴー = new CCounter();


            this.n総移動時間 = -1;
            this.n総移動時間2 = -1;
            this.nDefaultJudgePos[0] = TJAPlayer3.Skin.Game_Lane_Field_X[0];
            this.nDefaultJudgePos[1] = TJAPlayer3.Skin.Game_Lane_Field_Y[0];
            this.ctゴーゴー炎 = new CCounter(0, 6, 50, TJAPlayer3.Timer);
            base.On活性化();
        }

        public override void On非活性化()
        {
            for (int i = 0; i < 4; i++)
            {
                this.st状態[i].ct進行 = null;
                this.stBranch[i].ct分岐アニメ進行 = null;
            }
            TJAPlayer3.Skin.Game_Lane_Field_X[0] = this.nDefaultJudgePos[0];
            TJAPlayer3.Skin.Game_Lane_Field_Y[0] = this.nDefaultJudgePos[1];
            this.ctゴーゴー = null;

            base.On非活性化();
        }

        public override int On進行描画()
        {
            if (base.b初めての進行描画)
            {
                for (int i = 0; i < 4; i++)
                    this.stBranch[i].nフラッシュ制御タイマ = (long)(CSound管理.rc演奏用タイマ.n現在時刻 * (((double)TJAPlayer3.ConfigIni.n演奏速度) / 20.0));
                base.b初めての進行描画 = false;
            }

            //それぞれが独立したレイヤーでないといけないのでforループはパーツごとに分離すること。

            #region[ レーン本体 ]
            if (TJAPlayer3.Tx.Lane_Background_Main != null)
            {
                for (int i = 0; i < TJAPlayer3.ConfigIni.nPlayerCount; i++)
                {
                    TJAPlayer3.Tx.Lane_Background_Main.t2D描画(TJAPlayer3.app.Device, TJAPlayer3.Skin.Game_Lane_Background_X[i], TJAPlayer3.Skin.Game_Lane_Background_Y[i]);
                }
            }
            #endregion
            for (int i = 0; i < TJAPlayer3.ConfigIni.nPlayerCount; i++)
            {
                #region[ 分岐アニメ制御タイマー ]
                long num = FDK.CSound管理.rc演奏用タイマ.n現在時刻;
                if (num < this.stBranch[i].nフラッシュ制御タイマ)
                {
                    this.stBranch[i].nフラッシュ制御タイマ = num;
                }
                while ((num - this.stBranch[i].nフラッシュ制御タイマ) >= 30)
                {
                    if (this.stBranch[i].nBranchレイヤー透明度 <= 255)
                    {
                        this.stBranch[i].nBranchレイヤー透明度 += 10;
                    }

                    if (this.stBranch[i].nBranch文字透明度 >= 0)
                    {
                        this.stBranch[i].nBranch文字透明度 -= 10;
                    }

                    if (this.stBranch[i].nY座標 != 0 && this.stBranch[i].nY座標 <= 20)
                    {
                        this.stBranch[i].nY座標++;
                    }

                    this.stBranch[i].nフラッシュ制御タイマ += 8;
                }

                if (!this.stBranch[i].ct分岐アニメ進行.b停止中)
                {
                    this.stBranch[i].ct分岐アニメ進行.t進行();
                    if (this.stBranch[i].ct分岐アニメ進行.b終了値に達した)
                    {
                        this.stBranch[i].ct分岐アニメ進行.t停止();
                    }
                }
                #endregion
            }
            #region[ 分岐レイヤー ]
            for (int i = 0; i < TJAPlayer3.ConfigIni.nPlayerCount; i++)
            {
                if (TJAPlayer3.stage演奏ドラム画面.bUseBranch[i] == true)
                {
                    #region[ 動いていない ]
                    switch (TJAPlayer3.stage演奏ドラム画面.nレーン用表示コース[i])
                    {
                        case CDTX.ECourse.eNormal:
                            if (TJAPlayer3.Tx.Lane_Base[0] != null)
                            {
                                TJAPlayer3.Tx.Lane_Base[0].Opacity = 255;
                                TJAPlayer3.Tx.Lane_Base[0].t2D描画(TJAPlayer3.app.Device, TJAPlayer3.Skin.Game_Lane_Background_X[i], TJAPlayer3.Skin.Game_Lane_Background_Y[i]);
                            }
                            break;
                        case CDTX.ECourse.eExpert:
                            if (TJAPlayer3.Tx.Lane_Base[1] != null)
                            {
                                TJAPlayer3.Tx.Lane_Base[1].Opacity = 255;
                                TJAPlayer3.Tx.Lane_Base[1].t2D描画(TJAPlayer3.app.Device, TJAPlayer3.Skin.Game_Lane_Background_X[i], TJAPlayer3.Skin.Game_Lane_Background_Y[i]);
                            }
                            break;
                        case CDTX.ECourse.eMaster:
                            if (TJAPlayer3.Tx.Lane_Base[2] != null)
                            {
                                TJAPlayer3.Tx.Lane_Base[2].Opacity = 255;
                                TJAPlayer3.Tx.Lane_Base[2].t2D描画(TJAPlayer3.app.Device, TJAPlayer3.Skin.Game_Lane_Background_X[i], TJAPlayer3.Skin.Game_Lane_Background_Y[i]);
                            }
                            break;
                    }
                    #endregion

                    if (TJAPlayer3.ConfigIni.nBranchAnime == 1)
                    {
                        #region[ AC7～14風の背後レイヤー ]
                        if (this.stBranch[i].ct分岐アニメ進行.b進行中)
                        {
                            int n透明度 = ((100 - this.stBranch[i].ct分岐アニメ進行.n現在の値) * 0xff) / 100;

                            if (this.stBranch[i].ct分岐アニメ進行.b終了値に達した)
                            {
                                n透明度 = 255;
                                this.stBranch[i].ct分岐アニメ進行.t停止();
                            }

                            #region[ 普通譜面_レベルアップ ]
                            //普通→玄人
                            if (this.stBranch[i].nBefore == CDTX.ECourse.eNormal && this.stBranch[i].nAfter == CDTX.ECourse.eExpert)
                            {
                                if (TJAPlayer3.Tx.Lane_Base[0] != null && TJAPlayer3.Tx.Lane_Base[1] != null)
                                {
                                    TJAPlayer3.Tx.Lane_Base[0].t2D描画(TJAPlayer3.app.Device, TJAPlayer3.Skin.Game_Lane_Background_X[i], TJAPlayer3.Skin.Game_Lane_Background_Y[i]);
                                    TJAPlayer3.Tx.Lane_Base[1].Opacity = this.stBranch[i].nBranchレイヤー透明度;
                                    TJAPlayer3.Tx.Lane_Base[1].t2D描画(TJAPlayer3.app.Device, TJAPlayer3.Skin.Game_Lane_Background_X[i], TJAPlayer3.Skin.Game_Lane_Background_Y[i]);
                                }
                            }
                            //普通→達人
                            if (this.stBranch[i].nBefore == CDTX.ECourse.eNormal && this.stBranch[i].nAfter == CDTX.ECourse.eMaster)
                            {
                                if (this.stBranch[i].ct分岐アニメ進行.n現在の値 < 100)
                                {
                                    n透明度 = ((100 - this.stBranch[i].ct分岐アニメ進行.n現在の値) * 0xff) / 100;
                                }
                                if (TJAPlayer3.Tx.Lane_Base[0] != null && TJAPlayer3.Tx.Lane_Base[2] != null)
                                {
                                    TJAPlayer3.Tx.Lane_Base[0].t2D描画(TJAPlayer3.app.Device, TJAPlayer3.Skin.Game_Lane_Background_X[i], TJAPlayer3.Skin.Game_Lane_Background_Y[i]);
                                    TJAPlayer3.Tx.Lane_Base[2].t2D描画(TJAPlayer3.app.Device, TJAPlayer3.Skin.Game_Lane_Background_X[i], TJAPlayer3.Skin.Game_Lane_Background_Y[i]);
                                    TJAPlayer3.Tx.Lane_Base[2].Opacity = this.stBranch[i].nBranchレイヤー透明度;
                                }
                            }
                            #endregion
                            #region[ 玄人譜面_レベルアップ ]
                            if (this.stBranch[i].nBefore == CDTX.ECourse.eExpert && this.stBranch[i].nAfter == CDTX.ECourse.eMaster)
                            {
                                if (TJAPlayer3.Tx.Lane_Base[1] != null && TJAPlayer3.Tx.Lane_Base[2] != null)
                                {
                                    TJAPlayer3.Tx.Lane_Base[1].t2D描画(TJAPlayer3.app.Device, TJAPlayer3.Skin.Game_Lane_Background_X[i], TJAPlayer3.Skin.Game_Lane_Background_Y[i]);
                                    TJAPlayer3.Tx.Lane_Base[2].t2D描画(TJAPlayer3.app.Device, TJAPlayer3.Skin.Game_Lane_Background_X[i], TJAPlayer3.Skin.Game_Lane_Background_Y[i]);
                                    TJAPlayer3.Tx.Lane_Base[2].Opacity = this.stBranch[i].nBranchレイヤー透明度;
                                }
                            }
                            #endregion
                            #region[ 玄人譜面_レベルダウン ]
                            if (this.stBranch[i].nBefore == CDTX.ECourse.eExpert && this.stBranch[i].nAfter == CDTX.ECourse.eNormal)
                            {
                                if (TJAPlayer3.Tx.Lane_Base[1] != null && TJAPlayer3.Tx.Lane_Base[0] != null)
                                {
                                    TJAPlayer3.Tx.Lane_Base[1].t2D描画(TJAPlayer3.app.Device, TJAPlayer3.Skin.Game_Lane_Background_X[i], TJAPlayer3.Skin.Game_Lane_Background_Y[i]);
                                    TJAPlayer3.Tx.Lane_Base[0].t2D描画(TJAPlayer3.app.Device, TJAPlayer3.Skin.Game_Lane_Background_X[i], TJAPlayer3.Skin.Game_Lane_Background_Y[i]);
                                    TJAPlayer3.Tx.Lane_Base[0].Opacity = this.stBranch[i].nBranchレイヤー透明度;
                                }
                            }
                            #endregion
                            #region[ 達人譜面_レベルダウン ]
                            if (this.stBranch[i].nBefore == CDTX.ECourse.eMaster && this.stBranch[i].nAfter == CDTX.ECourse.eNormal)
                            {
                                if (TJAPlayer3.Tx.Lane_Base[2] != null && TJAPlayer3.Tx.Lane_Base[0] != null)
                                {
                                    TJAPlayer3.Tx.Lane_Base[2].t2D描画(TJAPlayer3.app.Device, TJAPlayer3.Skin.Game_Lane_Background_X[i], TJAPlayer3.Skin.Game_Lane_Background_Y[i]);
                                    TJAPlayer3.Tx.Lane_Base[0].t2D描画(TJAPlayer3.app.Device, TJAPlayer3.Skin.Game_Lane_Background_X[i], TJAPlayer3.Skin.Game_Lane_Background_Y[i]);
                                    TJAPlayer3.Tx.Lane_Base[0].Opacity = this.stBranch[i].nBranchレイヤー透明度;
                                }
                            }
                            #endregion
                        }
                        #endregion
                    }
                    else if (TJAPlayer3.ConfigIni.nBranchAnime == 0)
                    {
                        TJAPlayer3.stage演奏ドラム画面.actLane.On進行描画();
                    }
                }
            }
            #endregion
            for (int i = 0; i < TJAPlayer3.ConfigIni.nPlayerCount; i++)
            {
                #region[ ゴーゴータイムレーン背景レイヤー ]
                if (TJAPlayer3.Tx.Lane_Background_GoGo != null && TJAPlayer3.stage演奏ドラム画面.bIsGOGOTIME[i])
                {
                    if (!this.ctゴーゴー.b停止中)
                    {
                        this.ctゴーゴー.t進行();
                    }

                    if (this.ctゴーゴー.n現在の値 <= 4)
                    {
                        TJAPlayer3.Tx.Lane_Background_GoGo.vc拡大縮小倍率.Y = 0.2f;
                        TJAPlayer3.Tx.Lane_Background_GoGo.t2D描画(TJAPlayer3.app.Device, TJAPlayer3.Skin.Game_Lane_Background_X[i], TJAPlayer3.Skin.Game_Lane_Background_Y[i] + 54);
                    }
                    else if (this.ctゴーゴー.n現在の値 <= 5)
                    {
                        TJAPlayer3.Tx.Lane_Background_GoGo.vc拡大縮小倍率.Y = 0.4f;
                        TJAPlayer3.Tx.Lane_Background_GoGo.t2D描画(TJAPlayer3.app.Device, TJAPlayer3.Skin.Game_Lane_Background_X[i], TJAPlayer3.Skin.Game_Lane_Background_Y[i] + 40);
                    }
                    else if (this.ctゴーゴー.n現在の値 <= 6)
                    {
                        TJAPlayer3.Tx.Lane_Background_GoGo.vc拡大縮小倍率.Y = 0.6f;
                        TJAPlayer3.Tx.Lane_Background_GoGo.t2D描画(TJAPlayer3.app.Device, TJAPlayer3.Skin.Game_Lane_Background_X[i], TJAPlayer3.Skin.Game_Lane_Background_Y[i] + 26);
                    }
                    else if (this.ctゴーゴー.n現在の値 <= 8)
                    {
                        TJAPlayer3.Tx.Lane_Background_GoGo.vc拡大縮小倍率.Y = 0.8f;
                        TJAPlayer3.Tx.Lane_Background_GoGo.t2D描画(TJAPlayer3.app.Device, TJAPlayer3.Skin.Game_Lane_Background_X[i], TJAPlayer3.Skin.Game_Lane_Background_Y[i] + 13);
                    }
                    else if (this.ctゴーゴー.n現在の値 >= 9)
                    {
                        TJAPlayer3.Tx.Lane_Background_GoGo.vc拡大縮小倍率.Y = 1.0f;
                        TJAPlayer3.Tx.Lane_Background_GoGo.t2D描画(TJAPlayer3.app.Device, TJAPlayer3.Skin.Game_Lane_Background_X[i], TJAPlayer3.Skin.Game_Lane_Background_Y[i]);
                    }
                }
                #endregion
            }
            
            for (int i = 0; i < TJAPlayer3.ConfigIni.nPlayerCount; i++)
            {
                if (TJAPlayer3.stage演奏ドラム画面.bUseBranch[i] == true)
                {
                    if (TJAPlayer3.ConfigIni.nBranchAnime == 0)
                    {
                        if (!this.stBranch[i].ct分岐アニメ進行.b進行中)
                        {
                            switch (TJAPlayer3.stage演奏ドラム画面.nレーン用表示コース[i])
                            {
                                case CDTX.ECourse.eNormal:
                                    TJAPlayer3.Tx.Lane_Text[0].Opacity = 255;
                                    TJAPlayer3.Tx.Lane_Text[0].t2D描画(TJAPlayer3.app.Device, TJAPlayer3.Skin.Game_Lane_Background_X[i], TJAPlayer3.Skin.Game_Lane_Background_Y[i]);
                                    break;
                                case CDTX.ECourse.eExpert:
                                    TJAPlayer3.Tx.Lane_Text[1].Opacity = 255;
                                    TJAPlayer3.Tx.Lane_Text[1].t2D描画(TJAPlayer3.app.Device, TJAPlayer3.Skin.Game_Lane_Background_X[i], TJAPlayer3.Skin.Game_Lane_Background_Y[i]);
                                    break;
                                case CDTX.ECourse.eMaster:
                                    TJAPlayer3.Tx.Lane_Text[2].Opacity = 255;
                                    TJAPlayer3.Tx.Lane_Text[2].t2D描画(TJAPlayer3.app.Device, TJAPlayer3.Skin.Game_Lane_Background_X[i], TJAPlayer3.Skin.Game_Lane_Background_Y[i]);
                                    break;
                            }
                        }
                        if (this.stBranch[i].ct分岐アニメ進行.b進行中)
                        {
                            #region[ 普通譜面_レベルアップ ]
                            //普通→玄人
                            if (this.stBranch[i].nBefore == 0 && this.stBranch[i].nAfter == CDTX.ECourse.eExpert)
                            {
                                TJAPlayer3.Tx.Lane_Text[0].Opacity = 255;
                                TJAPlayer3.Tx.Lane_Text[1].Opacity = 255;
                                TJAPlayer3.Tx.Lane_Text[2].Opacity = 255;

                                TJAPlayer3.Tx.Lane_Text[0].Opacity = this.stBranch[i].ct分岐アニメ進行.n現在の値 > 100 ? 0 : (255 - ((this.stBranch[i].ct分岐アニメ進行.n現在の値 * 0xff) / 60));
                                if (this.stBranch[i].ct分岐アニメ進行.n現在の値 < 60)
                                {
                                    this.stBranch[i].nY = this.stBranch[i].ct分岐アニメ進行.n現在の値 / 2;
                                    TJAPlayer3.Tx.Lane_Text[0].t2D描画(TJAPlayer3.app.Device, TJAPlayer3.Skin.Game_Lane_Background_X[i], TJAPlayer3.Skin.Game_Lane_Background_Y[i] + this.stBranch[i].nY);
                                    TJAPlayer3.Tx.Lane_Text[1].Opacity = 255;
                                    TJAPlayer3.Tx.Lane_Text[1].t2D描画(TJAPlayer3.app.Device, TJAPlayer3.Skin.Game_Lane_Background_X[i], (TJAPlayer3.Skin.Game_Lane_Background_Y[i] - 30) + this.stBranch[i].nY);
                                }
                                else
                                {
                                    TJAPlayer3.Tx.Lane_Text[1].Opacity = 255;
                                    TJAPlayer3.Tx.Lane_Text[1].t2D描画(TJAPlayer3.app.Device, TJAPlayer3.Skin.Game_Lane_Background_X[i], TJAPlayer3.Skin.Game_Lane_Background_Y[i]);
                                }

                            }

                            //普通→達人
                            if (this.stBranch[i].nBefore == 0 && this.stBranch[i].nAfter == CDTX.ECourse.eMaster)
                            {
                                TJAPlayer3.Tx.Lane_Text[0].Opacity = 255;
                                TJAPlayer3.Tx.Lane_Text[1].Opacity = 255;
                                TJAPlayer3.Tx.Lane_Text[2].Opacity = 255;
                                if (this.stBranch[i].ct分岐アニメ進行.n現在の値 < 60)
                                {
                                    this.stBranch[i].nY = this.stBranch[i].ct分岐アニメ進行.n現在の値 / 2;
                                    TJAPlayer3.Tx.Lane_Text[0].t2D描画(TJAPlayer3.app.Device, TJAPlayer3.Skin.Game_Lane_Background_X[i], (TJAPlayer3.Skin.Game_Lane_Background_Y[i] - 12) + this.stBranch[i].nY);
                                    TJAPlayer3.Tx.Lane_Text[0].Opacity = this.stBranch[i].ct分岐アニメ進行.n現在の値 > 100 ? 0 : (255 - ((this.stBranch[i].ct分岐アニメ進行.n現在の値 * 0xff) / 100));
                                    TJAPlayer3.Tx.Lane_Text[1].t2D描画(TJAPlayer3.app.Device, TJAPlayer3.Skin.Game_Lane_Background_X[i], (TJAPlayer3.Skin.Game_Lane_Background_Y[i] - 20) + this.stBranch[i].nY);
                                }
                                else if (this.stBranch[i].ct分岐アニメ進行.n現在の値 >= 60 && this.stBranch[i].ct分岐アニメ進行.n現在の値 < 150)
                                {
                                    this.stBranch[i].nY = 21;
                                    TJAPlayer3.Tx.Lane_Text[1].t2D描画(TJAPlayer3.app.Device, TJAPlayer3.Skin.Game_Lane_Background_X[i], TJAPlayer3.Skin.Game_Lane_Background_Y[i]);
                                    TJAPlayer3.Tx.Lane_Text[1].Opacity = 255;
                                    TJAPlayer3.Tx.Lane_Text[2].Opacity = 255;
                                }
                                else if (this.stBranch[i].ct分岐アニメ進行.n現在の値 >= 150 && this.stBranch[i].ct分岐アニメ進行.n現在の値 < 210)
                                {
                                    this.stBranch[i].nY = ((this.stBranch[i].ct分岐アニメ進行.n現在の値 - 150) / 2);
                                    TJAPlayer3.Tx.Lane_Text[1].t2D描画(TJAPlayer3.app.Device, TJAPlayer3.Skin.Game_Lane_Background_X[i], TJAPlayer3.Skin.Game_Lane_Background_Y[i] + this.stBranch[i].nY);
                                    TJAPlayer3.Tx.Lane_Text[1].Opacity = this.stBranch[i].ct分岐アニメ進行.n現在の値 > 100 ? 0 : (255 - ((this.stBranch[i].ct分岐アニメ進行.n現在の値 * 0xff) / 100));
                                    TJAPlayer3.Tx.Lane_Text[2].t2D描画(TJAPlayer3.app.Device, TJAPlayer3.Skin.Game_Lane_Background_X[i], (TJAPlayer3.Skin.Game_Lane_Background_Y[i] - 20) + this.stBranch[i].nY);
                                }
                                else
                                {
                                    TJAPlayer3.Tx.Lane_Text[2].Opacity = 255;
                                    TJAPlayer3.Tx.Lane_Text[2].t2D描画(TJAPlayer3.app.Device, TJAPlayer3.Skin.Game_Lane_Background_X[i], TJAPlayer3.Skin.Game_Lane_Background_Y[i]);
                                }
                            }
                            #endregion
                            #region[ 玄人譜面_レベルアップ ]
                            //玄人→達人
                            if (this.stBranch[i].nBefore == CDTX.ECourse.eExpert && this.stBranch[i].nAfter == CDTX.ECourse.eMaster)
                            {
                                TJAPlayer3.Tx.Lane_Text[0].Opacity = 255;
                                TJAPlayer3.Tx.Lane_Text[1].Opacity = 255;
                                TJAPlayer3.Tx.Lane_Text[2].Opacity = 255;

                                TJAPlayer3.Tx.Lane_Text[1].Opacity = this.stBranch[i].ct分岐アニメ進行.n現在の値 > 100 ? 0 : (255 - ((this.stBranch[i].ct分岐アニメ進行.n現在の値 * 0xff) / 60));
                                if (this.stBranch[i].ct分岐アニメ進行.n現在の値 < 60)
                                {
                                    this.stBranch[i].nY = this.stBranch[i].ct分岐アニメ進行.n現在の値 / 2;
                                    TJAPlayer3.Tx.Lane_Text[1].t2D描画(TJAPlayer3.app.Device, TJAPlayer3.Skin.Game_Lane_Background_X[i], TJAPlayer3.Skin.Game_Lane_Background_Y[i] + this.stBranch[i].nY);
                                    TJAPlayer3.Tx.Lane_Text[2].t2D描画(TJAPlayer3.app.Device, TJAPlayer3.Skin.Game_Lane_Background_X[i], (TJAPlayer3.Skin.Game_Lane_Background_Y[i] - 20) + this.stBranch[i].nY);
                                }
                                else
                                {
                                    TJAPlayer3.Tx.Lane_Text[2].t2D描画(TJAPlayer3.app.Device, TJAPlayer3.Skin.Game_Lane_Background_X[i], TJAPlayer3.Skin.Game_Lane_Background_Y[i]);
                                }
                            }
                            #endregion
                            #region[ 玄人譜面_レベルダウン ]
                            if (this.stBranch[i].nBefore == CDTX.ECourse.eExpert && this.stBranch[i].nAfter == CDTX.ECourse.eNormal)
                            {
                                TJAPlayer3.Tx.Lane_Text[0].Opacity = 255;
                                TJAPlayer3.Tx.Lane_Text[1].Opacity = 255;
                                TJAPlayer3.Tx.Lane_Text[2].Opacity = 255;

                                TJAPlayer3.Tx.Lane_Text[1].Opacity = this.stBranch[i].ct分岐アニメ進行.n現在の値 > 100 ? 0 : (255 - ((this.stBranch[i].ct分岐アニメ進行.n現在の値 * 0xff) / 60));
                                if (this.stBranch[i].ct分岐アニメ進行.n現在の値 < 60)
                                {
                                    this.stBranch[i].nY = this.stBranch[i].ct分岐アニメ進行.n現在の値 / 2;
                                    TJAPlayer3.Tx.Lane_Text[1].t2D描画(TJAPlayer3.app.Device, TJAPlayer3.Skin.Game_Lane_Background_X[i], TJAPlayer3.Skin.Game_Lane_Background_Y[i] - this.stBranch[i].nY);
                                    TJAPlayer3.Tx.Lane_Text[0].t2D描画(TJAPlayer3.app.Device, TJAPlayer3.Skin.Game_Lane_Background_X[i], (TJAPlayer3.Skin.Game_Lane_Background_Y[i] + 30) - this.stBranch[i].nY);
                                }
                                else
                                {
                                    TJAPlayer3.Tx.Lane_Text[0].t2D描画(TJAPlayer3.app.Device, TJAPlayer3.Skin.Game_Lane_Background_X[i], TJAPlayer3.Skin.Game_Lane_Background_Y[i]);
                                }
                            }
                            #endregion
                            #region[ 達人譜面_レベルダウン ]
                            if (this.stBranch[i].nBefore == CDTX.ECourse.eMaster && this.stBranch[i].nAfter == CDTX.ECourse.eNormal)
                            {
                                TJAPlayer3.Tx.Lane_Text[0].Opacity = 255;
                                TJAPlayer3.Tx.Lane_Text[1].Opacity = 255;
                                TJAPlayer3.Tx.Lane_Text[2].Opacity = 255;

                                if (this.stBranch[i].ct分岐アニメ進行.n現在の値 < 60)
                                {
                                    this.stBranch[i].nY = this.stBranch[i].ct分岐アニメ進行.n現在の値 / 2;
                                    TJAPlayer3.Tx.Lane_Text[2].Opacity = this.stBranch[i].ct分岐アニメ進行.n現在の値 > 100 ? 0 : (255 - ((this.stBranch[i].ct分岐アニメ進行.n現在の値 * 0xff) / 60));
                                    TJAPlayer3.Tx.Lane_Text[2].t2D描画(TJAPlayer3.app.Device, TJAPlayer3.Skin.Game_Lane_Background_X[i], TJAPlayer3.Skin.Game_Lane_Background_Y[i] - this.stBranch[i].nY);
                                    TJAPlayer3.Tx.Lane_Text[1].t2D描画(TJAPlayer3.app.Device, TJAPlayer3.Skin.Game_Lane_Background_X[i], (TJAPlayer3.Skin.Game_Lane_Background_Y[i] + 30) - this.stBranch[i].nY);
                                }
                                else if (this.stBranch[i].ct分岐アニメ進行.n現在の値 >= 60 && this.stBranch[i].ct分岐アニメ進行.n現在の値 < 150)
                                {
                                    this.stBranch[i].nY = 21;
                                    TJAPlayer3.Tx.Lane_Text[1].t2D描画(TJAPlayer3.app.Device, TJAPlayer3.Skin.Game_Lane_Background_X[i], TJAPlayer3.Skin.Game_Lane_Background_Y[i]);
                                    TJAPlayer3.Tx.Lane_Text[1].Opacity = 255;
                                    TJAPlayer3.Tx.Lane_Text[2].Opacity = 255;
                                }
                                else if (this.stBranch[i].ct分岐アニメ進行.n現在の値 >= 150 && this.stBranch[i].ct分岐アニメ進行.n現在の値 < 210)
                                {
                                    this.stBranch[i].nY = ((this.stBranch[i].ct分岐アニメ進行.n現在の値 - 150) / 2);
                                    TJAPlayer3.Tx.Lane_Text[1].t2D描画(TJAPlayer3.app.Device, TJAPlayer3.Skin.Game_Lane_Background_X[i], TJAPlayer3.Skin.Game_Lane_Background_Y[i] - this.stBranch[i].nY);
                                    TJAPlayer3.Tx.Lane_Text[1].Opacity = this.stBranch[i].ct分岐アニメ進行.n現在の値 > 100 ? 0 : (255 - ((this.stBranch[i].ct分岐アニメ進行.n現在の値 * 0xff) / 100));
                                    TJAPlayer3.Tx.Lane_Text[0].t2D描画(TJAPlayer3.app.Device, TJAPlayer3.Skin.Game_Lane_Background_X[i], (TJAPlayer3.Skin.Game_Lane_Background_Y[i] + 30) - this.stBranch[i].nY);
                                }
                                else if (this.stBranch[i].ct分岐アニメ進行.n現在の値 >= 210)
                                {
                                    TJAPlayer3.Tx.Lane_Text[0].Opacity = 255;
                                    TJAPlayer3.Tx.Lane_Text[0].t2D描画(TJAPlayer3.app.Device, TJAPlayer3.Skin.Game_Lane_Background_X[i], TJAPlayer3.Skin.Game_Lane_Background_Y[i]);
                                }
                            }
                            if (this.stBranch[i].nBefore == CDTX.ECourse.eMaster && this.stBranch[i].nAfter == CDTX.ECourse.eExpert)
                            {
                                TJAPlayer3.Tx.Lane_Text[0].Opacity = 255;
                                TJAPlayer3.Tx.Lane_Text[1].Opacity = 255;
                                TJAPlayer3.Tx.Lane_Text[2].Opacity = 255;

                                TJAPlayer3.Tx.Lane_Text[2].Opacity = this.stBranch[i].ct分岐アニメ進行.n現在の値 > 100 ? 0 : (255 - ((this.stBranch[i].ct分岐アニメ進行.n現在の値 * 0xff) / 60));
                                if (this.stBranch[i].ct分岐アニメ進行.n現在の値 < 60)
                                {
                                    this.stBranch[i].nY = this.stBranch[i].ct分岐アニメ進行.n現在の値 / 2;
                                    TJAPlayer3.Tx.Lane_Text[2].t2D描画(TJAPlayer3.app.Device, TJAPlayer3.Skin.Game_Lane_Background_X[i], TJAPlayer3.Skin.Game_Lane_Background_Y[i] - this.stBranch[i].nY);
                                    TJAPlayer3.Tx.Lane_Text[1].t2D描画(TJAPlayer3.app.Device, TJAPlayer3.Skin.Game_Lane_Background_X[i], (TJAPlayer3.Skin.Game_Lane_Background_Y[i] + 30) - this.stBranch[i].nY);
                                }
                                else
                                {
                                    TJAPlayer3.Tx.Lane_Text[1].t2D描画(TJAPlayer3.app.Device, TJAPlayer3.Skin.Game_Lane_Background_X[i], TJAPlayer3.Skin.Game_Lane_Background_Y[i]);
                                }

                            }
                            #endregion
                        }
                    }
                    else
                    {
                        if (this.stBranch[i].nY座標 == 21)
                        {
                            this.stBranch[i].nY座標 = 0;
                        }

                        if (this.stBranch[i].nY座標 == 0)
                        {
                            switch (TJAPlayer3.stage演奏ドラム画面.nレーン用表示コース[0])
                            {
                                case CDTX.ECourse.eNormal:
                                    TJAPlayer3.Tx.Lane_Text[0].Opacity = 255;
                                    TJAPlayer3.Tx.Lane_Text[0].t2D描画(TJAPlayer3.app.Device, TJAPlayer3.Skin.Game_Lane_Background_X[i], TJAPlayer3.Skin.Game_Lane_Background_Y[i]);
                                    break;
                                case CDTX.ECourse.eExpert:
                                    TJAPlayer3.Tx.Lane_Text[1].Opacity = 255;
                                    TJAPlayer3.Tx.Lane_Text[1].t2D描画(TJAPlayer3.app.Device, TJAPlayer3.Skin.Game_Lane_Background_X[i], TJAPlayer3.Skin.Game_Lane_Background_Y[i]);
                                    break;
                                case CDTX.ECourse.eMaster:
                                    TJAPlayer3.Tx.Lane_Text[2].Opacity = 255;
                                    TJAPlayer3.Tx.Lane_Text[2].t2D描画(TJAPlayer3.app.Device, TJAPlayer3.Skin.Game_Lane_Background_X[i], TJAPlayer3.Skin.Game_Lane_Background_Y[i]);
                                    break;
                            }
                        }


                        if (this.stBranch[i].nY座標 != 0)
                        {
                            #region[ 普通譜面_レベルアップ ]
                            //普通→玄人
                            if (this.stBranch[i].nBefore == CDTX.ECourse.eNormal && this.stBranch[i].nAfter == CDTX.ECourse.eExpert)
                            {
                                TJAPlayer3.Tx.Lane_Text[0].t2D描画(TJAPlayer3.app.Device, TJAPlayer3.Skin.Game_Lane_Background_X[i], TJAPlayer3.Skin.Game_Lane_Background_Y[i] - this.stBranch[i].nY座標);
                                TJAPlayer3.Tx.Lane_Text[1].t2D描画(TJAPlayer3.app.Device, TJAPlayer3.Skin.Game_Lane_Background_X[i], TJAPlayer3.Skin.Game_Lane_Background_Y[i] + 20 - this.stBranch[i].nY座標);
                                TJAPlayer3.Tx.Lane_Text[0].Opacity = this.stBranch[i].nBranchレイヤー透明度;
                            }
                            //普通→達人
                            if (this.stBranch[i].nBefore == CDTX.ECourse.eNormal && this.stBranch[i].nAfter == CDTX.ECourse.eMaster)
                            {
                                TJAPlayer3.Tx.Lane_Text[0].t2D描画(TJAPlayer3.app.Device, TJAPlayer3.Skin.Game_Lane_Background_X[i], TJAPlayer3.Skin.Game_Lane_Background_Y[i] - this.stBranch[i].nY座標);
                                TJAPlayer3.Tx.Lane_Text[2].t2D描画(TJAPlayer3.app.Device, TJAPlayer3.Skin.Game_Lane_Background_X[i], TJAPlayer3.Skin.Game_Lane_Background_Y[i] + 20 - this.stBranch[i].nY座標);
                                TJAPlayer3.Tx.Lane_Text[0].Opacity = this.stBranch[i].nBranchレイヤー透明度;
                            }
                            #endregion
                            #region[ 玄人譜面_レベルアップ ]
                            //玄人→達人
                            if (this.stBranch[i].nBefore == CDTX.ECourse.eExpert && this.stBranch[i].nAfter == CDTX.ECourse.eMaster)
                            {
                                TJAPlayer3.Tx.Lane_Text[1].t2D描画(TJAPlayer3.app.Device, TJAPlayer3.Skin.Game_Lane_Background_X[i], TJAPlayer3.Skin.Game_Lane_Background_Y[i] - this.stBranch[i].nY座標);
                                TJAPlayer3.Tx.Lane_Text[2].t2D描画(TJAPlayer3.app.Device, TJAPlayer3.Skin.Game_Lane_Background_X[i], TJAPlayer3.Skin.Game_Lane_Background_Y[i] + 20 - this.stBranch[i].nY座標);
                                TJAPlayer3.Tx.Lane_Text[1].Opacity = this.stBranch[i].nBranchレイヤー透明度;
                            }
                            #endregion
                            #region[ 玄人譜面_レベルダウン ]
                            if (this.stBranch[i].nBefore == CDTX.ECourse.eExpert && this.stBranch[i].nAfter == CDTX.ECourse.eNormal)
                            {
                                TJAPlayer3.Tx.Lane_Text[1].t2D描画(TJAPlayer3.app.Device, TJAPlayer3.Skin.Game_Lane_Background_X[i], TJAPlayer3.Skin.Game_Lane_Background_Y[i] + this.stBranch[i].nY座標);
                                TJAPlayer3.Tx.Lane_Text[0].t2D描画(TJAPlayer3.app.Device, TJAPlayer3.Skin.Game_Lane_Background_X[i], TJAPlayer3.Skin.Game_Lane_Background_Y[i] - 24 + this.stBranch[i].nY座標);
                                TJAPlayer3.Tx.Lane_Text[1].Opacity = this.stBranch[i].nBranchレイヤー透明度;
                            }
                            #endregion
                            #region[ 達人譜面_レベルダウン ]
                            if (this.stBranch[i].nBefore == CDTX.ECourse.eMaster && this.stBranch[i].nAfter == CDTX.ECourse.eNormal)
                            {
                                TJAPlayer3.Tx.Lane_Text[2].t2D描画(TJAPlayer3.app.Device, TJAPlayer3.Skin.Game_Lane_Background_X[i], TJAPlayer3.Skin.Game_Lane_Background_Y[i] + this.stBranch[i].nY座標);
                                TJAPlayer3.Tx.Lane_Text[0].t2D描画(TJAPlayer3.app.Device, TJAPlayer3.Skin.Game_Lane_Background_X[i], TJAPlayer3.Skin.Game_Lane_Background_Y[i] - 24 + this.stBranch[i].nY座標);
                                TJAPlayer3.Tx.Lane_Text[2].Opacity = this.stBranch[i].nBranchレイヤー透明度;
                            }
                            if (this.stBranch[i].nBefore == CDTX.ECourse.eMaster && this.stBranch[i].nAfter == CDTX.ECourse.eExpert)
                            {
                                TJAPlayer3.Tx.Lane_Text[2].t2D描画(TJAPlayer3.app.Device, TJAPlayer3.Skin.Game_Lane_Background_X[i], TJAPlayer3.Skin.Game_Lane_Background_Y[i] + this.stBranch[i].nY座標);
                                TJAPlayer3.Tx.Lane_Text[1].t2D描画(TJAPlayer3.app.Device, TJAPlayer3.Skin.Game_Lane_Background_X[i], TJAPlayer3.Skin.Game_Lane_Background_Y[i] - 24 + this.stBranch[i].nY座標);
                                TJAPlayer3.Tx.Lane_Text[2].Opacity = this.stBranch[i].nBranchレイヤー透明度;
                            }
                            #endregion
                        }
                    }

                }
            }



            if (TJAPlayer3.Tx.Lane_Background_Sub != null)
            {
                TJAPlayer3.Tx.Lane_Background_Sub.t2D描画(TJAPlayer3.app.Device, TJAPlayer3.Skin.Game_Lane_Background_X[0], 134 + TJAPlayer3.Skin.Game_Lane_Background_Y[0]);
                if (TJAPlayer3.stage演奏ドラム画面.bDoublePlay)
                {
                    TJAPlayer3.Tx.Lane_Background_Sub.t2D描画(TJAPlayer3.app.Device, TJAPlayer3.Skin.Game_Lane_Background_X[1], 134 + TJAPlayer3.Skin.Game_Lane_Background_Y[1]);
                }
            }


            TJAPlayer3.stage演奏ドラム画面.actTaikoLaneFlash.On進行描画();



            TJAPlayer3.Tx.Taiko_Frame[0]?.t2D描画(TJAPlayer3.app.Device, TJAPlayer3.Skin.Game_Taiko_Frame_X[0], TJAPlayer3.Skin.Game_Taiko_Frame_Y[0]);

            if (TJAPlayer3.stage演奏ドラム画面.bDoublePlay)
            {
                TJAPlayer3.Tx.Taiko_Frame[1]?.t2D描画(TJAPlayer3.app.Device, TJAPlayer3.Skin.Game_Taiko_Frame_X[1], TJAPlayer3.Skin.Game_Taiko_Frame_Y[1]);
            }
            var nTime = (long)(CSound管理.rc演奏用タイマ.n現在時刻 * (((double)TJAPlayer3.ConfigIni.n演奏速度) / 20.0));



            if (this.n総移動時間 != -1)
            {
                if (n移動方向 == 1)
                {
                    TJAPlayer3.Skin.Game_Lane_Field_X[0] = this.n移動開始X + (int)((((int)nTime - this.n移動開始時刻) / (double)(this.n総移動時間)) * this.n移動距離px);
                    TJAPlayer3.Skin.Game_Lane_Field_Y[0] = this.n移動開始Y + (int)((((int)nTime - this.n移動開始時刻) / (double)(this.n総移動時間)) * this.n移動距離Ypx);
                    TJAPlayer3.stage演奏ドラム画面.FlyingNotes.StartPointX[0] = this.n移動開始X + (int)((((int)nTime - this.n移動開始時刻) / (double)(this.n総移動時間)) * this.n移動距離px);
                    TJAPlayer3.stage演奏ドラム画面.FlyingNotes.StartPointY[0] = this.n移動開始Y + (int)((((int)nTime - this.n移動開始時刻) / (double)(this.n総移動時間)) * this.n移動距離Ypx) + (130 / 2);
                }
                else
                {
                    TJAPlayer3.Skin.Game_Lane_Field_X[0] = this.n移動開始X - (int)((((int)nTime - this.n移動開始時刻) / (double)(this.n総移動時間)) * this.n移動距離px);
                    TJAPlayer3.Skin.Game_Lane_Field_Y[0] = this.n移動開始Y - (int)((((int)nTime - this.n移動開始時刻) / (double)(this.n総移動時間)) * this.n移動距離Ypx);
                    TJAPlayer3.stage演奏ドラム画面.FlyingNotes.StartPointX[0] = this.n移動開始X - (int)((((int)nTime - this.n移動開始時刻) / (double)(this.n総移動時間)) * this.n移動距離px);
                    TJAPlayer3.stage演奏ドラム画面.FlyingNotes.StartPointY[0] = this.n移動開始Y - (int)((((int)nTime - this.n移動開始時刻) / (double)(this.n総移動時間)) * this.n移動距離Ypx) + (130 / 2);
                }

                if (((int)nTime) > this.n移動開始時刻 + this.n総移動時間)
                {
                    this.n総移動時間 = -1;
                }
            }
            if (this.n総移動時間2 != -1)
            {
                if (n移動方向2 == 1)
                {
                    TJAPlayer3.Skin.Game_Lane_Field_X[1] = this.n移動開始X2 + (int)((((int)nTime - this.n移動開始時刻2) / (double)(this.n総移動時間2)) * this.n移動距離px2);
                    TJAPlayer3.Skin.Game_Lane_Field_Y[1] = this.n移動開始Y2 + (int)((((int)nTime - this.n移動開始時刻2) / (double)(this.n総移動時間2)) * this.n移動距離Ypx2);
                    TJAPlayer3.stage演奏ドラム画面.FlyingNotes.StartPointX[1] = this.n移動開始X2 + (int)((((int)nTime - this.n移動開始時刻2) / (double)(this.n総移動時間2)) * this.n移動距離px2);
                    TJAPlayer3.stage演奏ドラム画面.FlyingNotes.StartPointY[1] = this.n移動開始Y2 + (int)((((int)nTime - this.n移動開始時刻2) / (double)(this.n総移動時間2)) * this.n移動距離Ypx2) + (130 / 2);
                }
                else
                {
                    TJAPlayer3.Skin.Game_Lane_Field_X[1] = this.n移動開始X2 - (int)((((int)nTime - this.n移動開始時刻2) / (double)(this.n総移動時間2)) * this.n移動距離px2);
                    TJAPlayer3.Skin.Game_Lane_Field_Y[1] = this.n移動開始Y2 - (int)((((int)nTime - this.n移動開始時刻2) / (double)(this.n総移動時間2)) * this.n移動距離Ypx2);
                    TJAPlayer3.stage演奏ドラム画面.FlyingNotes.StartPointX[1] = this.n移動開始X2 - (int)((((int)nTime - this.n移動開始時刻2) / (double)(this.n総移動時間2)) * this.n移動距離px2);
                    TJAPlayer3.stage演奏ドラム画面.FlyingNotes.StartPointY[1] = this.n移動開始Y2 - (int)((((int)nTime - this.n移動開始時刻2) / (double)(this.n総移動時間2)) * this.n移動距離Ypx2) + (130 / 2);
                }


                if (((int)nTime) > this.n移動開始時刻2 + this.n総移動時間2)
                {
                    this.n総移動時間2 = -1;
                }
            }




            if (TJAPlayer3.ConfigIni.bAVI有効 && TJAPlayer3.DTX.listAVI.Count > 0)
            {
                if (TJAPlayer3.Tx.Lane_Background_Main != null) TJAPlayer3.Tx.Lane_Background_Main.Opacity = TJAPlayer3.ConfigIni.nBGAlpha;
                if(TJAPlayer3.Tx.Lane_Background_Sub != null) TJAPlayer3.Tx.Lane_Background_Sub.Opacity = TJAPlayer3.ConfigIni.nBGAlpha;
                if (TJAPlayer3.Tx.Lane_Background_GoGo != null) TJAPlayer3.Tx.Lane_Background_GoGo.Opacity = TJAPlayer3.ConfigIni.nBGAlpha;
            }
            else
            {
                if (TJAPlayer3.Tx.Lane_Background_Main != null) TJAPlayer3.Tx.Lane_Background_Main.Opacity = 255;
                if (TJAPlayer3.Tx.Lane_Background_Sub != null) TJAPlayer3.Tx.Lane_Background_Sub.Opacity = 255;
                if (TJAPlayer3.Tx.Lane_Background_GoGo != null) TJAPlayer3.Tx.Lane_Background_GoGo.Opacity = 255;
            }

            return base.On進行描画();
        }

        public void ゴーゴー炎()
        {
            //判定枠
            if (TJAPlayer3.Tx.Judge_Frame != null)
            {
                int nJudgeX = TJAPlayer3.Skin.Game_Lane_Field_X[0] - (130 / 2); //元の値は349なんだけど...
                int nJudgeY = TJAPlayer3.Skin.Game_Lane_Field_Y[0]; //元の値は349なんだけど...
                TJAPlayer3.Tx.Judge_Frame.b加算合成 = TJAPlayer3.Skin.Game_JudgeFrame_AddBlend;
                TJAPlayer3.Tx.Judge_Frame.t2D描画(TJAPlayer3.app.Device, nJudgeX, nJudgeY, new Rectangle(0, 0, 130, 130));

                if (TJAPlayer3.stage演奏ドラム画面.bDoublePlay)
                    TJAPlayer3.Tx.Judge_Frame.t2D描画(TJAPlayer3.app.Device, nJudgeX, nJudgeY + 176, new Rectangle(0, 0, 130, 130));
            }


            #region[ ゴーゴー炎 ]
            for (int i = 0; i < TJAPlayer3.ConfigIni.nPlayerCount; i++)
            {
                if (TJAPlayer3.stage演奏ドラム画面.bIsGOGOTIME[i])
                {
                    this.ctゴーゴー炎.t進行Loop();

                    if (TJAPlayer3.Tx.Effects_Fire != null)
                    {
                        float f倍率 = 1.0f;

                        float[] ar倍率 = new float[] { 0.8f, 1.2f, 1.7f, 2.5f, 2.3f, 2.2f, 2.0f, 1.8f, 1.7f, 1.6f, 1.6f, 1.5f, 1.5f, 1.4f, 1.3f, 1.2f, 1.1f, 1.0f };

                        f倍率 = ar倍率[this.ctゴーゴー.n現在の値];

                        Matrix4 mat = Matrix4.Identity;
                        mat *= Matrix4.CreateScale(f倍率, f倍率, 1.0f);
                        mat *= Matrix4.CreateTranslation(TJAPlayer3.Skin.Game_Lane_Field_X[i] - GameWindowSize.Width / 2.0f, -(TJAPlayer3.Skin.Game_Lane_Field_Y[i] + (130 / 2) - GameWindowSize.Height / 2.0f), 0f);

                        if (this.ctゴーゴー.b終了値に達した)
                        {
                            TJAPlayer3.Tx.Effects_Fire.t2D描画(TJAPlayer3.app.Device, TJAPlayer3.Skin.Game_Lane_Field_X[i] - 180, TJAPlayer3.Skin.Game_Lane_Field_Y[i] + (130 / 2) - (TJAPlayer3.Tx.Effects_Fire.szテクスチャサイズ.Height / 2), new Rectangle(360 * (this.ctゴーゴー炎.n現在の値), 0, 360, 370));
                        }
                        else
                        {
                            TJAPlayer3.Tx.Effects_Fire.t3D描画(TJAPlayer3.app.Device, mat, new Rectangle(360 * (this.ctゴーゴー炎.n現在の値), 0, 360, 370));
                        }
                    }
                }
            }
            #endregion
            for (int i = 0; i < TJAPlayer3.ConfigIni.nPlayerCount; i++)
            {
                if (!this.st状態[i].ct進行.b停止中)
                {
                    this.st状態[i].ct進行.t進行();
                    if (this.st状態[i].ct進行.b終了値に達した)
                    {
                        this.st状態[i].ct進行.t停止();
                    }
                    {
                        int n = this.st状態[i].nIsBig == 1 ? 520 : 0;

                        switch (st状態[i].judge)
                        {
                            case E判定.Perfect:
                            case E判定.Great:
                            case E判定.Auto:
                                if (this.st状態[i].nIsBig == 1 && TJAPlayer3.Tx.Effects_Hit_Great_Big[this.st状態[i].ct進行.n現在の値] != null)
                                    TJAPlayer3.Tx.Effects_Hit_Great_Big[this.st状態[i].ct進行.n現在の値].t2D描画(TJAPlayer3.app.Device, TJAPlayer3.Skin.Game_Lane_Field_X[0] - TJAPlayer3.Tx.Effects_Hit_Great_Big[0].szテクスチャサイズ.Width / 2, TJAPlayer3.Skin.Game_Lane_Field_Y[i] + (130 / 2) - TJAPlayer3.Tx.Effects_Hit_Great_Big[0].szテクスチャサイズ.Width / 2);
                                else if (TJAPlayer3.Tx.Effects_Hit_Great[this.st状態[i].ct進行.n現在の値] != null)
                                    TJAPlayer3.Tx.Effects_Hit_Great[this.st状態[i].ct進行.n現在の値].t2D描画(TJAPlayer3.app.Device, TJAPlayer3.Skin.Game_Lane_Field_X[0] - TJAPlayer3.Tx.Effects_Hit_Great[0].szテクスチャサイズ.Width / 2, TJAPlayer3.Skin.Game_Lane_Field_Y[i] + (130 / 2) - TJAPlayer3.Tx.Effects_Hit_Great[0].szテクスチャサイズ.Width / 2);
                                break;

                            case E判定.Good:
                                if (this.st状態[i].nIsBig == 1 && TJAPlayer3.Tx.Effects_Hit_Good_Big[this.st状態[i].ct進行.n現在の値] != null)
                                    TJAPlayer3.Tx.Effects_Hit_Good_Big[this.st状態[i].ct進行.n現在の値].t2D描画(TJAPlayer3.app.Device, TJAPlayer3.Skin.Game_Lane_Field_X[0] - TJAPlayer3.Tx.Effects_Hit_Good_Big[0].szテクスチャサイズ.Width / 2, TJAPlayer3.Skin.Game_Lane_Field_Y[i] + (130 / 2) - TJAPlayer3.Tx.Effects_Hit_Good_Big[0].szテクスチャサイズ.Width / 2);
                                else if (TJAPlayer3.Tx.Effects_Hit_Good[this.st状態[i].ct進行.n現在の値] != null)
                                    TJAPlayer3.Tx.Effects_Hit_Good[this.st状態[i].ct進行.n現在の値].t2D描画(TJAPlayer3.app.Device, TJAPlayer3.Skin.Game_Lane_Field_X[0] - TJAPlayer3.Tx.Effects_Hit_Good[0].szテクスチャサイズ.Width / 2, TJAPlayer3.Skin.Game_Lane_Field_Y[i] + (130 / 2) - TJAPlayer3.Tx.Effects_Hit_Good[0].szテクスチャサイズ.Width / 2);
                                break;

                            case E判定.Miss:
                            case E判定.Bad:
                                break;
                        }
                    }
                }
            }


        }

        public virtual void Start(int nLane, E判定 judge, bool b両手入力, int nPlayer)
        {
            //2017.08.15 kairera0467 排他なので番地をそのまま各レーンの状態として扱う

            {
                this.st状態[nPlayer].ct進行 = new CCounter(0, 14, 20, TJAPlayer3.Timer);
                this.st状態[nPlayer].judge = judge;
                this.st状態[nPlayer].nPlayer = nPlayer;

                switch (nLane)
                {
                    case 0x11:
                    case 0x12:
                        this.st状態[nPlayer].nIsBig = 0;
                        break;
                    case 0x13:
                    case 0x14:
                    case 0x1A:
                    case 0x1B:
                        {
                            if (b両手入力)
                                this.st状態[nPlayer].nIsBig = 1;
                            else
                                this.st状態[nPlayer].nIsBig = 0;
                        }
                        break;
                }
            }
        }


        public void GOGOSTART()
        {
            this.ctゴーゴー = new CCounter(0, 17, 18, TJAPlayer3.Timer);
            if(TJAPlayer3.ConfigIni.nPlayerCount == 1 && TJAPlayer3.stage選曲.n確定された曲の難易度 != (int)Difficulty.Dan) TJAPlayer3.stage演奏ドラム画面.GoGoSplash.StartSplash();
        }


        public void t分岐レイヤー_コース変化(CDTX.ECourse n現在, CDTX.ECourse n次回, int nPlayer)
        {
            if (n現在 == n次回)
            {
                return;
            }
            this.stBranch[nPlayer].ct分岐アニメ進行 = new CCounter(0, 300, 2, TJAPlayer3.Timer);

            this.stBranch[nPlayer].nBranchレイヤー透明度 = 6;
            this.stBranch[nPlayer].nY座標 = 1;

            this.stBranch[nPlayer].nBefore = n現在;
            this.stBranch[nPlayer].nAfter = n次回;

            TJAPlayer3.stage演奏ドラム画面.actLane.t分岐レイヤー_コース変化(n現在, n次回, nPlayer);
        }

        public void t判定枠移動XY(double db移動時間, int n移動px, int n移動Ypx, int n移動方向)
        {
            this.n移動開始時刻 = (int)(CSound管理.rc演奏用タイマ.n現在時刻 * (((double)TJAPlayer3.ConfigIni.n演奏速度) / 20.0));
            this.n移動開始X = TJAPlayer3.Skin.Game_Lane_Field_X[0];
            this.n移動開始Y = TJAPlayer3.Skin.Game_Lane_Field_Y[0];
            this.n総移動時間 = (int)(db移動時間 * 1000);
            this.n移動方向 = n移動方向;
            this.n移動距離px = n移動px;
            this.n移動距離Ypx = n移動Ypx;
        }

        public void t判定枠移動XY2(double db移動時間, int n移動px, int n移動Ypx, int n移動方向)
        {
            this.n移動開始時刻2 = (int)(CSound管理.rc演奏用タイマ.n現在時刻 * (((double)TJAPlayer3.ConfigIni.n演奏速度) / 20.0));
            this.n移動開始X2 = TJAPlayer3.Skin.Game_Lane_Field_X[1];
            this.n移動開始Y2 = TJAPlayer3.Skin.Game_Lane_Field_Y[1];
            this.n総移動時間2 = (int)(db移動時間 * 1000);
            this.n移動方向2 = n移動方向;
            this.n移動距離px2 = n移動px;
            this.n移動距離Ypx2 = n移動Ypx;
        }
        #region[ private ]

        protected STSTATUS[] st状態 = new STSTATUS[4];

        protected struct STSTATUS
        {
            public bool b使用中;
            public CCounter ct進行;
            public E判定 judge;
            public int nIsBig;
            public int n透明度;
            public int nPlayer;
        }
        private CCounter ctゴーゴー;
        private CCounter ctゴーゴー炎;



        public STBRANCH[] stBranch = new STBRANCH[4];
        public struct STBRANCH
        {
            public CCounter ct分岐アニメ進行;
            public CDTX.ECourse nBefore;
            public CDTX.ECourse nAfter;

            public long nフラッシュ制御タイマ;
            public int nBranchレイヤー透明度;
            public int nBranch文字透明度;
            public int nY座標;
            public int nY;
        }


        private int n総移動時間;
        private int n移動開始X;
        private int n移動開始Y;
        private int n移動開始時刻;
        private int n移動距離px;
        private int n移動距離Ypx;
        private int n移動方向;
        private int n総移動時間2;
        private int n移動開始X2;
        private int n移動開始Y2;
        private int n移動開始時刻2;
        private int n移動距離px2;
        private int n移動距離Ypx2;
        private int n移動方向2;

        private int[] nDefaultJudgePos = new int[2];


        //-----------------
        #endregion
    }
}
