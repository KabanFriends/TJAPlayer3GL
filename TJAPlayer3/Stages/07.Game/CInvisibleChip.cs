using System;
using FDK;

namespace TJAPlayer3
{
	public class CInvisibleChip : IDisposable
	{
		/// <summary>ミス後表示する時間(ms)</summary>
		public int nDisplayTimeMs
		{
			get;
			set;
		}
		/// <summary>表示期間終了後、フェードアウトする時間</summary>
		public int nFadeoutTimeMs
		{
			get;
			set;
		}
		/// <summary>楽器ごとのInvisibleモード</summary>
		public STDGBVALUE<EInvisible> eInvisibleMode;



		#region [ コンストラクタ ]
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="_dbDisplayTime">ミス時再表示する時間(秒)</param>
		/// <param name="_dbFadeoutTime">再表示後フェードアウトする時間(秒)</param>
		public CInvisibleChip( int _nDisplayTimeMs, int _nFadeoutTimeMs )
		{
			Initialize( _nDisplayTimeMs, _nFadeoutTimeMs );
		}
		private void Initialize( int _nDisplayTimeMs, int _nFadeoutTimeMs )
		{
			nDisplayTimeMs = _nDisplayTimeMs;
			nFadeoutTimeMs = _nFadeoutTimeMs;
			Reset();
		}
		#endregion

		/// <summary>
		/// 内部状態を初期化する
		/// </summary>
		public void Reset()
		{
			for ( int i = 0; i < 4; i++ )
			{
				ccounter[ i ] = new CCounter();
				b演奏チップが１つでもバーを通過した[ i ] = false;
			}
		}

		/// <summary>
		/// まだSemi-Invisibleを開始していなければ、開始する
		/// </summary>
		/// <param name="eInst"></param>
		public void StartSemiInvisible( E楽器パート eInst )
		{
			int nInst = (int) eInst;
			if ( !b演奏チップが１つでもバーを通過した[ nInst ] )
			{
				b演奏チップが１つでもバーを通過した[ nInst ] = true;
				if ( this.eInvisibleMode[ nInst ] == EInvisible.SEMI )
				{
					ShowChipTemporally( eInst );
					ccounter[ nInst ].n現在の値 = nDisplayTimeMs;
				}
			}
		}
		/// <summary>
		/// 一時的にチップを表示するモードを開始する
		/// </summary>
		/// <param name="eInst">楽器パート</param>
		public void ShowChipTemporally( E楽器パート eInst )
		{
			ccounter[ (int) eInst ].t開始( 0, nDisplayTimeMs + nFadeoutTimeMs + 1, 1, TJAPlayer3.Timer );
		}

		#region [ Dispose-Finalize パターン実装 ]
		//-----------------
		public void Dispose()
		{
			this.Dispose( true );
			GC.SuppressFinalize( this );
		}
		protected void Dispose( bool disposeManagedObjects )
		{
			if( this.bDispose完了済み )
				return;

			if( disposeManagedObjects )
			{
				// (A) Managed リソースの解放
				for ( int i = 0; i < 4; i++ )
				{
					// ctInvisibleTimer[ i ].Dispose();
					ccounter[ i ].t停止();
					ccounter[ i ] = null;
				}
			}

			// (B) Unamanaged リソースの解放

			this.bDispose完了済み = true;
		}
		~CInvisibleChip()
		{
			this.Dispose( false );
		}
		//-----------------
		#endregion

		private STDGBVALUE<CCounter> ccounter;
		private bool bDispose完了済み = false;
		private STDGBVALUE<bool> b演奏チップが１つでもバーを通過した;
	}
}
