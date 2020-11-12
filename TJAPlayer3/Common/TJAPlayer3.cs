using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Imaging;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.IO;
using System.Threading;
using FDK;
using System.Reflection;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

using Rectangle = System.Drawing.Rectangle;
using Point = System.Drawing.Point;
using Color = System.Drawing.Color;

namespace TJAPlayer3
{
	internal class TJAPlayer3 : Game
	{
		// プロパティ
		#region [ properties ]
		public static readonly string VERSION = Assembly.GetExecutingAssembly().GetName().Version.ToString().Substring(0, Assembly.GetExecutingAssembly().GetName().Version.ToString().Length - 2);

		public static TJAPlayer3 app
		{
			get;
			private set;
		}
		public static C文字コンソール act文字コンソール
		{
			get;
			private set;
		}

		public static bool bコンパクトモード
		{
			get;
			private set;
		}

		public static CConfigIni ConfigIni
		{
			get;
			private set;
		}
		public static CDTX DTX
		{
			get
			{
				return dtx[0];
			}
			set
			{
				if ((dtx[0] != null) && (app != null))
				{
					dtx[0].On非活性化();
					app.listトップレベルActivities.Remove(dtx[0]);
				}
				dtx[0] = value;
				if ((dtx[0] != null) && (app != null))
				{
					app.listトップレベルActivities.Add(dtx[0]);
				}
			}
		}
		public static CDTX DTX_2P
		{
			get
			{
				return dtx[1];
			}
			set
			{
				if ((dtx[1] != null) && (app != null))
				{
					dtx[1].On非活性化();
					app.listトップレベルActivities.Remove(dtx[1]);
				}
				dtx[1] = value;
				if ((dtx[1] != null) && (app != null))
				{
					app.listトップレベルActivities.Add(dtx[1]);
				}
			}
		}

		public static bool IsPerformingCalibration;

		public static CFPS FPS
		{
			get;
			private set;
		}

		public static CInput管理 Input管理
		{
			get;
			private set;
		}
		#region [ 入力範囲ms ]
		public static int nPerfect範囲ms
		{
			get
			{
				if (stage選曲.r確定された曲 != null)
				{
					C曲リストノード c曲リストノード = stage選曲.r確定された曲.r親ノード;
					if (((c曲リストノード != null) && (c曲リストノード.eノード種別 == C曲リストノード.Eノード種別.BOX)) && (c曲リストノード.nPerfect範囲ms >= 0))
					{
						return c曲リストノード.nPerfect範囲ms;
					}
				}
				return ConfigIni.nヒット範囲ms.Perfect;
			}
		}
		public static int nGreat範囲ms
		{
			get
			{
				if (stage選曲.r確定された曲 != null)
				{
					C曲リストノード c曲リストノード = stage選曲.r確定された曲.r親ノード;
					if (((c曲リストノード != null) && (c曲リストノード.eノード種別 == C曲リストノード.Eノード種別.BOX)) && (c曲リストノード.nGreat範囲ms >= 0))
					{
						return c曲リストノード.nGreat範囲ms;
					}
				}
				return ConfigIni.nヒット範囲ms.Great;
			}
		}
		public static int nGood範囲ms
		{
			get
			{
				if (stage選曲.r確定された曲 != null)
				{
					C曲リストノード c曲リストノード = stage選曲.r確定された曲.r親ノード;
					if (((c曲リストノード != null) && (c曲リストノード.eノード種別 == C曲リストノード.Eノード種別.BOX)) && (c曲リストノード.nGood範囲ms >= 0))
					{
						return c曲リストノード.nGood範囲ms;
					}
				}
				return ConfigIni.nヒット範囲ms.Good;
			}
		}
		public static int nPoor範囲ms
		{
			get
			{
				if (stage選曲.r確定された曲 != null)
				{
					C曲リストノード c曲リストノード = stage選曲.r確定された曲.r親ノード;
					if (((c曲リストノード != null) && (c曲リストノード.eノード種別 == C曲リストノード.Eノード種別.BOX)) && (c曲リストノード.nPoor範囲ms >= 0))
					{
						return c曲リストノード.nPoor範囲ms;
					}
				}
				return ConfigIni.nヒット範囲ms.Poor;
			}
		}
		#endregion
		public static CPad Pad
		{
			get;
			private set;
		}
		public static Random Random
		{
			get;
			private set;
		}
		public static CSkin Skin
		{
			get;
			private set;
		}
		public static CSongs管理 Songs管理
		{
			get;
			set;    // 2012.1.26 yyagi private解除 CStage起動でのdesirialize読み込みのため
		}
		public static CEnumSongs EnumSongs
		{
			get;
			private set;
		}
		public static CActEnumSongs actEnumSongs
		{
			get;
			private set;
		}
		public static CActScanningLoudness actScanningLoudness
		{
			get;
			private set;
		}

		public static CSound管理 Sound管理
		{
			get;
			private set;
		}

		public static SongGainController SongGainController
		{
			get;
			private set;
		}

		public static SoundGroupLevelController SoundGroupLevelController
		{
			get;
			private set;
		}

		public static CStage起動 stage起動
		{
			get;
			private set;
		}
		public static CStageタイトル stageタイトル
		{
			get;
			private set;
		}
		//		public static CStageオプション stageオプション
		//		{ 
		//			get;
		//			private set;
		//		}
		public static CStageコンフィグ stageコンフィグ
		{
			get;
			private set;
		}
		public static CStage選曲 stage選曲
		{
			get;
			private set;
		}
		public static CStage曲読み込み stage曲読み込み
		{
			get;
			private set;
		}
		public static CStage演奏ドラム画面 stage演奏ドラム画面
		{
			get;
			private set;
		}
		public static CStage結果 stage結果
		{
			get;
			private set;
		}
		public static CStageChangeSkin stageChangeSkin
		{
			get;
			private set;
		}
		public static CStage終了 stage終了
		{
			get;
			private set;
		}
		public static CStage r現在のステージ = null;
		public static CStage r直前のステージ = null;
		public static string strEXEのあるフォルダ
		{
			get;
			private set;
		}
		public static string strコンパクトモードファイル
		{
			get;
			private set;
		}

		public static CTimer Timer
		{
			get;
			private set;
		}

		public bool bApplicationActive
		{
			get
			{
				return this.Focused;
			}
		}
		public bool b次のタイミングで垂直帰線同期切り替えを行う
		{
			get;
			set;
		}
		public bool b次のタイミングで全画面_ウィンドウ切り替えを行う
		{
			get;
			set;
		}
		public int Device
		{
			get
			{
				return 0;
			}
		}
		public IntPtr WindowHandle                  // 2012.10.24 yyagi; to add ASIO support
		{
			get { return base.WindowInfo.Handle; }
		}
		public static CDTXVmode DTXVmode			// #28821 2014.1.23 yyagi
		{
			get;
			set;
		}

		#endregion

		// コンストラクタ

		public TJAPlayer3()
		{
			TJAPlayer3.app = this;
			this.t起動処理();
		}


		// メソッド

		public void t全画面_ウィンドウモード切り替え()
		{
			if ((ConfigIni != null))
			{
				if (ConfigIni.bウィンドウモード == false)   // #23510 2010.10.27 yyagi: backup current window size before going fullscreen mode
				{
					base.WindowState = WindowState.Fullscreen;
				}

				if (ConfigIni.bウィンドウモード == true)    // #23510 2010.10.27 yyagi: to resume window size from backuped value
				{
					base.WindowState = WindowState.Normal;
				}
			}
		}

		#region [ #24609 リザルト画像をpngで保存する ]		// #24609 2011.3.14 yyagi; to save result screen in case BestRank or HiSkill.
		/// <summary>
		/// リザルト画像のキャプチャと保存。
		/// </summary>
		/// <param name="strFilename">保存するファイル名(フルパス)</param>
		public bool SaveResultScreen(string strFullPath)
		{
			string strSavePath = Path.GetDirectoryName(strFullPath);
			if (!Directory.Exists(strSavePath))
			{
				try
				{
					Directory.CreateDirectory(strSavePath);
				}
				catch (Exception e)
				{
					Trace.TraceError(e.ToString());
					Trace.TraceError("例外が発生しましたが処理を継続します。 (0bfe6bff-2a56-4df4-9333-2df26d9b765b)");
					return false;
				}
			}

			if (GraphicsContext.CurrentContext == null)
				throw new GraphicsContextException();

			Bitmap bmp = new Bitmap(ClientSize.Width, ClientSize.Height);
			System.Drawing.Imaging.BitmapData data =
				bmp.LockBits(ClientRectangle, System.Drawing.Imaging.ImageLockMode.WriteOnly,
				System.Drawing.Imaging.PixelFormat.Format24bppRgb);
			GL.ReadPixels(0, 0, ClientSize.Width, ClientSize.Height,
				OpenTK.Graphics.OpenGL.PixelFormat.Bgr, PixelType.UnsignedByte, data.Scan0);
			bmp.UnlockBits(data);

			bmp.RotateFlip(RotateFlipType.RotateNoneFlipY);
			bmp.Save(strFullPath, ImageFormat.Png);
			bmp.Dispose();

			return true;
		}
		#endregion

