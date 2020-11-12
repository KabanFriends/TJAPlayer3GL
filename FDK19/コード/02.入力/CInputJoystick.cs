using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

namespace FDK
{
	public class CInputJoystick : IInputDevice, IDisposable
	{
		// コンストラクタ

		public CInputJoystick(int joystickindex)
		{
			this.e入力デバイス種別 = E入力デバイス種別.Joystick;
			this.ID = joystickindex;
			this.GUID = OpenTK.Input.Joystick.GetGuid(ID).ToString();

			for (int i = 0; i < this.bButtonState.Length; i++)
				this.bButtonState[i] = false;

			//this.timer = new CTimer( CTimer.E種別.MultiMedia );

			this.list入力イベント = new List<STInputEvent>(32);
		}


		// メソッド

		#region [ IInputDevice 実装 ]
		//-----------------
		public E入力デバイス種別 e入力デバイス種別
		{
			get;
			private set;
		}
		public string GUID
		{
			get;
			private set;
		}
		public int ID
		{
			get;
			private set;
		}
		public List<STInputEvent> list入力イベント
		{
			get;
			private set;
		}

		public void tポーリング(bool bWindowがアクティブ中)
		{
			#region [ bButtonフラグ初期化 ]
			for (int i = 0; i < 256; i++)
			{
				this.bButtonPushDown[i] = false;
				this.bButtonPullUp[i] = false;
			}
			#endregion

			if (bWindowがアクティブ中)
			{

				this.list入力イベント.Clear();

				#region [ 入力 ]

				OpenTK.Input.JoystickState ButtonState = OpenTK.Input.Joystick.GetState(ID);

				if (ButtonState.IsConnected)
				{
					#region[X軸]
					if (ButtonState.GetAxis(0) < -0.5) //JoystickAxisがない？ので数値指定
					{
						if (this.bButtonState[0] == false)
						{
							STInputEvent ev = new STInputEvent()
							{
								nKey = 0,
								b押された = true,
								nTimeStamp = CSound管理.rc演奏用タイマ.nシステム時刻, // 演奏用タイマと同じタイマを使うことで、BGMと譜面、入力ずれを防ぐ。
								nVelocity = CInput管理.n通常音量
							};
							this.list入力イベント.Add(ev);

							this.bButtonState[0] = true;
							this.bButtonPushDown[0] = true;
						}
					}
					else
					{
						if (this.bButtonState[0] == true)
						{
							STInputEvent ev = new STInputEvent()
							{
								nKey = 0,
								b押された = false,
								nTimeStamp = CSound管理.rc演奏用タイマ.nシステム時刻, // 演奏用タイマと同じタイマを使うことで、BGMと譜面、入力ずれを防ぐ。
								nVelocity = CInput管理.n通常音量
							};
							this.list入力イベント.Add(ev);

							this.bButtonState[0] = false;
							this.bButtonPullUp[0] = true;
						}
					}
					if (ButtonState.GetAxis(0) > 0.5)
					{
						if (this.bButtonState[1] == false)
						{
							STInputEvent ev = new STInputEvent()
							{
								nKey = 1,
								b押された = true,
								nTimeStamp = CSound管理.rc演奏用タイマ.nシステム時刻, // 演奏用タイマと同じタイマを使うことで、BGMと譜面、入力ずれを防ぐ。
								nVelocity = CInput管理.n通常音量
							};
							this.list入力イベント.Add(ev);

							this.bButtonState[1] = true;
							this.bButtonPushDown[1] = true;
						}
					}
					else
					{
						if (this.bButtonState[1] == true)
						{
							STInputEvent ev = new STInputEvent()
							{
								nKey = 1,
								b押された = false,
								nTimeStamp = CSound管理.rc演奏用タイマ.nシステム時刻, // 演奏用タイマと同じタイマを使うことで、BGMと譜面、入力ずれを防ぐ。
								nVelocity = CInput管理.n通常音量
							};
							this.list入力イベント.Add(ev);

							this.bButtonState[1] = false;
							this.bButtonPullUp[1] = true;
						}
					}
					#endregion

					#region[Y軸]
					if (ButtonState.GetAxis(1) < -0.5) //JoystickAxisがない？ので数値指定
					{
						if (this.bButtonState[2] == false)
						{
							STInputEvent ev = new STInputEvent()
							{
								nKey = 2,
								b押された = true,
								nTimeStamp = CSound管理.rc演奏用タイマ.nシステム時刻, // 演奏用タイマと同じタイマを使うことで、BGMと譜面、入力ずれを防ぐ。
								nVelocity = CInput管理.n通常音量
							};
							this.list入力イベント.Add(ev);

							this.bButtonState[2] = true;
							this.bButtonPushDown[2] = true;
						}
					}
					else
					{
						if (this.bButtonState[2] == true)
						{
							STInputEvent ev = new STInputEvent()
							{
								nKey = 2,
								b押された = false,
								nTimeStamp = CSound管理.rc演奏用タイマ.nシステム時刻, // 演奏用タイマと同じタイマを使うことで、BGMと譜面、入力ずれを防ぐ。
								nVelocity = CInput管理.n通常音量
							};
							this.list入力イベント.Add(ev);

							this.bButtonState[2] = false;
							this.bButtonPullUp[2] = true;
						}
					}
					if (ButtonState.GetAxis(1) > 0.5)
					{
						if (this.bButtonState[3] == false)
						{
							STInputEvent ev = new STInputEvent()
							{
								nKey = 3,
								b押された = true,
								nTimeStamp = CSound管理.rc演奏用タイマ.nシステム時刻, // 演奏用タイマと同じタイマを使うことで、BGMと譜面、入力ずれを防ぐ。
								nVelocity = CInput管理.n通常音量
							};
							this.list入力イベント.Add(ev);

							this.bButtonState[3] = true;
							this.bButtonPushDown[3] = true;
						}
					}
					else
					{
						if (this.bButtonState[3] == true)
						{
							STInputEvent ev = new STInputEvent()
							{
								nKey = 3,
								b押された = false,
								nTimeStamp = CSound管理.rc演奏用タイマ.nシステム時刻, // 演奏用タイマと同じタイマを使うことで、BGMと譜面、入力ずれを防ぐ。
								nVelocity = CInput管理.n通常音量
							};
							this.list入力イベント.Add(ev);

							this.bButtonState[3] = false;
							this.bButtonPullUp[3] = true;
						}
					}
					#endregion

					#region[Z軸]
					if (ButtonState.GetAxis(2) < -0.5) //JoystickAxisがない？ので数値指定
					{
						if (this.bButtonState[4] == false)
						{
							STInputEvent ev = new STInputEvent()
							{
								nKey = 4,
								b押された = true,
								nTimeStamp = CSound管理.rc演奏用タイマ.nシステム時刻, // 演奏用タイマと同じタイマを使うことで、BGMと譜面、入力ずれを防ぐ。
								nVelocity = CInput管理.n通常音量
							};
							this.list入力イベント.Add(ev);

							this.bButtonState[4] = true;
							this.bButtonPushDown[4] = true;
						}
					}
					else
					{
						if (this.bButtonState[4] == true)
						{
							STInputEvent ev = new STInputEvent()
							{
								nKey = 4,
								b押された = false,
								nTimeStamp = CSound管理.rc演奏用タイマ.nシステム時刻, // 演奏用タイマと同じタイマを使うことで、BGMと譜面、入力ずれを防ぐ。
								nVelocity = CInput管理.n通常音量
							};
							this.list入力イベント.Add(ev);

							this.bButtonState[4] = false;
							this.bButtonPullUp[4] = true;
						}
					}
					if (ButtonState.GetAxis(2) > 0.5)
					{
						if (this.bButtonState[5] == false)
						{
							STInputEvent ev = new STInputEvent()
							{
								nKey = 5,
								b押された = true,
								nTimeStamp = CSound管理.rc演奏用タイマ.nシステム時刻, // 演奏用タイマと同じタイマを使うことで、BGMと譜面、入力ずれを防ぐ。
								nVelocity = CInput管理.n通常音量
							};
							this.list入力イベント.Add(ev);

							this.bButtonState[5] = true;
							this.bButtonPushDown[5] = true;
						}
					}
					else
					{
						if (this.bButtonState[5] == true)
						{
							STInputEvent ev = new STInputEvent()
							{
								nKey = 5,
								b押された = false,
								nTimeStamp = CSound管理.rc演奏用タイマ.nシステム時刻, // 演奏用タイマと同じタイマを使うことで、BGMと譜面、入力ずれを防ぐ。
								nVelocity = CInput管理.n通常音量
							};
							this.list入力イベント.Add(ev);

							this.bButtonState[5] = false;
							this.bButtonPullUp[5] = true;
						}
					}
					#endregion

					#region[Z軸回転]
					if (ButtonState.GetAxis(3) < -0.5) //JoystickAxisがない？ので数値指定
					{
						if (this.bButtonState[6] == false)
						{
							STInputEvent ev = new STInputEvent()
							{
								nKey = 6,
								b押された = true,
								nTimeStamp = CSound管理.rc演奏用タイマ.nシステム時刻, // 演奏用タイマと同じタイマを使うことで、BGMと譜面、入力ずれを防ぐ。
								nVelocity = CInput管理.n通常音量
							};
							this.list入力イベント.Add(ev);

							this.bButtonState[6] = true;
							this.bButtonPushDown[6] = true;
						}
					}
					else
					{
						if (this.bButtonState[6] == true)
						{
							STInputEvent ev = new STInputEvent()
							{
								nKey = 6,
								b押された = false,
								nTimeStamp = CSound管理.rc演奏用タイマ.nシステム時刻, // 演奏用タイマと同じタイマを使うことで、BGMと譜面、入力ずれを防ぐ。
								nVelocity = CInput管理.n通常音量
							};
							this.list入力イベント.Add(ev);

							this.bButtonState[6] = false;
							this.bButtonPullUp[6] = true;
						}
					}
					if (ButtonState.GetAxis(3) > 0.5)
					{
						if (this.bButtonState[7] == false)
						{
							STInputEvent ev = new STInputEvent()
							{
								nKey = 7,
								b押された = true,
								nTimeStamp = CSound管理.rc演奏用タイマ.nシステム時刻, // 演奏用タイマと同じタイマを使うことで、BGMと譜面、入力ずれを防ぐ。
								nVelocity = CInput管理.n通常音量
							};
							this.list入力イベント.Add(ev);

							this.bButtonState[7] = true;
							this.bButtonPushDown[7] = true;
						}
					}
					else
					{
						if (this.bButtonState[7] == true)
						{
							STInputEvent ev = new STInputEvent()
							{
								nKey = 7,
								b押された = false,
								nTimeStamp = CSound管理.rc演奏用タイマ.nシステム時刻, // 演奏用タイマと同じタイマを使うことで、BGMと譜面、入力ずれを防ぐ。
								nVelocity = CInput管理.n通常音量
							};
							this.list入力イベント.Add(ev);

							this.bButtonState[7] = false;
							this.bButtonPullUp[7] = true;
						}
					}
					#endregion

					#region[POV]
					OpenTK.Input.JoystickHatState hatState = ButtonState.GetHat(OpenTK.Input.JoystickHat.Hat0);

					for (int i = 0; i < Enum.GetNames(typeof(OpenTK.Input.HatPosition)).Length; i++)
					{
						if (hatState.Position == (OpenTK.Input.HatPosition)i + 1)
						{
							if (this.bButtonState[8 + 128 + i] == false)
							{
								STInputEvent stevent = new STInputEvent()
								{
									nKey = 8 + 128 + i,
									//Debug.WriteLine( "POVS:" + povs[ 0 ].ToString( CultureInfo.CurrentCulture ) + ", " +stevent.nKey );
									b押された = true,
									nTimeStamp = CSound管理.rc演奏用タイマ.nシステム時刻, // 演奏用タイマと同じタイマを使うことで、BGMと譜面、入力ずれを防ぐ。
									nVelocity = CInput管理.n通常音量
								};
								this.list入力イベント.Add(stevent);

								this.bButtonState[stevent.nKey] = true;
								this.bButtonPushDown[stevent.nKey] = true;
							}
						}
						else
						{
							if (this.bButtonState[8 + 128 + i] == true)
							{
								STInputEvent stevent = new STInputEvent()
								{
									nKey = 8 + 128 + i,
									b押された = false,
									nTimeStamp = CSound管理.rc演奏用タイマ.nシステム時刻, // 演奏用タイマと同じタイマを使うことで、BGMと譜面、入力ずれを防ぐ。
									nVelocity = 0
								};
								this.list入力イベント.Add(stevent);

								this.bButtonState[stevent.nKey] = false;
								this.bButtonPullUp[stevent.nKey] = true;
							}
						}
					}

					#endregion

					#region[Button]
					for (int index = 0; index < 128; index++)
					{

						if (ButtonState.IsButtonDown(index))
						{
							if (this.bButtonState[8 + index] == false)
							{
								STInputEvent item = new STInputEvent()
								{
									nKey = 8 + index,
									b押された = true,
									nTimeStamp = CSound管理.rc演奏用タイマ.nシステム時刻, // 演奏用タイマと同じタイマを使うことで、BGMと譜面、入力ずれを防ぐ。
									nVelocity = CInput管理.n通常音量
								};
								this.list入力イベント.Add(item);

								this.bButtonState[8 + index] = true;
								this.bButtonPushDown[8 + index] = true;
							}
						}
						else
						{
							if (this.bButtonState[8 + index] == true)
							{
								STInputEvent item = new STInputEvent()
								{
									nKey = 8 + index,
									b押された = false,
									nTimeStamp = CSound管理.rc演奏用タイマ.nシステム時刻, // 演奏用タイマと同じタイマを使うことで、BGMと譜面、入力ずれを防ぐ。
									nVelocity = CInput管理.n通常音量
								};
								this.list入力イベント.Add(item);

								this.bButtonState[8 + index] = false;
								this.bButtonPullUp[8 + index] = true;
							}
						}
					}
					#endregion
				}
				#endregion
			}
		}

		public bool bキーが押された(int nButton)
		{
			return this.bButtonPushDown[nButton];
		}
		public bool bキーが押されている(int nButton)
		{
			return this.bButtonState[nButton];
		}
		public bool bキーが離された(int nButton)
		{
			return this.bButtonPullUp[nButton];
		}
		public bool bキーが離されている(int nButton)
		{
			return !this.bButtonState[nButton];
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
		private bool[] bButtonPullUp = new bool[0x100];
		private bool[] bButtonPushDown = new bool[0x100];
		private bool[] bButtonState = new bool[0x100];      // 0-5: XYZ, 6 - 0x128+5: buttons, 0x128+6 - 0x128+6+8: POV/HAT
		private bool bDispose完了済み;
		//private CTimer timer;
		//-----------------
		#endregion
	}
}
