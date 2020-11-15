using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace FDK
{
	public class CInput管理 : IDisposable
	{
		// 定数

		public static int n通常音量 = 110;


		// プロパティ

		public List<IInputDevice> list入力デバイス
		{
			get;
			private set;
		}
		public IInputDevice Keyboard
		{
			get
			{
				if (this._Keyboard != null)
				{
					return this._Keyboard;
				}
				foreach (IInputDevice device in this.list入力デバイス)
				{
					if (device.e入力デバイス種別 == E入力デバイス種別.Keyboard)
					{
						this._Keyboard = device;
						return device;
					}
				}
				return null;
			}
		}
		public IInputDevice Mouse
		{
			get
			{
				if (this._Mouse != null)
				{
					return this._Mouse;
				}
				foreach (IInputDevice device in this.list入力デバイス)
				{
					if (device.e入力デバイス種別 == E入力デバイス種別.Mouse)
					{
						this._Mouse = device;
						return device;
					}
				}
				return null;
			}
		}


		// コンストラクタ
		public CInput管理()
		{
			// this.timer = new CTimer( CTimer.E種別.MultiMedia );

			this.list入力デバイス = new List<IInputDevice>(10);
			#region [ Enumerate keyboard/mouse: exception is masked if keyboard/mouse is not connected ]
			CInputKeyboard cinputkeyboard = null;
			CInputMouse cinputmouse = null;
			try
			{
				cinputkeyboard = new CInputKeyboard();
				cinputmouse = new CInputMouse();
			}
			catch
			{
			}
			if (cinputkeyboard != null)
			{
				this.list入力デバイス.Add(cinputkeyboard);
			}
			if (cinputmouse != null)
			{
				this.list入力デバイス.Add(cinputmouse);
			}
			#endregion
			#region [ Enumerate joypad ]
			try
			{
				for (int joynum = 0; joynum < 8; joynum++)//2020.06.28 Mr-Ojii joystickの検出数を返す関数が見つからないので適当に8個で
				{
					if (OpenTK.Input.Joystick.GetState(joynum).IsConnected)
						this.list入力デバイス.Add(new CInputJoystick(joynum));
				}
			}
			catch (Exception e)
			{
				Trace.WriteLine(e.ToString());
			}
			#endregion
		}


		// メソッド

		public IInputDevice Joystick(int ID)
		{
			foreach (IInputDevice device in this.list入力デバイス)
			{
				if ((device.e入力デバイス種別 == E入力デバイス種別.Joystick) && (device.ID == ID))
				{
					return device;
				}
			}
			return null;
		}

		public void tポーリング(bool bWindowがアクティブ中)
		{
				//				foreach( IInputDevice device in this.list入力デバイス )
			for (int i = this.list入力デバイス.Count - 1; i >= 0; i--)    // #24016 2011.1.6 yyagi: change not to use "foreach" to avoid InvalidOperation exception by Remove().
			{
				IInputDevice device = this.list入力デバイス[i];
				try
				{
					device.tポーリング(bWindowがアクティブ中);
				}
				catch (Exception e)                                      // #24016 2011.1.6 yyagi: catch exception for unplugging USB joystick, and remove the device object from the polling items.
				{
					Trace.WriteLine(e.ToString());//2020.06.28 isconnectを使用しているので、デバイスを消さない。
				}
			}
		}

		#region [ IDisposable＋α ]
		//-----------------
		public void Dispose()
		{
			this.Dispose(true);
		}
		public void Dispose(bool disposeManagedObjects)
		{
			if (!this.bDisposed済み)
			{
				if (disposeManagedObjects)
				{
					foreach (IInputDevice device in this.list入力デバイス)
					{
						device.Dispose();
					}
					foreach (IInputDevice device2 in this.list入力デバイス)
					{
						device2.Dispose();
					}

					this.list入力デバイス.Clear();

					//if( this.timer != null )
					//{
					//    this.timer.Dispose();
					//    this.timer = null;
					//}
				}
				this.bDisposed済み = true;
			}
		}
		~CInput管理()
		{
			this.Dispose(false);
			GC.KeepAlive(this);
		}
		//-----------------
		#endregion


		// その他

		#region [ private ]
		//-----------------
		private IInputDevice _Keyboard;
		private IInputDevice _Mouse;
		private bool bDisposed済み;
		//		private CTimer timer;

		//-----------------
		#endregion
	}
}