		// Game 実装


		protected override void OnResize(EventArgs e)
		{
			base.OnResize(e);
			base.Width = ClientRectangle.Height * 16 / 9;
			base.Height = ClientRectangle.Height;
			GL.Viewport(ClientRectangle);
		}

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);
			if (this.listトップレベルActivities != null)
			{
				foreach (CActivity activity in this.listトップレベルActivities)
					activity.OnManagedリソースの作成();
			}
			if (ConfigIni.bウィンドウモード)
			{
				if (!this.bマウスカーソル表示中)
				{
					//Cursor.Show();
					this.bマウスカーソル表示中 = true;
				}
			}
			else if (this.bマウスカーソル表示中)
			{
				//Cursor.Hide();
				this.bマウスカーソル表示中 = false;
			}

			GL.ClearColor(Color.Black);
			GL.Disable(EnableCap.DepthTest);
			GL.Enable(EnableCap.Blend);
			GL.Enable(EnableCap.Texture2D);
			GL.Enable(EnableCap.AlphaTest);

			if (this.listトップレベルActivities != null)
			{
				foreach (CActivity activity in this.listトップレベルActivities)
					activity.OnUnmanagedリソースの作成();
			}
		}
		protected override void OnUnload(EventArgs e)
		{
			if (this.listトップレベルActivities != null)
			{
				foreach (CActivity activity in this.listトップレベルActivities)
					activity.OnUnmanagedリソースの解放();
			}
			CPowerManagement.tEnableMonitorSuspend();       // スリープ抑止状態を解除
			this.t終了処理();
			base.OnUnload(e);
		}
		protected override void OnUpdateFrame(FrameEventArgs e)
		{
			base.OnUpdateFrame(e);
		}
		//private Stopwatch rsw = new Stopwatch();
		protected override void OnRenderFrame(FrameEventArgs e)
		{
			//rsw.Reset();
			//rsw.Start();
			base.OnRenderFrame(e);

			Sound管理?.t再生中の処理をする();
			Timer?.t更新();
			CSound管理.rc演奏用タイマ?.t更新();
			Input管理?.tポーリング(this.bApplicationActive);
			FPS?.tカウンタ更新();

			if (this.Device == null)
				return;

			if (this.bApplicationActive)    // DTXMania本体起動中の本体/モニタの省電力モード移行を抑止
				CPowerManagement.tDisableMonitorSuspend();

			// #xxxxx 2013.4.8 yyagi; sleepの挿入位置を、EndScnene～Present間から、BeginScene前に移動。描画遅延を小さくするため。
			#region [ スリープ ]
			if (ConfigIni.nフレーム毎スリープms >= 0)            // #xxxxx 2011.11.27 yyagi
			{
				Thread.Sleep(ConfigIni.nフレーム毎スリープms);
			}
			#endregion

			GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

			Matrix4 modelView = Matrix4.LookAt(Vector3.UnitZ * 10, Vector3.Zero, Vector3.UnitY);
			GL.MatrixMode(MatrixMode.Modelview);
			GL.LoadMatrix(ref modelView);

			Matrix4 projection = Matrix4.CreatePerspectiveFieldOfView(MathHelper.PiOver4, (float)this.Width / (float)this.Height, 1.0f, 64.0f);
			GL.MatrixMode(MatrixMode.Projection);
			GL.LoadMatrix(ref projection);

			if (r現在のステージ != null)
			{
				this.n進行描画の戻り値 = (r現在のステージ != null) ? r現在のステージ.On進行描画() : 0;

				CScoreIni scoreIni = null;

				if (Control.IsKeyLocked(Keys.CapsLock))             // #30925 2013.3.11 yyagi; capslock=ON時は、EnumSongsしないようにして、起動負荷とASIOの音切れの関係を確認する
				{                                                       // → songs.db等の書き込み時だと音切れするっぽい
					actEnumSongs.On非活性化();
					EnumSongs.SongListEnumCompletelyDone();
					TJAPlayer3.stage選曲.bIsEnumeratingSongs = false;
				}
				#region [ 曲検索スレッドの起動/終了 ]					// ここに"Enumerating Songs..."表示を集約
				actEnumSongs.On進行描画();                          // "Enumerating Songs..."アイコンの描画
				switch (r現在のステージ.eステージID)
				{
					case CStage.Eステージ.タイトル:
					case CStage.Eステージ.コンフィグ:
					case CStage.Eステージ.選曲:
					case CStage.Eステージ.曲読み込み:
						if (EnumSongs != null)
						{
							#region [ (特定条件時) 曲検索スレッドの起動_開始 ]
							if (r現在のステージ.eステージID == CStage.Eステージ.タイトル &&
								 r直前のステージ.eステージID == CStage.Eステージ.起動 &&
								 this.n進行描画の戻り値 == (int)CStageタイトル.E戻り値.継続 &&
								 !EnumSongs.IsSongListEnumStarted)
							{
								actEnumSongs.On活性化();
								TJAPlayer3.stage選曲.bIsEnumeratingSongs = true;
								EnumSongs.Init(TJAPlayer3.Songs管理.listSongsDB, TJAPlayer3.Songs管理.nSongsDBから取得できたスコア数); // songs.db情報と、取得した曲数を、新インスタンスにも与える
								EnumSongs.StartEnumFromDisk();      // 曲検索スレッドの起動_開始
								if (TJAPlayer3.Songs管理.nSongsDBから取得できたスコア数 == 0)    // もし初回起動なら、検索スレッドのプライオリティをLowestでなくNormalにする
								{
									EnumSongs.ChangeEnumeratePriority(ThreadPriority.Normal);
								}
							}
							#endregion

							#region [ 曲検索の中断と再開 ]
							if (r現在のステージ.eステージID == CStage.Eステージ.選曲 && !EnumSongs.IsSongListEnumCompletelyDone)
							{
								switch (this.n進行描画の戻り値)
								{
									case 0:     // 何もない
												//if ( CDTXMania.stage選曲.bIsEnumeratingSongs )
										if (true)
										{
											EnumSongs.Resume();                     // #27060 2012.2.6 yyagi 中止していたバックグランド曲検索を再開
											EnumSongs.IsSlowdown = false;
										}
										else
										{
											// EnumSongs.Suspend();					// #27060 2012.3.2 yyagi #PREMOVIE再生中は曲検索を低速化
											EnumSongs.IsSlowdown = true;
										}
										actEnumSongs.On活性化();
										break;

									case 2:     // 曲決定
										EnumSongs.Suspend();                        // #27060 バックグラウンドの曲検索を一時停止
										actEnumSongs.On非活性化();
										break;
								}
							}
							#endregion

							#region [ 曲探索中断待ち待機 ]
							if (r現在のステージ.eステージID == CStage.Eステージ.曲読み込み && !EnumSongs.IsSongListEnumCompletelyDone &&
								EnumSongs.thDTXFileEnumerate != null)                           // #28700 2012.6.12 yyagi; at Compact mode, enumerating thread does not exist.
							{
								EnumSongs.WaitUntilSuspended();                                 // 念のため、曲検索が一時中断されるまで待機
							}
							#endregion

							#region [ 曲検索が完了したら、実際の曲リストに反映する ]
							// CStage選曲.On活性化() に回した方がいいかな？
							if (EnumSongs.IsSongListEnumerated)
							{
								actEnumSongs.On非活性化();
								TJAPlayer3.stage選曲.bIsEnumeratingSongs = false;

								bool bRemakeSongTitleBar = (r現在のステージ.eステージID == CStage.Eステージ.選曲) ? true : false;
								TJAPlayer3.stage選曲.Refresh(EnumSongs.Songs管理, bRemakeSongTitleBar);
								EnumSongs.SongListEnumCompletelyDone();
							}
							#endregion
						}
						break;
				}
				#endregion

				switch (r現在のステージ.eステージID)
				{
					case CStage.Eステージ.何もしない:
						break;

					case CStage.Eステージ.起動:
						#region [ *** ]
						//-----------------------------
						if (this.n進行描画の戻り値 != 0)
						{
							r現在のステージ.On非活性化();
							Trace.TraceInformation("----------------------");
							Trace.TraceInformation("■ タイトル");
							stageタイトル.On活性化();
							r直前のステージ = r現在のステージ;
							r現在のステージ = stageタイトル;

							this.tガベージコレクションを実行する();
						}
						//-----------------------------
						#endregion
						break;

					case CStage.Eステージ.タイトル:
						#region [ *** ]
						//-----------------------------
						switch (this.n進行描画の戻り値)
						{
							case (int)CStageタイトル.E戻り値.GAMESTART:
								#region [ 選曲処理へ ]
								//-----------------------------
								r現在のステージ.On非活性化();
								Trace.TraceInformation("----------------------");
								Trace.TraceInformation("■ 選曲");
								stage選曲.On活性化();
								r直前のステージ = r現在のステージ;
								r現在のステージ = stage選曲;
								//-----------------------------
								#endregion
								break;

							case (int)CStageタイトル.E戻り値.CONFIG:
								#region [ *** ]
								//-----------------------------
								r現在のステージ.On非活性化();
								Trace.TraceInformation("----------------------");
								Trace.TraceInformation("■ コンフィグ");
								stageコンフィグ.On活性化();
								r直前のステージ = r現在のステージ;
								r現在のステージ = stageコンフィグ;
								//-----------------------------
								#endregion
								break;

							case (int)CStageタイトル.E戻り値.EXIT:
								#region [ *** ]
								//-----------------------------
								r現在のステージ.On非活性化();
								Trace.TraceInformation("----------------------");
								Trace.TraceInformation("■ 終了");
								stage終了.On活性化();
								r直前のステージ = r現在のステージ;
								r現在のステージ = stage終了;
								//-----------------------------
								#endregion
								break;
						}

						//this.tガベージコレクションを実行する();		// #31980 2013.9.3 yyagi タイトル画面でだけ、毎フレームGCを実行して重くなっていた問題の修正
						//-----------------------------
						#endregion
						break;

					//					case CStage.Eステージ.オプション:
					#region [ *** ]
					//						//-----------------------------
					//						if( this.n進行描画の戻り値 != 0 )
					//						{
					//							switch( r直前のステージ.eステージID )
					//							{
					//								case CStage.Eステージ.タイトル:
					//									#region [ *** ]
					//									//-----------------------------
					//									r現在のステージ.On非活性化();
					//									Trace.TraceInformation( "----------------------" );
					//									Trace.TraceInformation( "■ タイトル" );
					//									stageタイトル.On活性化();
					//									r直前のステージ = r現在のステージ;
					//									r現在のステージ = stageタイトル;
					//				
					//									this.tガベージコレクションを実行する();
					//									break;
					//								//-----------------------------
					//									#endregion
					//
					//								case CStage.Eステージ.選曲:
					//									#region [ *** ]
					//									//-----------------------------
					//									r現在のステージ.On非活性化();
					//									Trace.TraceInformation( "----------------------" );
					//									Trace.TraceInformation( "■ 選曲" );
					//									stage選曲.On活性化();
					//									r直前のステージ = r現在のステージ;
					//									r現在のステージ = stage選曲;
					//
					//									this.tガベージコレクションを実行する();
					//									break;
					//								//-----------------------------
					//									#endregion
					//							}
					//						}
					//						//-----------------------------
					#endregion
					//						break;

					case CStage.Eステージ.コンフィグ:
						#region [ *** ]
						//-----------------------------
						if (this.n進行描画の戻り値 != 0)
						{
							switch (r直前のステージ.eステージID)
							{
								case CStage.Eステージ.タイトル:
									#region [ *** ]
									//-----------------------------
									r現在のステージ.On非活性化();
									Trace.TraceInformation("----------------------");
									Trace.TraceInformation("■ タイトル");
									stageタイトル.On活性化();
									r直前のステージ = r現在のステージ;
									r現在のステージ = stageタイトル;

									this.tガベージコレクションを実行する();
									break;
								//-----------------------------
								#endregion

								case CStage.Eステージ.選曲:
									#region [ *** ]
									//-----------------------------
									r現在のステージ.On非活性化();
									Trace.TraceInformation("----------------------");
									Trace.TraceInformation("■ 選曲");
									stage選曲.On活性化();
									r直前のステージ = r現在のステージ;
									r現在のステージ = stage選曲;

									this.tガベージコレクションを実行する();
									break;
									//-----------------------------
									#endregion
							}
						}
						//-----------------------------
						#endregion
						break;

					case CStage.Eステージ.選曲:
						#region [ *** ]
						//-----------------------------
						switch (this.n進行描画の戻り値)
						{
							case (int)CStage選曲.E戻り値.タイトルに戻る:
								#region [ *** ]
								//-----------------------------
								r現在のステージ.On非活性化();
								Trace.TraceInformation("----------------------");
								Trace.TraceInformation("■ タイトル");
								stageタイトル.On活性化();
								r直前のステージ = r現在のステージ;
								r現在のステージ = stageタイトル;

								this.tガベージコレクションを実行する();
								break;
							//-----------------------------
							#endregion

							case (int)CStage選曲.E戻り値.選曲した:
								#region [ *** ]
								//-----------------------------
								r現在のステージ.On非活性化();
								Trace.TraceInformation("----------------------");
								Trace.TraceInformation("■ 曲読み込み");
								stage曲読み込み.On活性化();
								r直前のステージ = r現在のステージ;
								r現在のステージ = stage曲読み込み;

								this.tガベージコレクションを実行する();
								break;
							//-----------------------------
							#endregion

							//							case (int) CStage選曲.E戻り値.オプション呼び出し:
							#region [ *** ]
							//								//-----------------------------
							//								r現在のステージ.On非活性化();
							//								Trace.TraceInformation( "----------------------" );
							//								Trace.TraceInformation( "■ オプション" );
							//								stageオプション.On活性化();
							//								r直前のステージ = r現在のステージ;
							//								r現在のステージ = stageオプション;
							//
							//								this.tガベージコレクションを実行する();
							//								break;
							//							//-----------------------------
							#endregion

							case (int)CStage選曲.E戻り値.コンフィグ呼び出し:
								#region [ *** ]
								//-----------------------------
								r現在のステージ.On非活性化();
								Trace.TraceInformation("----------------------");
								Trace.TraceInformation("■ コンフィグ");
								stageコンフィグ.On活性化();
								r直前のステージ = r現在のステージ;
								r現在のステージ = stageコンフィグ;

								this.tガベージコレクションを実行する();
								break;
							//-----------------------------
							#endregion

							case (int)CStage選曲.E戻り値.スキン変更:

								#region [ *** ]
								//-----------------------------
								r現在のステージ.On非活性化();
								Trace.TraceInformation("----------------------");
								Trace.TraceInformation("■ スキン切り替え");
								stageChangeSkin.On活性化();
								r直前のステージ = r現在のステージ;
								r現在のステージ = stageChangeSkin;
								break;
								//-----------------------------
								#endregion
						}
						//-----------------------------
						#endregion
						break;

					case CStage.Eステージ.曲読み込み:
						#region [ *** ]
						//-----------------------------
						DTXVmode.Refreshed = false;     // 曲のリロード中に発生した再リロードは、無視する。
						if (this.n進行描画の戻り値 != 0)
						{
							TJAPlayer3.Pad.st検知したデバイス.Clear();  // 入力デバイスフラグクリア(2010.9.11)
							r現在のステージ.On非活性化();
							#region [ ESC押下時は、曲の読み込みを中止して選曲画面に戻る ]
							if (this.n進行描画の戻り値 == (int)E曲読込画面の戻り値.読込中止)
							{
								//DTX.t全チップの再生停止();
								if (DTX != null)
									DTX.On非活性化();
								Trace.TraceInformation("曲の読み込みを中止しました。");
								this.tガベージコレクションを実行する();
								Trace.TraceInformation("----------------------");
								Trace.TraceInformation("■ 選曲");
								stage選曲.On活性化();
								r直前のステージ = r現在のステージ;
								r現在のステージ = stage選曲;
								break;
							}
							#endregion

							Trace.TraceInformation("----------------------");
							Trace.TraceInformation("■ 演奏（ドラム画面）");

							r直前のステージ = r現在のステージ;
							r現在のステージ = stage演奏ドラム画面;

							this.tガベージコレクションを実行する();
						}
						//-----------------------------
						#endregion
						break;

					case CStage.Eステージ.演奏:
						#region [ *** ]
						//-----------------------------
						//long n1 = FDK.CSound管理.rc演奏用タイマ.nシステム時刻ms;
						//long n2 = FDK.CSound管理.SoundDevice.n経過時間ms;
						//long n3 = FDK.CSound管理.SoundDevice.tmシステムタイマ.nシステム時刻ms;
						//long n4 = FDK.CSound管理.rc演奏用タイマ.n現在時刻;
						//long n5 = FDK.CSound管理.SoundDevice.n経過時間を更新したシステム時刻ms;

						//swlist1.Add( Convert.ToInt32(n1) );
						//swlist2.Add( Convert.ToInt32(n2) );
						//swlist3.Add( Convert.ToInt32( n3 ) );
						//swlist4.Add( Convert.ToInt32( n4 ) );
						//swlist5.Add( Convert.ToInt32( n5 ) );

						#region [ DTXVモード中にDTXCreatorから指示を受けた場合の処理 ]
						if (DTXVmode.Enabled && DTXVmode.Refreshed)
						{
							DTXVmode.Refreshed = false;

							if (DTXVmode.Command == CDTXVmode.ECommand.Stop)
							{
								TJAPlayer3.stage演奏ドラム画面.t停止();
								if (previewSound != null)
								{
									this.previewSound.tサウンドを停止する();
									this.previewSound.Dispose();
									this.previewSound = null;
								}
								//{
								//    int lastd = 0;
								//    int f = 0;
								//    for ( int i = 0; i < swlist1.Count; i++ )
								//    {
								//        int d1 = swlist1[ i ];
								//        int d2 = swlist2[ i ];
								//        int d3 = swlist3[ i ];
								//        int d4 = swlist4[ i ];
								//        int d5 = swlist5[ i ];

								//        int dif = d1 - lastd;
								//        string s = "";
								//        if ( 16 <= dif && dif <= 17 )
								//        {
								//        }
								//        else
								//        {
								//            s = "★";
								//        }
								//        Trace.TraceInformation( "frame {0:D4}: {1:D3} ( {2:D3}, {3:D3} - {7:D3}, {4:D3} ) {5}, n現在時刻={6}", f, dif, d1, d2, d3, s, d4, d5 );
								//        lastd = d1;
								//        f++;
								//    }
								//    swlist1.Clear();
								//    swlist2.Clear();
								//    swlist3.Clear();
								//    swlist4.Clear();
								//    swlist5.Clear();

								//}
							}
							else if (DTXVmode.Command == CDTXVmode.ECommand.Play)
							{
								if (DTXVmode.NeedReload)
								{
									TJAPlayer3.stage演奏ドラム画面.t再読込();

									TJAPlayer3.ConfigIni.bTimeStretch = DTXVmode.TimeStretch;
									CSound管理.bIsTimeStretch = DTXVmode.TimeStretch;
									if (TJAPlayer3.ConfigIni.b垂直帰線待ちを行う != DTXVmode.VSyncWait)
									{
										TJAPlayer3.ConfigIni.b垂直帰線待ちを行う = DTXVmode.VSyncWait;
										TJAPlayer3.app.b次のタイミングで垂直帰線同期切り替えを行う = true;
									}
								}
								else
								{
									TJAPlayer3.stage演奏ドラム画面.t演奏位置の変更(TJAPlayer3.DTXVmode.nStartBar, 0);
								}
							}
						}
						#endregion

						switch (this.n進行描画の戻り値)
						{
							case (int)E演奏画面の戻り値.再読込_再演奏:
								#region [ DTXファイルを再読み込みして、再演奏 ]
								DTX.t全チップの再生停止();
								DTX.On非活性化();
								r現在のステージ.On非活性化();
								stage曲読み込み.On活性化();
								r直前のステージ = r現在のステージ;
								r現在のステージ = stage曲読み込み;
								this.tガベージコレクションを実行する();
								break;
							#endregion

							//case (int) E演奏画面の戻り値.再演奏:
							#region [ 再読み込み無しで、再演奏 ]
							#endregion
							//	break;

							case (int)E演奏画面の戻り値.継続:
								break;

							case (int)E演奏画面の戻り値.演奏中断:
								#region [ 演奏キャンセル ]
								//-----------------------------
								scoreIni = this.tScoreIniへBGMAdjustとHistoryとPlayCountを更新("Play canceled");


								DTX.t全チップの再生停止();
								DTX.On非活性化();
								r現在のステージ.On非活性化();

								Trace.TraceInformation("----------------------");
								Trace.TraceInformation("■ 選曲");
								stage選曲.On活性化();
								r直前のステージ = r現在のステージ;
								r現在のステージ = stage選曲;

								this.tガベージコレクションを実行する();

								break;
							//-----------------------------
							#endregion

							case (int)E演奏画面の戻り値.ステージ失敗:
								#region [ 演奏失敗(StageFailed) ]
								//-----------------------------
								scoreIni = this.tScoreIniへBGMAdjustとHistoryとPlayCountを更新("Stage failed");

								DTX.t全チップの再生停止();
								DTX.On非活性化();
								r現在のステージ.On非活性化();

								Trace.TraceInformation("----------------------");
								Trace.TraceInformation("■ 選曲");
								stage選曲.On活性化();
								r直前のステージ = r現在のステージ;
								r現在のステージ = stage選曲;

								this.tガベージコレクションを実行する();
								break;
							//-----------------------------
							#endregion

							case (int)E演奏画面の戻り値.ステージクリア:
								#region [ 演奏クリア ]
								//-----------------------------
								CScoreIni.C演奏記録 c演奏記録_Drums;
								stage演奏ドラム画面.t演奏結果を格納する(out c演奏記録_Drums);

								double ps = 0.0, gs = 0.0;
								if (!c演奏記録_Drums.b全AUTOである && c演奏記録_Drums.n全チップ数 > 0)
								{
									ps = c演奏記録_Drums.db演奏型スキル値;
									gs = c演奏記録_Drums.dbゲーム型スキル値;
								}
								string str = "Cleared";
								switch (CScoreIni.t総合ランク値を計算して返す(c演奏記録_Drums, null, null))
								{
									case (int)CScoreIni.ERANK.SS:
										str = string.Format("Cleared (SS: {0:F2})", ps);
										break;

									case (int)CScoreIni.ERANK.S:
										str = string.Format("Cleared (S: {0:F2})", ps);
										break;

									case (int)CScoreIni.ERANK.A:
										str = string.Format("Cleared (A: {0:F2})", ps);
										break;

									case (int)CScoreIni.ERANK.B:
										str = string.Format("Cleared (B: {0:F2})", ps);
										break;

									case (int)CScoreIni.ERANK.C:
										str = string.Format("Cleared (C: {0:F2})", ps);
										break;

									case (int)CScoreIni.ERANK.D:
										str = string.Format("Cleared (D: {0:F2})", ps);
										break;

									case (int)CScoreIni.ERANK.E:
										str = string.Format("Cleared (E: {0:F2})", ps);
										break;

									case (int)CScoreIni.ERANK.UNKNOWN:  // #23534 2010.10.28 yyagi add: 演奏チップが0個のとき
										str = "Cleared (No chips)";
										break;
								}

								scoreIni = this.tScoreIniへBGMAdjustとHistoryとPlayCountを更新(str);

								r現在のステージ.On非活性化();
								Trace.TraceInformation("----------------------");
								Trace.TraceInformation("■ 結果");
								stage結果.st演奏記録.Drums = c演奏記録_Drums;
								stage結果.On活性化();
								r直前のステージ = r現在のステージ;
								r現在のステージ = stage結果;

								break;
								//-----------------------------
								#endregion
						}
						//-----------------------------
						#endregion
						break;

					case CStage.Eステージ.結果:
						#region [ *** ]
						//-----------------------------
						if (this.n進行描画の戻り値 != 0)
						{
							//DTX.t全チップの再生一時停止();
							DTX.t全チップの再生停止とミキサーからの削除();
							DTX.On非活性化();
							r現在のステージ.On非活性化();
							this.tガベージコレクションを実行する();

							Trace.TraceInformation("----------------------");
							Trace.TraceInformation("■ 選曲");
							stage選曲.On活性化();
							r直前のステージ = r現在のステージ;
							r現在のステージ = stage選曲;

							this.tガベージコレクションを実行する();
						}
						//-----------------------------
						#endregion
						break;

					case CStage.Eステージ.ChangeSkin:
						#region [ *** ]
						//-----------------------------
						if (this.n進行描画の戻り値 != 0)
						{
							r現在のステージ.On非活性化();
							Trace.TraceInformation("----------------------");
							Trace.TraceInformation("■ 選曲");
							stage選曲.On活性化();
							r直前のステージ = r現在のステージ;
							r現在のステージ = stage選曲;
							this.tガベージコレクションを実行する();
						}
						//-----------------------------
						#endregion
						break;

					case CStage.Eステージ.終了:
						#region [ *** ]
						//-----------------------------
						if (this.n進行描画の戻り値 != 0)
						{
							base.Exit();
						}
						//-----------------------------
						#endregion
						break;
				}

				actScanningLoudness.On進行描画();

				// オーバレイを描画する(テクスチャの生成されていない起動ステージは例外
				if (r現在のステージ != null && r現在のステージ.eステージID != CStage.Eステージ.起動 && TJAPlayer3.Tx.Overlay != null)
				{
					TJAPlayer3.Tx.Overlay.t2D描画(app.Device, 0, 0);
				}
			}
			if (Sound管理?.GetCurrentSoundDeviceType() != "DirectSound")
			{
				Sound管理?.t再生中の処理をする();  // サウンドバッファの更新; 画面描画と同期させることで、スクロールをスムーズにする
			}


			#region [ 全画面_ウインドウ切り替え ]
			if (this.b次のタイミングで全画面_ウィンドウ切り替えを行う)
			{
				ConfigIni.b全画面モード = !ConfigIni.b全画面モード;
				app.t全画面_ウィンドウモード切り替え();
				this.b次のタイミングで全画面_ウィンドウ切り替えを行う = false;
			}
			#endregion
			#region [ 垂直基線同期切り替え ]
			if (this.b次のタイミングで垂直帰線同期切り替えを行う)
			{
				bool bIsMaximized = (this.WindowState == WindowState.Maximized);// #23510 2010.11.3 yyagi: to backup current window mode before changing VSyncWait
				bool bIsFullScreen = (this.WindowState == WindowState.Fullscreen);

				if (ConfigIni.b垂直帰線待ちを行う)
					base.VSync = VSyncMode.On;
				else
					base.VSync = VSyncMode.Off;

				this.b次のタイミングで垂直帰線同期切り替えを行う = false;
				base.Width = ConfigIni.bウィンドウモード ? ConfigIni.nウインドウwidth : GameWindowSize.Width;
				base.Height = ConfigIni.bウィンドウモード ? ConfigIni.nウインドウheight : GameWindowSize.Height;
				if (bIsMaximized)
				{
					this.WindowState = WindowState.Maximized;// #23510 2010.11.3 yyagi: to resume window mode after changing VSyncWait
				}
				else if (bIsFullScreen)
				{
					this.WindowState = WindowState.Fullscreen;
				}
			}
			#endregion
			
			GL.Flush();

			base.SwapBuffers();
			//rsw.Stop();
			//Trace.WriteLine("Render Time=" + rsw.ElapsedMilliseconds + "ms");
		}

		// その他

		#region [ 汎用ヘルパー ]
		//-----------------
		public static CTexture tテクスチャの生成(string fileName)
		{
			if (app == null)
			{
				return null;
			}
			try
			{
				return new CTexture(app.Device, fileName);
			}
			catch (CTextureCreateFailedException e)
			{
				Trace.TraceError(e.ToString());
				Trace.TraceError("テクスチャの生成に失敗しました。({0})", fileName);
				return null;
			}
			catch (FileNotFoundException)
			{
				Trace.TraceWarning("テクスチャファイルが見つかりませんでした。({0})", fileName);
				return null;
			}
		}
		public static CTexture tテクスチャの生成(Bitmap bitmap)
		{
			if (app == null)
			{
				return null;
			}
			if (bitmap == null)
			{
				Trace.TraceError("テクスチャの生成に失敗しました。(bitmap==null)");
				return null;
			}
			try
			{
				return new CTexture(app.Device, bitmap);
			}
			catch (CTextureCreateFailedException e)
			{
				Trace.TraceError(e.ToString());
				Trace.TraceError("テクスチャの生成に失敗しました。(txData)");
				return null;
			}
		}

		public static CTexture ColorTexture(string htmlcolor, int width, int height)//2020.05.31 Mr-Ojii 単色塗りつぶしテクスチャの生成。必要かって？Tile_Black・Tile_Whiteがいらなくなるじゃん。あと、メンテモードの画像生成に便利かなって。
		{
			if (htmlcolor.Length == 7 && htmlcolor.StartsWith("#"))
			{
				Color color = ColorTranslator.FromHtml(htmlcolor);
				Brush brush = new SolidBrush(color);
				return ColorTexture(brush, width, height);
			}
			else
				return ColorTexture(Brushes.Black);
		}
		public static CTexture ColorTexture(string htmlcolor)
		{
			if (htmlcolor.Length == 7 && htmlcolor.StartsWith("#"))
			{
				Color color = ColorTranslator.FromHtml(htmlcolor);
				Brush brush = new SolidBrush(color);
				return ColorTexture(brush);
			}
			else
				return ColorTexture(Brushes.Black);
		}
		public static CTexture ColorTexture(Brush brush)
		{
			return ColorTexture(brush, 64, 64);
		}
		/// <summary>
		/// 単色塗りつぶしテクスチャの生成
		/// </summary>
		/// <param name="brush">ブラシの色とかの指定</param>
		/// <param name="width">幅</param>
		/// <param name="height">高さ</param>
		/// <returns></returns>
		public static CTexture ColorTexture(Brush brush, int width, int height)
		{
			Bitmap bmp = new Bitmap(width, height);
			Graphics gra = Graphics.FromImage(bmp);
			gra.FillRectangle(brush, 0, 0, width, height);
			gra.Dispose();
			return TJAPlayer3.tテクスチャの生成(bmp);
		}

		public static CDirectShow t失敗してもスキップ可能なDirectShowを生成する(string fileName, IntPtr hWnd, bool bオーディオレンダラなし)
		{
			CDirectShow ds = null;
			if (File.Exists(fileName))
			{
				try
				{
					ds = new CDirectShow(fileName, hWnd, bオーディオレンダラなし);
				}
				catch (FileNotFoundException e)
				{
					Trace.TraceError(e.ToString());
					Trace.TraceError("動画ファイルが見つかりませんでした。({0})", fileName);
					ds = null;      // Dispose はコンストラクタ内で実施済み
				}
				catch (Exception e)
				{
					Trace.TraceError(e.ToString());
					Trace.TraceError("DirectShow の生成に失敗しました。[{0}]", fileName);
					ds = null;      // Dispose はコンストラクタ内で実施済み
				}
			}
			else
			{
				Trace.TraceError("動画ファイルが見つかりませんでした。({0})", fileName);
				return null;
			}

			return ds;
		}

		/// <summary>プロパティ、インデクサには ref は使用できないので注意。</summary>
		public static void t安全にDisposeする<T>(ref T obj) where T : class, IDisposable //2020.06.06 Mr-Ojii twopointzero氏のソースコードをもとに改良
		{
			if (obj == null)
				return;

			obj.Dispose();
			obj = null;
		}
		public static void t安全にDisposeする<T>(ref T[] array) where T : class, IDisposable //2020.08.01 Mr-Ojii twopointzero氏のソースコードをもとに追加
		{
			if (array == null)
			{
				return;
			}

			for (var i = 0; i < array.Length; i++)
			{
				array[i]?.Dispose();
				array[i] = null;
			}
		}

		/// <summary>
		/// そのフォルダの連番画像の最大値を返す。
		/// </summary>
		public static int t連番画像の枚数を数える(string ディレクトリ名, string プレフィックス = "", string 拡張子 = ".png")
		{
			int num = 0;
			while (File.Exists(ディレクトリ名 + プレフィックス + num + 拡張子))
			{
				num++;
			}
			return num;
		}

		/// <summary>
		/// そのフォルダの連番フォルダの最大値を返す。
		/// </summary>
		public static int t連番フォルダの個数を数える(string ディレクトリ名, string プレフィックス = "")
		{
			int num = 0;
			while (Directory.Exists(ディレクトリ名 + プレフィックス + num))
			{
				num++;
			}
			return num;
		}

		/// <summary>
		/// 曲名テクスチャの縮小倍率を返す。
		/// </summary>
		/// <param name="cTexture">曲名テクスチャ。</param>
		/// <param name="samePixel">等倍で表示するピクセル数の最大値(デフォルト値:645)</param>
		/// <returns>曲名テクスチャの縮小倍率。そのテクスチャがnullならば一倍(1f)を返す。</returns>
		public static float GetSongNameXScaling(ref CTexture cTexture, int samePixel = 660)
		{
			if (cTexture == null) return 1f;
			float scalingRate = (float)samePixel / (float)cTexture.szテクスチャサイズ.Width;
			if (cTexture.szテクスチャサイズ.Width <= samePixel)
				scalingRate = 1.0f;
			return scalingRate;
		}

		/// <summary>
		/// 難易度を表す数字を列挙体に変換します。
		/// </summary>
		/// <param name="number">難易度を表す数字。</param>
		/// <returns>Difficulty 列挙体</returns>
		public static Difficulty DifficultyNumberToEnum(int number)
		{
			switch (number)
			{
				case 0:
					return Difficulty.Easy;
				case 1:
					return Difficulty.Normal;
				case 2:
					return Difficulty.Hard;
				case 3:
					return Difficulty.Oni;
				case 4:
					return Difficulty.Edit;
				case 5:
					return Difficulty.Tower;
				case 6:
					return Difficulty.Dan;
				default:
					throw new IndexOutOfRangeException();
			}
		}

		//-----------------
		#endregion

		#region [ private ]
		//-----------------
		private bool bマウスカーソル表示中 = true;
		private bool b終了処理完了済み;
		private static CDTX[] dtx = new CDTX[4];

		public static TextureLoader Tx = new TextureLoader();

		private List<CActivity> listトップレベルActivities;
		private int n進行描画の戻り値;
		private OpenTK.Input.MouseButton mb = OpenTK.Input.MouseButton.Left;
		private Stopwatch sw = new Stopwatch();

		private CSound previewSound;
		public static long StartupTime
		{
			get;
			private set;
		}

		private void t起動処理()
		{
			#region [ strEXEのあるフォルダを決定する ]
			//-----------------
			// BEGIN #23629 2010.11.13 from: デバッグ時は Application.ExecutablePath が ($SolutionDir)/bin/x86/Debug/ などになり System/ の読み込みに失敗するので、カレントディレクトリを採用する。（プロジェクトのプロパティ→デバッグ→作業ディレクトリが有効になる）
#if DEBUG
			strEXEのあるフォルダ = Environment.CurrentDirectory + @"\";
#else
			strEXEのあるフォルダ = Path.GetDirectoryName( Application.ExecutablePath ) + @"\";	// #23629 2010.11.9 yyagi: set correct pathname where DTXManiaGR.exe is.
#endif
			// END #23629 2010.11.13 from
			//-----------------
			#endregion

			#region [ Config.ini の読込み ]
			//---------------------
			ConfigIni = new CConfigIni();
			string path = strEXEのあるフォルダ + "Config.ini";
			if (File.Exists(path))
			{
				try
				{
					ConfigIni.tファイルから読み込み(path);
				}
				catch (Exception e)
				{
					//ConfigIni = new CConfigIni();	// 存在してなければ新規生成
					Trace.TraceError(e.ToString());
					Trace.TraceError("例外が発生しましたが処理を継続します。 (b8d93255-bbe4-4ca3-8264-7ee5175b19f3)");
				}
			}

			//---------------------
			#endregion
			#region [ ログ出力開始 ]
			//---------------------
			Trace.AutoFlush = true;
			if (ConfigIni.bログ出力)
			{
				bool log出力ok = false;
				int num = 0;
				while (!log出力ok)
				{
					try
					{
						string logname;
						if (num == 0)
							logname = "TJAPlayer3-f.log";
						else
							logname = "TJAPlayer3-f_" + num.ToString() + ".log";
						Trace.Listeners.Add(new CTraceLogListener(new StreamWriter(System.IO.Path.Combine(strEXEのあるフォルダ, logname), false, new UTF8Encoding(false))));
						log出力ok = true;
					}
					catch (Exception)
					{
						num++;
					}
				}
			}
			Trace.WriteLine("");
			Trace.WriteLine("DTXMania powered by YAMAHA Silent Session Drums");
			Trace.WriteLine(string.Format("Release: {0}", VERSION));
			Trace.WriteLine("");
			Trace.TraceInformation("----------------------");
			Trace.TraceInformation("■ アプリケーションの初期化");
			Trace.TraceInformation("OS Version: " + Environment.OSVersion);
			Trace.TraceInformation("ProcessorCount: " + Environment.ProcessorCount.ToString());
			Trace.TraceInformation("CLR Version: " + Environment.Version.ToString());
			//---------------------
			#endregion

			#region [ DTXVmodeクラス の初期化 ]
			//---------------------
			//Trace.TraceInformation( "DTXVモードの初期化を行います。" );
			//Trace.Indent();
			try
			{
				DTXVmode = new CDTXVmode();
				DTXVmode.Enabled = false;
				//Trace.TraceInformation( "DTXVモードの初期化を完了しました。" );
			}
			finally
			{
				//Trace.Unindent();
			}
			//---------------------
			#endregion

			#region [ ウィンドウ初期化 ]
			//---------------------
			base.X = ConfigIni.n初期ウィンドウ開始位置X;
			base.Y = ConfigIni.n初期ウィンドウ開始位置Y;



			base.Title = "";

			base.ClientSize = new Size(ConfigIni.nウインドウwidth, ConfigIni.nウインドウheight);   // #34510 yyagi 2010.10.31 to change window size got from Config.ini

			base.Icon = global::TJAPlayer3.Properties.Resources.tjap3;
			base.KeyDown += this.Window_KeyDown;
			base.MouseDown += this.Window_MouseDown;
			base.Resize += this.Window_ResizeEnd;                       // #23510 2010.11.20 yyagi: to set resized window size in Config.ini*/
			base.Move += this.Window_MoveEnd;
			//---------------------
			#endregion
			#region [ Direct3D9 デバイスの生成 ]
			//---------------------
			if (ConfigIni.bウィンドウモード)
				base.WindowState = WindowState.Normal;
			else
				base.WindowState = WindowState.Fullscreen;

			if (ConfigIni.b垂直帰線待ちを行う)
				base.VSync = VSyncMode.On;
			else
				base.VSync = VSyncMode.Off;



			base.ClientSize = new Size(ConfigIni.nウインドウwidth, ConfigIni.nウインドウheight);   // #23510 2010.10.31 yyagi: to recover window size. width and height are able to get from Config.ini.

			//---------------------
			#endregion

			DTX = null;

			#region [ Skin の初期化 ]
			//---------------------
			Trace.TraceInformation("スキンの初期化を行います。");
			Trace.Indent();
			try
			{
				Skin = new CSkin(TJAPlayer3.ConfigIni.strSystemSkinSubfolderFullName, false);
				TJAPlayer3.ConfigIni.strSystemSkinSubfolderFullName = TJAPlayer3.Skin.GetCurrentSkinSubfolderFullName(true);    // 旧指定のSkinフォルダが消滅していた場合に備える
				Trace.TraceInformation("スキンの初期化を完了しました。");
			}
			catch
			{
				Trace.TraceInformation("スキンの初期化に失敗しました。");
				throw;
			}
			finally
			{
				Trace.Unindent();
			}
			//---------------------
			#endregion
			//-----------
			#region [ Timer の初期化 ]
			//---------------------
			Trace.TraceInformation("タイマの初期化を行います。");
			Trace.Indent();
			try
			{
				Timer = new CTimer(CTimer.E種別.MultiMedia);
				Trace.TraceInformation("タイマの初期化を完了しました。");
			}
			finally
			{
				Trace.Unindent();
			}
			//---------------------
			#endregion
			//-----------

			#region [ FPS カウンタの初期化 ]
			//---------------------
			Trace.TraceInformation("FPSカウンタの初期化を行います。");
			Trace.Indent();
			try
			{
				FPS = new CFPS();
				Trace.TraceInformation("FPSカウンタを生成しました。");
			}
			finally
			{
				Trace.Unindent();
			}
			//---------------------
			#endregion
			#region [ act文字コンソールの初期化 ]
			//---------------------
			Trace.TraceInformation("文字コンソールの初期化を行います。");
			Trace.Indent();
			try
			{
				act文字コンソール = new C文字コンソール();
				Trace.TraceInformation("文字コンソールを生成しました。");
				act文字コンソール.On活性化();
				Trace.TraceInformation("文字コンソールを活性化しました。");
				Trace.TraceInformation("文字コンソールの初期化を完了しました。");
			}
			catch (Exception exception)
			{
				Trace.TraceError(exception.ToString());
				Trace.TraceError("文字コンソールの初期化に失敗しました。");
			}
			finally
			{
				Trace.Unindent();
			}
			//---------------------
			#endregion
			#region [ Input管理 の初期化 ]
			//---------------------
			Trace.TraceInformation("DirectInput, MIDI入力の初期化を行います。");
			Trace.Indent();
			try
			{
				Input管理 = new CInput管理();
				foreach (IInputDevice device in Input管理.list入力デバイス)
				{
					if ((device.e入力デバイス種別 == E入力デバイス種別.Joystick) && !ConfigIni.dicJoystick.ContainsValue(device.GUID))
					{
						int key = 0;
						while (ConfigIni.dicJoystick.ContainsKey(key))
						{
							key++;
						}
						ConfigIni.dicJoystick.Add(key, device.GUID);
					}
				}
				Trace.TraceInformation("DirectInput の初期化を完了しました。");
			}
			catch
			{
				Trace.TraceError("DirectInput, MIDI入力の初期化に失敗しました。");
				throw;
			}
			finally
			{
				Trace.Unindent();
			}
			//---------------------
			#endregion
			#region [ Pad の初期化 ]
			//---------------------
			Trace.TraceInformation("パッドの初期化を行います。");
			Trace.Indent();
			try
			{
				Pad = new CPad(ConfigIni, Input管理);
				Trace.TraceInformation("パッドの初期化を完了しました。");
			}
			catch (Exception exception3)
			{
				Trace.TraceError(exception3.ToString());
				Trace.TraceError("パッドの初期化に失敗しました。");
			}
			finally
			{
				Trace.Unindent();
			}
			//---------------------
			#endregion
			#region [ Sound管理 の初期化 ]
			//---------------------
			Trace.TraceInformation("サウンドデバイスの初期化を行います。");
			Trace.Indent();
			try
			{
				ESoundDeviceType soundDeviceType;
				switch (TJAPlayer3.ConfigIni.nSoundDeviceType)
				{
					case 0:
						soundDeviceType = ESoundDeviceType.DirectSound;
						break;
					case 1:
						soundDeviceType = ESoundDeviceType.ASIO;
						break;
					case 2:
						soundDeviceType = ESoundDeviceType.ExclusiveWASAPI;
						break;
					case 3:
						soundDeviceType = ESoundDeviceType.SharedWASAPI;
						break;
					default:
						soundDeviceType = ESoundDeviceType.Unknown;
						break;
				}
				Sound管理 = new CSound管理(this.WindowHandle,
											soundDeviceType,
											TJAPlayer3.ConfigIni.nWASAPIBufferSizeMs,
											// CDTXMania.ConfigIni.nASIOBufferSizeMs,
											0,
											TJAPlayer3.ConfigIni.nASIODevice,
											TJAPlayer3.ConfigIni.bUseOSTimer
				);
				//Sound管理 = FDK.CSound管理.Instance;
				//Sound管理.t初期化( soundDeviceType, 0, 0, CDTXMania.ConfigIni.nASIODevice, base.Window.Handle );


				Trace.TraceInformation("Initializing loudness scanning, song gain control, and sound group level control...");
				Trace.Indent();
				try
				{
					actScanningLoudness = new CActScanningLoudness();
					actScanningLoudness.On活性化();
					LoudnessMetadataScanner.ScanningStateChanged +=
						(_, args) => actScanningLoudness.bIsActivelyScanning = args.IsActivelyScanning;
					LoudnessMetadataScanner.StartBackgroundScanning();

					SongGainController = new SongGainController();
					ConfigIniToSongGainControllerBinder.Bind(ConfigIni, SongGainController);

					SoundGroupLevelController = new SoundGroupLevelController(CSound.listインスタンス);
					ConfigIniToSoundGroupLevelControllerBinder.Bind(ConfigIni, SoundGroupLevelController);
				}
				finally
				{
					Trace.Unindent();
					Trace.TraceInformation("Initialized loudness scanning, song gain control, and sound group level control.");
				}

				ShowWindowTitleWithSoundType();
				FDK.CSound管理.bIsTimeStretch = TJAPlayer3.ConfigIni.bTimeStretch;
				Sound管理.nMasterVolume = TJAPlayer3.ConfigIni.nMasterVolume;
				//FDK.CSound管理.bIsMP3DecodeByWindowsCodec = CDTXMania.ConfigIni.bNoMP3Streaming;
				Trace.TraceInformation("サウンドデバイスの初期化を完了しました。");
			}
			catch (Exception e)
			{
				throw new NullReferenceException("サウンドデバイスがひとつも有効になっていないため、サウンドデバイスの初期化ができませんでした。", e);
			}
			finally
			{
				Trace.Unindent();
			}
			//---------------------
			#endregion
			#region [ Songs管理 の初期化 ]
			//---------------------
			Trace.TraceInformation("曲リストの初期化を行います。");
			Trace.Indent();
			try
			{
				Songs管理 = new CSongs管理();
				//				Songs管理_裏読 = new CSongs管理();
				EnumSongs = new CEnumSongs();
				actEnumSongs = new CActEnumSongs();
				Trace.TraceInformation("曲リストの初期化を完了しました。");
			}
			catch (Exception e)
			{
				Trace.TraceError(e.ToString());
				Trace.TraceError("曲リストの初期化に失敗しました。");
			}
			finally
			{
				Trace.Unindent();
			}
			//---------------------
			#endregion
			#region [ CAvi の初期化 ]
			//---------------------
			CAvi.t初期化();
			//---------------------
			#endregion
			#region [ Random の初期化 ]
			//---------------------
			Random = new Random((int)Timer.nシステム時刻);
			//---------------------
			#endregion
			#region [ ステージの初期化 ]
			//---------------------
			r現在のステージ = null;
			r直前のステージ = null;
			stage起動 = new CStage起動();
			stageタイトル = new CStageタイトル();
			//			stageオプション = new CStageオプション();
			stageコンフィグ = new CStageコンフィグ();
			stage選曲 = new CStage選曲();
			stage曲読み込み = new CStage曲読み込み();
			stage演奏ドラム画面 = new CStage演奏ドラム画面();
			stage結果 = new CStage結果();
			stageChangeSkin = new CStageChangeSkin();
			stage終了 = new CStage終了();
			this.listトップレベルActivities = new List<CActivity>();
			this.listトップレベルActivities.Add(actEnumSongs);
			this.listトップレベルActivities.Add(act文字コンソール);
			this.listトップレベルActivities.Add(stage起動);
			this.listトップレベルActivities.Add(stageタイトル);
			//			this.listトップレベルActivities.Add( stageオプション );
			this.listトップレベルActivities.Add(stageコンフィグ);
			this.listトップレベルActivities.Add(stage選曲);
			this.listトップレベルActivities.Add(stage曲読み込み);
			this.listトップレベルActivities.Add(stage演奏ドラム画面);
			this.listトップレベルActivities.Add(stage結果);
			this.listトップレベルActivities.Add(stageChangeSkin);
			this.listトップレベルActivities.Add(stage終了);
			//---------------------
			#endregion
			#region Discordの処理
			Discord.Initialize("692578108997632051");
			StartupTime = Discord.GetUnixTime();
			Discord.UpdatePresence("", Properties.Discord.Stage_StartUp, StartupTime);
			#endregion


			Trace.TraceInformation("アプリケーションの初期化を完了しました。");


			#region [ 最初のステージの起動 ]
			//---------------------
			Trace.TraceInformation("----------------------");
			Trace.TraceInformation("■ 起動");

			r現在のステージ = stage起動;

			r現在のステージ.On活性化();

			//---------------------
			#endregion
		}

		public void ShowWindowTitleWithSoundType()
		{
			string delay = "";
			if (Sound管理.GetCurrentSoundDeviceType() != "DirectSound")
			{
				delay = "(" + Sound管理.GetSoundDelay() + "ms)";
			}
			AssemblyName asmApp = Assembly.GetExecutingAssembly().GetName();
			base.Title = asmApp.Name + " Ver." + VERSION + " (" + Sound管理.GetCurrentSoundDeviceType() + delay + ")";
		}

		public void ChangeWindowTitle(string Name, bool StringInitialize = true, bool Concat = true)
		{
			if (StringInitialize)
				this.ShowWindowTitleWithSoundType();
			if (Concat)
				Name = base.Title + Name;
			base.Title = Name;
		}

		private void t終了処理()
		{
			if (!this.b終了処理完了済み)
			{
				Trace.TraceInformation("----------------------");
				Trace.TraceInformation("■ アプリケーションの終了");
				#region [ 曲検索の終了処理 ]
				//---------------------
				if (actEnumSongs != null)
				{
					Trace.TraceInformation("曲検索actの終了処理を行います。");
					Trace.Indent();
					try
					{
						actEnumSongs.On非活性化();
						actEnumSongs = null;
						Trace.TraceInformation("曲検索actの終了処理を完了しました。");
					}
					catch (Exception e)
					{
						Trace.TraceError(e.ToString());
						Trace.TraceError("曲検索actの終了処理に失敗しました。");
					}
					finally
					{
						Trace.Unindent();
					}
				}
				//---------------------
				#endregion
				#region [ 現在のステージの終了処理 ]
				//---------------------
				if (TJAPlayer3.r現在のステージ != null && TJAPlayer3.r現在のステージ.b活性化してる)     // #25398 2011.06.07 MODIFY FROM
				{
					Trace.TraceInformation("現在のステージを終了します。");
					Trace.Indent();
					try
					{
						r現在のステージ.On非活性化();
						Trace.TraceInformation("現在のステージの終了処理を完了しました。");
					}
					finally
					{
						Trace.Unindent();
					}
				}
				//---------------------
				#endregion
				#region Discordの処理
				Discord.Shutdown();
				#endregion
				#region [ 曲リストの終了処理 ]
				//---------------------
				if (Songs管理 != null)
				{
					Trace.TraceInformation("曲リストの終了処理を行います。");
					Trace.Indent();
					try
					{
						Songs管理 = null;
						Trace.TraceInformation("曲リストの終了処理を完了しました。");
					}
					catch (Exception exception)
					{
						Trace.TraceError(exception.ToString());
						Trace.TraceError("曲リストの終了処理に失敗しました。");
					}
					finally
					{
						Trace.Unindent();
					}
				}
				CAvi.t終了();
				//---------------------
				#endregion
				#region TextureLoaderの処理
				Tx.DisposeTexture();
				#endregion
				#region [ スキンの終了処理 ]
				//---------------------
				if (Skin != null)
				{
					Trace.TraceInformation("スキンの終了処理を行います。");
					Trace.Indent();
					try
					{
						Skin.Dispose();
						Skin = null;
						Trace.TraceInformation("スキンの終了処理を完了しました。");
					}
					catch (Exception exception2)
					{
						Trace.TraceError(exception2.ToString());
						Trace.TraceError("スキンの終了処理に失敗しました。");
					}
					finally
					{
						Trace.Unindent();
					}
				}
				//---------------------
				#endregion
				#region [ DirectSoundの終了処理 ]
				//---------------------
				if (Sound管理 != null)
				{
					Trace.TraceInformation("DirectSound の終了処理を行います。");
					Trace.Indent();
					try
					{
						Sound管理.Dispose();
						Sound管理 = null;
						Trace.TraceInformation("DirectSound の終了処理を完了しました。");
					}
					catch (Exception exception3)
					{
						Trace.TraceError(exception3.ToString());
						Trace.TraceError("DirectSound の終了処理に失敗しました。");
					}
					finally
					{
						Trace.Unindent();
					}
				}
				//---------------------
				#endregion
				#region [ パッドの終了処理 ]
				//---------------------
				if (Pad != null)
				{
					Trace.TraceInformation("パッドの終了処理を行います。");
					Trace.Indent();
					try
					{
						Pad = null;
						Trace.TraceInformation("パッドの終了処理を完了しました。");
					}
					catch (Exception exception4)
					{
						Trace.TraceError(exception4.ToString());
						Trace.TraceError("パッドの終了処理に失敗しました。");
					}
					finally
					{
						Trace.Unindent();
					}
				}
				//---------------------
				#endregion
				#region [ DirectInput, MIDI入力の終了処理 ]
				//---------------------
				if (Input管理 != null)
				{
					Trace.TraceInformation("DirectInput, MIDI入力の終了処理を行います。");
					Trace.Indent();
					try
					{
						Input管理.Dispose();
						Input管理 = null;
						Trace.TraceInformation("DirectInput, MIDI入力の終了処理を完了しました。");
					}
					catch (Exception exception5)
					{
						Trace.TraceError(exception5.ToString());
						Trace.TraceError("DirectInput, MIDI入力の終了処理に失敗しました。");
					}
					finally
					{
						Trace.Unindent();
					}
				}
				//---------------------
				#endregion
				#region [ 文字コンソールの終了処理 ]
				//---------------------
				if (act文字コンソール != null)
				{
					Trace.TraceInformation("文字コンソールの終了処理を行います。");
					Trace.Indent();
					try
					{
						act文字コンソール.On非活性化();
						act文字コンソール = null;
						Trace.TraceInformation("文字コンソールの終了処理を完了しました。");
					}
					catch (Exception exception6)
					{
						Trace.TraceError(exception6.ToString());
						Trace.TraceError("文字コンソールの終了処理に失敗しました。");
					}
					finally
					{
						Trace.Unindent();
					}
				}
				//---------------------
				#endregion
				#region [ FPSカウンタの終了処理 ]
				//---------------------
				Trace.TraceInformation("FPSカウンタの終了処理を行います。");
				Trace.Indent();
				try
				{
					if (FPS != null)
					{
						FPS = null;
					}
					Trace.TraceInformation("FPSカウンタの終了処理を完了しました。");
				}
				finally
				{
					Trace.Unindent();
				}
				//---------------------
				#endregion
				#region [ タイマの終了処理 ]
				//---------------------
				Trace.TraceInformation("タイマの終了処理を行います。");
				Trace.Indent();
				try
				{
					if (Timer != null)
					{
						Timer.Dispose();
						Timer = null;
						Trace.TraceInformation("タイマの終了処理を完了しました。");
					}
					else
					{
						Trace.TraceInformation("タイマは使用されていません。");
					}
				}
				finally
				{
					Trace.Unindent();
				}
				//---------------------
				#endregion
				#region [ Config.iniの出力 ]
				//---------------------
				Trace.TraceInformation("Config.ini を出力します。");
				string str = strEXEのあるフォルダ + "Config.ini";
				Trace.Indent();
				try
				{
					ConfigIni.t書き出し(str);
					Trace.TraceInformation("保存しました。({0})", str);
				}
				catch (Exception e)
				{
					Trace.TraceError(e.ToString());
					Trace.TraceError("Config.ini の出力に失敗しました。({0})", str);
				}
				finally
				{
					Trace.Unindent();
				}

				Trace.TraceInformation("Deinitializing loudness scanning, song gain control, and sound group level control...");
				Trace.Indent();
				try
				{
					SoundGroupLevelController = null;
					SongGainController = null;
					LoudnessMetadataScanner.StopBackgroundScanning(joinImmediately: true);
					actScanningLoudness.On非活性化();
					actScanningLoudness = null;
				}
				finally
				{
					Trace.Unindent();
					Trace.TraceInformation("Deinitialized loudness scanning, song gain control, and sound group level control.");
				}

				ConfigIni = null;

				//---------------------
				#endregion
				Trace.TraceInformation("アプリケーションの終了処理を完了しました。");

				this.b終了処理完了済み = true;
			}
		}
		private CScoreIni tScoreIniへBGMAdjustとHistoryとPlayCountを更新(string str新ヒストリ行)
		{
			bool bIsUpdatedDrums, bIsUpdatedGuitar, bIsUpdatedBass;
			string strFilename = DTX.strファイル名の絶対パス + ".score.ini";
			CScoreIni ini = new CScoreIni(strFilename);
			if (!File.Exists(strFilename))
			{
				ini.stファイル.Title = DTX.TITLE;
				ini.stファイル.Name = DTX.strファイル名;
				ini.stファイル.Hash = CScoreIni.tファイルのMD5を求めて返す(DTX.strファイル名の絶対パス);
				for (int i = 0; i < 3; i++)
				{
					ini.stセクション[i].nPerfectになる範囲ms = nPerfect範囲ms;
					ini.stセクション[i].nGreatになる範囲ms = nGreat範囲ms;
					ini.stセクション[i].nGoodになる範囲ms = nGood範囲ms;
					ini.stセクション[i].nPoorになる範囲ms = nPoor範囲ms;
				}
			}
			ini.stファイル.BGMAdjust = DTX.nBGMAdjust;
			CScoreIni.t更新条件を取得する(out bIsUpdatedDrums, out bIsUpdatedGuitar, out bIsUpdatedBass);

			if (bIsUpdatedDrums)
			{
				ini.stファイル.PlayCountDrums++;
			}
			ini.tヒストリを追加する(str新ヒストリ行);

			stage選曲.r現在選択中のスコア.譜面情報.演奏回数.Drums = ini.stファイル.PlayCountDrums;
			stage選曲.r現在選択中のスコア.譜面情報.演奏回数.Guitar = ini.stファイル.PlayCountGuitar;
			stage選曲.r現在選択中のスコア.譜面情報.演奏回数.Bass = ini.stファイル.PlayCountBass;
			for (int j = 0; j < ini.stファイル.History.Length; j++)
			{
				stage選曲.act曲リスト.r現在選択中のスコア.譜面情報.演奏履歴[j] = ini.stファイル.History[j];

			}

			if (ConfigIni.bScoreIniを出力する)
			{
				ini.t書き出し(strFilename);
			}

			return ini;
		}
		private void tガベージコレクションを実行する()
		{
			GC.Collect(GC.MaxGeneration);
			GC.WaitForPendingFinalizers();
			GC.Collect(GC.MaxGeneration);
		}

		public void RefleshSkin()
		{
			Trace.TraceInformation("スキン変更:" + TJAPlayer3.Skin.GetCurrentSkinSubfolderFullName(false));

			TJAPlayer3.act文字コンソール.On非活性化();

			TJAPlayer3.Skin.Dispose();
			TJAPlayer3.Skin = null;
			TJAPlayer3.Skin = new CSkin(TJAPlayer3.ConfigIni.strSystemSkinSubfolderFullName, false);


			TJAPlayer3.Tx.DisposeTexture();
			TJAPlayer3.Tx.LoadTexture();

			TJAPlayer3.act文字コンソール.On活性化();
		}
		#region [ Windowイベント処理 ]
		//-----------------
		private void Window_KeyDown(object sender, OpenTK.Input.KeyboardKeyEventArgs e)
		{
			if ((e.Key == OpenTK.Input.Key.Enter) && e.Alt)
			{
				if (ConfigIni != null)
				{
					ConfigIni.bウィンドウモード = !ConfigIni.bウィンドウモード;
					this.t全画面_ウィンドウモード切り替え();
				}
			}
			else
			{
				for (int i = 0; i < 0x10; i++)
				{
					if (ConfigIni.KeyAssign.System.Capture[i].コード > 0 &&
						 DeviceConstantConverter.TKKtoKey(e.Key) == (SlimDXKeys.Key)ConfigIni.KeyAssign.System.Capture[i].コード)
					{
						// Debug.WriteLine( "capture: " + string.Format( "{0:2x}", (int) e.KeyCode ) + " " + (int) e.KeyCode );
						string strFullPath =
						   Path.Combine(TJAPlayer3.strEXEのあるフォルダ, "Capture_img");
						strFullPath = Path.Combine(strFullPath, DateTime.Now.ToString("yyyyMMddHHmmss") + ".png");
						SaveResultScreen(strFullPath);
					}
				}
			}
		}
		private void Window_MouseDown(object sender, OpenTK.Input.MouseButtonEventArgs e)
		{
			sw.Stop();
			if (mb.Equals(OpenTK.Input.MouseButton.Left) && ConfigIni.bIsAllowedDoubleClickFullscreen && sw.ElapsedMilliseconds <= 250 && sw.ElapsedMilliseconds > 0)//ダブルクリックイベントがないみたいなので、自力実装してごまかす
			{
				ConfigIni.bウィンドウモード = false;
				this.t全画面_ウィンドウモード切り替え();
				sw.Start();
			}
			else
			{
				mb = e.Button;
				sw.Reset();
				sw.Start();
			}
		}
		private void Window_ResizeEnd(object sender, EventArgs e)               // #23510 2010.11.20 yyagi: to get resized window size
		{
			ConfigIni.nウインドウwidth = (ConfigIni.bウィンドウモード) ? base.ClientSize.Width : GameWindowSize.Width;   // #23510 2010.10.31 yyagi add
			ConfigIni.nウインドウheight = (ConfigIni.bウィンドウモード) ? base.ClientSize.Height : GameWindowSize.Height;
		}
		private void Window_MoveEnd(object sender, EventArgs e)
		{
			if (ConfigIni.bウィンドウモード)
			{
				ConfigIni.n初期ウィンドウ開始位置X = base.X;   // #30675 2013.02.04 ikanick add
				ConfigIni.n初期ウィンドウ開始位置Y = base.Y;   //
			}

		}
		#endregion
		#endregion
	}
}
