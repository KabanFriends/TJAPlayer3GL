﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Runtime.InteropServices;
using System.Threading;
using System.Diagnostics;
using System.Windows.Forms;
using System.IO;
using FDK;
using System.Reflection;

namespace TJAPlayer3
{
	internal class Program
	{
		#region [ 二重起動チェック、DLL存在チェック ]
		//-----------------------------
		private static Mutex mutex二重起動防止用;

		private static bool tDLLの存在チェック( string strDll名, string str存在しないときに表示するエラー文字列jp, string str存在しないときに表示するエラー文字列en, bool bLoadDllCheck )
		{
			string str存在しないときに表示するエラー文字列 = ( CultureInfo.CurrentUICulture.TwoLetterISOLanguageName == "ja" ) ?
				str存在しないときに表示するエラー文字列jp : str存在しないときに表示するエラー文字列en;
			if ( bLoadDllCheck )
			{
				IntPtr hModule = LoadLibrary( strDll名 );		// 実際にLoadDll()してチェックする
				if ( hModule == IntPtr.Zero )
				{
					MessageBox.Show( str存在しないときに表示するエラー文字列, "DTXMania runtime error", MessageBoxButtons.OK, MessageBoxIcon.Hand );
					return false;
				}
				FreeLibrary( hModule );
			}
			else
			{													// 単純にファイルの存在有無をチェックするだけ (プロジェクトで「参照」していたり、アンマネージドなDLLが暗黙リンクされるものはこちら)
				string path = Path.Combine( System.IO.Directory.GetCurrentDirectory(), strDll名 );
				if ( !File.Exists( path ) )
				{
					MessageBox.Show( str存在しないときに表示するエラー文字列, "DTXMania runtime error", MessageBoxButtons.OK, MessageBoxIcon.Hand );
					return false;
				}
			}
			return true;
		}
		private static bool tDLLの存在チェック( string strDll名, string str存在しないときに表示するエラー文字列jp, string str存在しないときに表示するエラー文字列en )
		{
			return true;
			//return tDLLの存在チェック( strDll名, str存在しないときに表示するエラー文字列jp, str存在しないときに表示するエラー文字列en, false );
		}

		#region [DllImport]
		[DllImport( "kernel32", CharSet = CharSet.Unicode, SetLastError = true )]
		internal static extern void FreeLibrary( IntPtr hModule );

		[DllImport( "kernel32", CharSet = CharSet.Unicode, SetLastError = true )]
		internal static extern IntPtr LoadLibrary( string lpFileName );
		#endregion
		//-----------------------------
		#endregion

		[STAThread] 
		private static void Main()
		{
			mutex二重起動防止用 = new Mutex( false, "DTXManiaMutex" );

			if ( mutex二重起動防止用.WaitOne( 0, false ) )
			{
				string newLine = Environment.NewLine;
				bool bDLLnotfound = false;

				Trace.WriteLine( "Current Directory: " + Environment.CurrentDirectory );
				Trace.WriteLine( "EXEのあるフォルダ: " + Path.GetDirectoryName( Application.ExecutablePath ) );

                Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;

				if ( !bDLLnotfound )
				{
#if DEBUG && TEST_ENGLISH
					Thread.CurrentThread.CurrentCulture = new CultureInfo( "en-US" );
#endif

					//DWM.EnableComposition( false );	// Disable AeroGrass temporally

					// BEGIN #23670 2010.11.13 from: キャッチされない例外は放出せずに、ログに詳細を出力する。
					// BEGIM #24606 2011.03.08 from: DEBUG 時は例外発生箇所を直接デバッグできるようにするため、例外をキャッチしないようにする。
#if !DEBUG
					try
#endif
					{
						using ( var mania = new TJAPlayer3() )
							mania.Run();

						Trace.WriteLine( "" );
						Trace.WriteLine( "遊んでくれてありがとう！" );
					}
#if !DEBUG
					catch( Exception e )
					{
						Trace.WriteLine( "" );
						Trace.Write( e.ToString() );
						Trace.WriteLine( "" );
						Trace.WriteLine( "エラーだゴメン！（涙" );
                        AssemblyName asmApp = Assembly.GetExecutingAssembly().GetName();
                        MessageBox.Show( "エラーが発生しました。\n" +
                            "原因がわからない場合は、以下のエラー文を添えて、エラー送信フォームに送信してください。\n" + 
                            e.ToString(), asmApp.Name + " Ver." + asmApp.Version.ToString().Substring(0, asmApp.Version.ToString().Length - 2) + " Error", MessageBoxButtons.OK, MessageBoxIcon.Error );	// #23670 2011.2.28 yyagi to show error dialog
                        DialogResult result = MessageBox.Show("エラー送信フォームを開きますか?(ブラウザが起動します)",
                            asmApp.Name + " Ver." + asmApp.Version.ToString().Substring(0, asmApp.Version.ToString().Length - 2),
                            MessageBoxButtons.YesNo,
                            MessageBoxIcon.Asterisk);
                        if(result == DialogResult.Yes)
                        {
                            Process.Start("https://docs.google.com/forms/d/e/1FAIpQLScr_Oqs9WKnonQyxpEVt7gZYPcjjIfN3SjgqWPvxfw95nAQ6g/viewform?usp=pp_url&entry.60593436=" + System.Web.HttpUtility.UrlEncode(e.ToString()));
                        }

					}
#endif
					// END #24606 2011.03.08 from
					// END #23670 2010.11.13 from

					if ( Trace.Listeners.Count > 1 )
						Trace.Listeners.RemoveAt( 1 );
				}

				// BEGIN #24615 2011.03.09 from: Mutex.WaitOne() が true を返した場合は、Mutex のリリースが必要である。

				mutex二重起動防止用.ReleaseMutex();
				mutex二重起動防止用 = null;

				// END #24615 2011.03.09 from
			}
			else        // DTXManiaが既に起動中
			{
				Environment.Exit(0);
			}
		}
	}
}
