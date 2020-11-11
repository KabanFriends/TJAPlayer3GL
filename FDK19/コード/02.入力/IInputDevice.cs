﻿using System;
using System.Collections.Generic;
using System.Text;

namespace FDK
{
	public interface IInputDevice : IDisposable
	{
		// プロパティ

		E入力デバイス種別 e入力デバイス種別
		{
			get;
		}
		string GUID 
		{
			get; 
		}
		int ID 
		{
			get;
		}
		List<STInputEvent> list入力イベント
		{
			get;
		}


		// メソッドインターフェース

		void tポーリング( bool bWindowがアクティブ中 );
		bool bキーが押された( int nKey );
		bool bキーが押されている( int nKey );
		bool bキーが離された( int nKey );
		bool bキーが離されている( int nKey );
	}
}
