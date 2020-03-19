using System;
using System.Runtime.InteropServices;

namespace FDK
{
	public class CWin32
	{
		#region [ Win32 定数 ]
		//-----------------
		public const int S_OK = 0x00000000;
		public const int S_FALSE = 0x00000001;
		public const int E_FAIL = unchecked( (int) 0x80004005 );

		#region [ MIDIメッセージ ]
		public const uint MIM_DATA = 0x3c3;
		#endregion

        #region [ DirectShow, VFW 関連 ]
		//-----------------
		public const int VFW_E_NOT_CONNECTED = unchecked( (int) 0x80040209 );
        public const int VFW_S_STATE_INTERMEDIATE = 0x00040237;
        //-----------------
		#endregion

		#region [ Windowsメッセージ ]
        public const uint WM_APP = 0x00008000;
        #endregion

		[Flags]
		internal enum ExecutionState : uint
		{
			Null = 0,					// 関数が失敗した時の戻り値
			SystemRequired = 1,			// スタンバイを抑止
			DisplayRequired = 2,		// 画面OFFを抑止
			Continuous = 0x80000000,	// 効果を永続させる。ほかオプションと併用する。
		}
		//-----------------
		#endregion

		#region [ Win32 関数 ]
		//-----------------
		[DllImport( "winmm.dll" )]
		public static extern uint midiInClose( uint hMidiIn );
		[DllImport( "winmm.dll" )]
		public static extern uint midiInGetDevCaps( uint uDeviceID, ref MIDIINCAPS lpMidiInCaps, uint cbMidiInCaps );
		[DllImport( "winmm.dll" )]
		public static extern uint midiInGetNumDevs();
		[DllImport( "winmm.dll" )]
		public static extern uint midiInOpen( ref uint phMidiIn, uint uDeviceID, MidiInProc dwCallback, int dwInstance, int fdwOpen );
		[DllImport( "winmm.dll" )]
		public static extern uint midiInReset( uint hMidiIn );
		[DllImport( "winmm.dll" )]
		public static extern uint midiInStart( uint hMidiIn );
		[DllImport( "winmm.dll" )]
		public static extern uint midiInStop( uint hMidiIn );
		[DllImport( "Kernel32.Dll" )]
		public static unsafe extern void CopyMemory( void* pDest, void* pSrc, uint numOfBytes );

		[DllImport( "kernel32.dll" )]
		internal static extern ExecutionState SetThreadExecutionState( ExecutionState esFlags );
		//-----------------
		#endregion

		#region [ Win32 構造体 ]
		//-----------------

		[StructLayout( LayoutKind.Sequential )]
		public struct MIDIINCAPS
		{
			public ushort wMid;
			public ushort wPid;
			public uint vDriverVersion;
			[MarshalAs( UnmanagedType.ByValTStr, SizeConst = 0x20 )]
			public string szPname;
			public uint dwSupport;
		}

		[StructLayout( LayoutKind.Sequential )]
		public struct WAVEFORMATEX
		{
			public ushort wFormatTag;
			public ushort nChannels;
			public uint nSamplesPerSec;
			public uint nAvgBytesPerSec;
			public ushort nBlockAlign;
			public ushort wBitsPerSample;
			public ushort cbSize;
		}

        //-----------------
		#endregion

        // Win32 メッセージ処理デリゲート

		public delegate void MidiInProc( uint hMidiIn, uint wMsg, int dwInstance, int dwParam1, int dwParam2 );
	}
}
