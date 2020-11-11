﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

using SlimDXKey = SlimDXKeys.Key;

namespace FDK
{
	public class CInputKeyboard : IInputDevice, IDisposable
	{
		// コンストラクタ

		public CInputKeyboard()
		{
			this.e入力デバイス種別 = E入力デバイス種別.Keyboard;
			this.GUID = "";
			this.ID = 0;

			for (int i = 0; i < this.bKeyState.Length; i++)
				this.bKeyState[i] = false;

			//this.timer = new CTimer( CTimer.E種別.MultiMedia );
			this.list入力イベント = new List<STInputEvent>(32);
			// this.ct = new CTimer( CTimer.E種別.PerformanceCounter );
		}


		// メソッド

		#region [ IInputDevice 実装 ]
		//-----------------
		public E入力デバイス種別 e入力デバイス種別 { get; private set; }
		public string GUID { get; private set; }
		public int ID { get; private set; }
		public List<STInputEvent> list入力イベント { get; private set; }

		public void tポーリング(bool bWindowがアクティブ中)
		{
			for (int i = 0; i < 256; i++)
			{
				this.bKeyPushDown[i] = false;
				this.bKeyPullUp[i] = false;
			}

			if (bWindowがアクティブ中)
			{
				//this.list入力イベント = new List<STInputEvent>( 32 );
				this.list入力イベント.Clear();            // #xxxxx 2012.6.11 yyagi; To optimize, I removed new();
													//string d = DateTime.Now.ToString( "yyyy/MM/dd HH:mm:ss.ffff" );


				#region [ 入力 ]
				//-----------------------------
				OpenTK.Input.KeyboardState currentState = OpenTK.Input.Keyboard.GetState();

				if (currentState.IsConnected)
				{

					for (int index = 0; index < Enum.GetNames(typeof(OpenTK.Input.Key)).Length; index++)
					{
						if (currentState[(OpenTK.Input.Key)index])
						{
							var key = DeviceConstantConverter.TKKtoKey((OpenTK.Input.Key)index);
							if (SlimDXKey.Unknown == key)
								continue;   // 未対応キーは無視。


							if (this.bKeyState[(int)key] == false)
							{
								if (key != SlimDXKey.Return || (bKeyState[(int)SlimDXKey.LeftAlt] == false && bKeyState[(int)SlimDXKey.RightAlt] == false))    // #23708 2016.3.19 yyagi
								{
									var ev = new STInputEvent()
									{
										nKey = (int)key,
										b押された = true,
										b離された = false,
										nTimeStamp = CSound管理.rc演奏用タイマ.nシステム時刻, // 演奏用タイマと同じタイマを使うことで、BGMと譜面、入力ずれを防ぐ。
										nVelocity = CInput管理.n通常音量,
									};
									this.list入力イベント.Add(ev);

									this.bKeyState[(int)key] = true;
									this.bKeyPushDown[(int)key] = true;
								}

								//if ( (int) key == (int) SlimDXKey.Space )
								//{
								//    Trace.TraceInformation( "FDK(direct): SPACE key registered. " + ct.nシステム時刻 );
								//}
							}
						}
						{
							// #xxxxx: 2017.5.7: from: DIK (SharpDX.DirectInput.Key) を SlimDX.DirectInput.Key に変換。
							var key = DeviceConstantConverter.TKKtoKey((OpenTK.Input.Key)index);
							if (SlimDXKey.Unknown == key)
								continue;   // 未対応キーは無視。

							if (this.bKeyState[(int)key] == true && !currentState.IsKeyDown((OpenTK.Input.Key)index)) // 前回は押されているのに今回は押されていない → 離された
							{
								var ev = new STInputEvent()
								{
									nKey = (int)key,
									b押された = false,
									b離された = true,
									nTimeStamp = CSound管理.rc演奏用タイマ.nシステム時刻, // 演奏用タイマと同じタイマを使うことで、BGMと譜面、入力ずれを防ぐ。
									nVelocity = CInput管理.n通常音量,
								};
								this.list入力イベント.Add(ev);

								this.bKeyState[(int)key] = false;
								this.bKeyPullUp[(int)key] = true;
							}
						}
					}
				}
				//-----------------------------
				#endregion
				
			}
		}

		/// <param name="nKey">
		///		調べる SlimDX.DirectInput.Key を int にキャストした値。（SharpDX.DirectInput.Key ではないので注意。）
		/// </param>
		public bool bキーが押された(int nKey)
		{
			return this.bKeyPushDown[nKey];
		}

		/// <param name="nKey">
		///		調べる SlimDX.DirectInput.Key を int にキャストした値。（SharpDX.DirectInput.Key ではないので注意。）
		/// </param>
		public bool bキーが押されている(int nKey)
		{
			return this.bKeyState[nKey];
		}

		/// <param name="nKey">
		///		調べる SlimDX.DirectInput.Key を int にキャストした値。（SharpDX.DirectInput.Key ではないので注意。）
		/// </param>
		public bool bキーが離された(int nKey)
		{
			return this.bKeyPullUp[nKey];
		}

		/// <param name="nKey">
		///		調べる SlimDX.DirectInput.Key を int にキャストした値。（SharpDX.DirectInput.Key ではないので注意。）
		/// </param>
		public bool bキーが離されている(int nKey)
		{
			return !this.bKeyState[nKey];
		}
		//-----------------
		#endregion

		#region [ IDisposable 実装 ]
		//-----------------
		public void Dispose()
		{
			if (!this.bDispose完了済み)
			{
				if (this.list入力イベント != null)
				{
					this.list入力イベント = null;
				}
				this.bDispose完了済み = true;
			}
		}
		//-----------------
		#endregion


		// その他

		#region [ private ]
		//-----------------
		private bool bDispose完了済み;
		private bool[] bKeyPullUp = new bool[256];
		private bool[] bKeyPushDown = new bool[256];
		private bool[] bKeyState = new bool[256];
		//private CTimer timer;
		//private CTimer ct;
		//-----------------
		#endregion
	}
}
