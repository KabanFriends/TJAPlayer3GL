using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using TJAPlayer3.C曲リストノードComparers;
using FDK;
using FDK.ExtensionMethods;

namespace TJAPlayer3
{
	[Serializable]
	internal class CSongs管理
	{
		// プロパティ

		public int nSongsDBから取得できたスコア数
		{
			get; 
			set; 
		}
		public int nSongsDBへ出力できたスコア数
		{
			get;
			set;
		}
		public int nスコアキャッシュから反映できたスコア数 
		{
			get;
			set; 
		}
		public int nファイルから反映できたスコア数
		{
			get;
			set;
		}
		public int n検索されたスコア数 
		{ 
			get;
			set;
		}
		public int n検索された曲ノード数
		{
			get; 
			set;
		}
		[NonSerialized]
		public List<Cスコア> listSongsDB;					// songs.dbから構築されるlist
		public List<C曲リストノード> list曲ルート;			// 起動時にフォルダ検索して構築されるlist
		public bool bIsSuspending							// 外部スレッドから、内部スレッドのsuspendを指示する時にtrueにする
		{													// 再開時は、これをfalseにしてから、次のautoReset.Set()を実行する
			get;
			set;
		}
		public bool bIsSlowdown								// #PREMOVIE再生時に曲検索を遅くする
		{
			get;
			set;
		}
		[NonSerialized]
		private AutoResetEvent autoReset;
		public AutoResetEvent AutoReset
		{
			get
			{
				return autoReset;
			}
			private set
			{
				autoReset = value;
			}
		}

		private int searchCount;							// #PREMOVIE中は検索n回実行したら少しスリープする

		// コンストラクタ

		public CSongs管理()
		{
			this.listSongsDB = new List<Cスコア>();
			this.list曲ルート = new List<C曲リストノード>();
			this.n検索された曲ノード数 = 0;
			this.n検索されたスコア数 = 0;
			this.bIsSuspending = false;						// #27060
			this.autoReset = new AutoResetEvent( true );	// #27060
			this.searchCount = 0;
		}


		// メソッド

		#region [ SongsDB(songs.db) を読み込む ]
		//-----------------
		public void tSongsDBを読み込む( string SongsDBファイル名 )
		{
			this.nSongsDBから取得できたスコア数 = 0;
			if( File.Exists( SongsDBファイル名 ) )
			{
				BinaryReader br = null;
				try
				{
					br = new BinaryReader( File.OpenRead( SongsDBファイル名 ) );
					if ( !br.ReadString().Equals( SONGSDB_VERSION ) )
					{
						throw new InvalidDataException( "ヘッダが異なります。" );
					}
					this.listSongsDB = new List<Cスコア>();

					while( true )
					{
						try
						{
							Cスコア item = this.tSongsDBからスコアを１つ読み込む( br );
							this.listSongsDB.Add( item );
							this.nSongsDBから取得できたスコア数++;
						}
						catch( EndOfStreamException )
						{
							break;
						}
					}
				}
				finally
				{
					if( br != null )
						br.Close();
				}
			}
		}
		//-----------------
		#endregion

		#region [ 曲を検索してリストを作成する ]
		//-----------------
		public void t曲を検索してリストを作成する( string str基点フォルダ, bool b子BOXへ再帰する )
		{
			this.t曲を検索してリストを作成する( str基点フォルダ, b子BOXへ再帰する, this.list曲ルート, null );
		}
		private void t曲を検索してリストを作成する( string str基点フォルダ, bool b子BOXへ再帰する, List<C曲リストノード> listノードリスト, C曲リストノード node親 )
		{
			if( !str基点フォルダ.EndsWith( @"\" ) )
				str基点フォルダ = str基点フォルダ + @"\";

			DirectoryInfo info = new DirectoryInfo( str基点フォルダ );

			if( TJAPlayer3.ConfigIni.bLog曲検索ログ出力 )
				Trace.TraceInformation( "基点フォルダ: " + str基点フォルダ );

            var rawFileInfos = info.GetFiles();
            var playlistFileInfos = GetPlaylistFileInfos(rawFileInfos);
            var fileInfoIndexPairs = playlistFileInfos
                .Select((o, i) => new KeyValuePair<FileInfo, int?>(o, i))
                .Concat(rawFileInfos
                    .Select(o => new KeyValuePair<FileInfo, int?>(o, null))
                )
                .ToList();

			#region [ a.フォルダ内に set.def が存在する場合 → 1フォルダ内のtjaファイル無制限]
			//-----------------------------
			string path = str基点フォルダ + "set.def";
            if( File.Exists( path ) )
			{
				new FileInfo( path );
				if( TJAPlayer3.ConfigIni.bLog曲検索ログ出力 )
				{
					Trace.TraceInformation( "set.def検出 : {0}", path );
					Trace.Indent();
				}
				try
				{
                    foreach( var kvp in fileInfoIndexPairs )
                    {
                        var fileinfo = kvp.Key;
                        var index = kvp.Value;

					    SlowOrSuspendSearchTask();
                        #region[ 拡張子を取得 ]
					    string strExt = fileinfo.Extension.ToLower();
                        #endregion
                        if( ( strExt.Equals( ".tja" ) || strExt.Equals( ".dtx" ) ) )
                        {
                            if( strExt.Equals( ".tja" ) )
                            {
                                //tja、dtxが両方存在していた場合、tjaを読み込まずにtjaと同名のdtxだけを使う。
                                string dtxscoreini = Path.ChangeExtension(fileinfo.FullName, ".dtx");
                                if( File.Exists( dtxscoreini ) )
                                {
                                    continue;
                                }
                            }

                            #region[ 新処理 ]
                            CDTX dtx = new CDTX( fileinfo.FullName, false, 1.0, 0, 1 );
                            C曲リストノード c曲リストノード = new C曲リストノード();
                            c曲リストノード.nIndex = index;
                            c曲リストノード.eノード種別 = C曲リストノード.Eノード種別.SCORE;

                            var isPlaylistEntry = index != null;
                            var boxdef = isPlaylistEntry ? CBoxDef.Find(fileinfo.Directory) : null;

                            bool b = false;
                            for( int n = 0; n < (int)Difficulty.Total; n++ )
                            {
                                if( dtx.b譜面が存在する[ n ] )
                                {
                                    c曲リストノード.nスコア数++;
                                    c曲リストノード.r親ノード = node親;
                                    c曲リストノード.strBreadcrumbs = ( c曲リストノード.r親ノード == null ) ?
                                    fileinfo.FullName : c曲リストノード.r親ノード.strBreadcrumbs + " > " + fileinfo.FullName;

                                    c曲リストノード.strタイトル = dtx.TITLE;
                                    c曲リストノード.strサブタイトル = dtx.SUBTITLE;

                                    c曲リストノード.strジャンル = dtx.GENRE.ToNullIfEmpty() ?? boxdef?.Genre ?? c曲リストノード.r親ノード?.strジャンル;
                                    c曲リストノード.ForeColor = boxdef?.ForeColor ?? c曲リストノード.r親ノード?.ForeColor ?? c曲リストノード.ForeColor;
                                    c曲リストノード.BackColor = boxdef?.BackColor ?? c曲リストノード.r親ノード?.BackColor ?? c曲リストノード.BackColor;

                                    switch (CStrジャンルtoNum.ForAC15SortOrder(c曲リストノード.strジャンル))
                                    {
                                        case EジャンルAC15SortOrder.JPOP:
                                            c曲リストノード.ForeColor = TJAPlayer3.Skin.SongSelect_ForeColor_JPOP;
                                            c曲リストノード.BackColor = TJAPlayer3.Skin.SongSelect_BackColor_JPOP;
                                            break;
                                        case EジャンルAC15SortOrder.アニメ:
                                            c曲リストノード.ForeColor = TJAPlayer3.Skin.SongSelect_ForeColor_Anime;
                                            c曲リストノード.BackColor = TJAPlayer3.Skin.SongSelect_BackColor_Anime;
                                            break;
                                        case EジャンルAC15SortOrder.ボーカロイド:
                                            c曲リストノード.ForeColor = TJAPlayer3.Skin.SongSelect_ForeColor_VOCALOID;
                                            c曲リストノード.BackColor = TJAPlayer3.Skin.SongSelect_BackColor_VOCALOID;
                                            break;
                                        case EジャンルAC15SortOrder.どうよう:
                                            c曲リストノード.ForeColor = TJAPlayer3.Skin.SongSelect_ForeColor_Children;
                                            c曲リストノード.BackColor = TJAPlayer3.Skin.SongSelect_BackColor_Children;
                                            break;
                                        case EジャンルAC15SortOrder.バラエティ:
                                            c曲リストノード.ForeColor = TJAPlayer3.Skin.SongSelect_ForeColor_Variety;
                                            c曲リストノード.BackColor = TJAPlayer3.Skin.SongSelect_BackColor_Variety;
                                            break;
                                        case EジャンルAC15SortOrder.クラシック:
                                            c曲リストノード.ForeColor = TJAPlayer3.Skin.SongSelect_ForeColor_Classic;
                                            c曲リストノード.BackColor = TJAPlayer3.Skin.SongSelect_BackColor_Classic;
                                            break;
                                        case EジャンルAC15SortOrder.ゲームミュージック:
                                            c曲リストノード.ForeColor = TJAPlayer3.Skin.SongSelect_ForeColor_GameMusic;
                                            c曲リストノード.BackColor = TJAPlayer3.Skin.SongSelect_BackColor_GameMusic;
                                            break;
                                        case EジャンルAC15SortOrder.ナムコオリジナル:
                                            c曲リストノード.ForeColor = TJAPlayer3.Skin.SongSelect_ForeColor_Namco;
                                            c曲リストノード.BackColor = TJAPlayer3.Skin.SongSelect_BackColor_Namco;
                                            break;
                                        default:
                                            break;
                                    }


                                    c曲リストノード.nLevel = dtx.LEVELtaiko;
                                    
                                    c曲リストノード.arスコア[ n ] = new Cスコア();
                                    c曲リストノード.arスコア[ n ].ファイル情報.ファイルの絶対パス = fileinfo.FullName;
                                    c曲リストノード.arスコア[ n ].ファイル情報.フォルダの絶対パス = fileinfo.DirectoryName + Path.DirectorySeparatorChar;
                                    c曲リストノード.arスコア[ n ].ファイル情報.ファイルサイズ = fileinfo.Length;
                                    c曲リストノード.arスコア[ n ].ファイル情報.最終更新日時 = fileinfo.LastWriteTime;
                                    string strFileNameScoreIni = c曲リストノード.arスコア[ n ].ファイル情報.ファイルの絶対パス + ".score.ini";
                                    if( File.Exists( strFileNameScoreIni ) )
                                    {
                                        FileInfo infoScoreIni = new FileInfo( strFileNameScoreIni );
                                        c曲リストノード.arスコア[ n ].ScoreIni情報.ファイルサイズ = infoScoreIni.Length;
                                        c曲リストノード.arスコア[ n ].ScoreIni情報.最終更新日時 = infoScoreIni.LastWriteTime;
                                    }
                                    if( b == false )
                                    {
                                        this.n検索されたスコア数++;
                                        listノードリスト.Add( c曲リストノード );
                                        this.n検索された曲ノード数++;
                                        b = true;
                                    }
                                }
                            }
                            dtx = null;
                        }
                        #endregion
                    }
				}
				finally
				{
					if( TJAPlayer3.ConfigIni.bLog曲検索ログ出力 )
					{
						Trace.Unindent();
					}
				}
			}
			//-----------------------------
			#endregion

			#region [ b.フォルダ内に set.def が存在しない場合 → 個別ファイルからノード作成 ]
			//-----------------------------
            else
			{
				foreach( var kvp in fileInfoIndexPairs )
				{
                    var fileinfo = kvp.Key;
                    var index = kvp.Value;

					SlowOrSuspendSearchTask();		// #27060 中断要求があったら、解除要求が来るまで待機, #PREMOVIE再生中は検索負荷を落とす
					string strExt = fileinfo.Extension.ToLower();

                    if( ( strExt.Equals( ".tja" ) || strExt.Equals( ".dtx" ) ) )
                    {
                        // 2017.06.02 kairera0467 廃止。
                        //if( strExt.Equals( ".tja" ) )
                        //{
                        //    //tja、dtxが両方存在していた場合、tjaを読み込まずにdtxだけ使う。
                        //    string[] dtxscoreini = Directory.GetFiles( str基点フォルダ, "*.dtx");
                        //    if(dtxscoreini.Length != 0 )
                        //    {
                        //        continue;
                        //    }
                        //}

                        #region[ 新処理 ]
                        CDTX dtx = new CDTX( fileinfo.FullName, false, 1.0, 0, 0 );
                        C曲リストノード c曲リストノード = new C曲リストノード();
                        c曲リストノード.nIndex = index;
                        c曲リストノード.eノード種別 = C曲リストノード.Eノード種別.SCORE;

                        var isPlaylistEntry = index != null;
                        var boxdef = isPlaylistEntry ? CBoxDef.Find(fileinfo.Directory) : null;

                        bool b = false;
                        for( int n = 0; n < (int)Difficulty.Total; n++ )
                        {
                            if( dtx.b譜面が存在する[ n ] )
                            {
                                c曲リストノード.nスコア数++;
                                c曲リストノード.r親ノード = node親;
                                c曲リストノード.strBreadcrumbs = ( c曲リストノード.r親ノード == null ) ?
                                    fileinfo.FullName : c曲リストノード.r親ノード.strBreadcrumbs + " > " + fileinfo.FullName;

                                c曲リストノード.strタイトル = dtx.TITLE;
                                c曲リストノード.strサブタイトル = dtx.SUBTITLE;
                                c曲リストノード.strジャンル = dtx.GENRE;

                                c曲リストノード.strジャンル = dtx.GENRE.ToNullIfEmpty() ?? boxdef?.Genre ?? c曲リストノード.r親ノード?.strジャンル;
                                c曲リストノード.ForeColor = boxdef?.ForeColor ?? c曲リストノード.r親ノード?.ForeColor ?? c曲リストノード.ForeColor;
                                c曲リストノード.BackColor = boxdef?.BackColor ?? c曲リストノード.r親ノード?.BackColor ?? c曲リストノード.BackColor;

                                switch (CStrジャンルtoNum.ForAC15SortOrder(c曲リストノード.strジャンル))
                                {
                                    case EジャンルAC15SortOrder.JPOP:
                                        c曲リストノード.ForeColor = TJAPlayer3.Skin.SongSelect_ForeColor_JPOP;
                                        c曲リストノード.BackColor = TJAPlayer3.Skin.SongSelect_BackColor_JPOP;
                                        break;
                                    case EジャンルAC15SortOrder.アニメ:
                                        c曲リストノード.ForeColor = TJAPlayer3.Skin.SongSelect_ForeColor_Anime;
                                        c曲リストノード.BackColor = TJAPlayer3.Skin.SongSelect_BackColor_Anime;
                                        break;
                                    case EジャンルAC15SortOrder.ボーカロイド:
                                        c曲リストノード.ForeColor = TJAPlayer3.Skin.SongSelect_ForeColor_VOCALOID;
                                        c曲リストノード.BackColor = TJAPlayer3.Skin.SongSelect_BackColor_VOCALOID;
                                        break;
                                    case EジャンルAC15SortOrder.どうよう:
                                        c曲リストノード.ForeColor = TJAPlayer3.Skin.SongSelect_ForeColor_Children;
                                        c曲リストノード.BackColor = TJAPlayer3.Skin.SongSelect_BackColor_Children;
                                        break;
                                    case EジャンルAC15SortOrder.バラエティ:
                                        c曲リストノード.ForeColor = TJAPlayer3.Skin.SongSelect_ForeColor_Variety;
                                        c曲リストノード.BackColor = TJAPlayer3.Skin.SongSelect_BackColor_Variety;
                                        break;
                                    case EジャンルAC15SortOrder.クラシック:
                                        c曲リストノード.ForeColor = TJAPlayer3.Skin.SongSelect_ForeColor_Classic;
                                        c曲リストノード.BackColor = TJAPlayer3.Skin.SongSelect_BackColor_Classic;
                                        break;
                                    case EジャンルAC15SortOrder.ゲームミュージック:
                                        c曲リストノード.ForeColor = TJAPlayer3.Skin.SongSelect_ForeColor_GameMusic;
                                        c曲リストノード.BackColor = TJAPlayer3.Skin.SongSelect_BackColor_GameMusic;
                                        break;
                                    case EジャンルAC15SortOrder.ナムコオリジナル:
                                        c曲リストノード.ForeColor = TJAPlayer3.Skin.SongSelect_ForeColor_Namco;
                                        c曲リストノード.BackColor = TJAPlayer3.Skin.SongSelect_BackColor_Namco;
                                        break;
                                    default:
                                        break;
                                }


                                c曲リストノード.nLevel = dtx.LEVELtaiko;

                                c曲リストノード.arスコア[ n ] = new Cスコア();
                                c曲リストノード.arスコア[ n ].ファイル情報.ファイルの絶対パス = fileinfo.FullName;
                                c曲リストノード.arスコア[ n ].ファイル情報.フォルダの絶対パス = fileinfo.DirectoryName + Path.DirectorySeparatorChar;
                                c曲リストノード.arスコア[ n ].ファイル情報.ファイルサイズ = fileinfo.Length;
                                c曲リストノード.arスコア[ n ].ファイル情報.最終更新日時 = fileinfo.LastWriteTime;
                                string strFileNameScoreIni = c曲リストノード.arスコア[ n ].ファイル情報.ファイルの絶対パス + ".score.ini";
                                if( File.Exists( strFileNameScoreIni ) )
                                {
                                    FileInfo infoScoreIni = new FileInfo( strFileNameScoreIni );
                                    c曲リストノード.arスコア[ n ].ScoreIni情報.ファイルサイズ = infoScoreIni.Length;
                                    c曲リストノード.arスコア[ n ].ScoreIni情報.最終更新日時 = infoScoreIni.LastWriteTime;
                                }
                                if( b == false )
                                {
                                    this.n検索されたスコア数++;
                                    listノードリスト.Add( c曲リストノード );
                                    this.n検索された曲ノード数++;
                                    b = true;
                                }

                                if( TJAPlayer3.ConfigIni.bLog曲検索ログ出力 )
                                {
                                //    Trace.Indent();
                                //    try
                                //    {
                                //        StringBuilder sb = new StringBuilder( 0x100 );
                                //        sb.Append( string.Format( "nID#{0:D3}", c曲リストノード.nID ) );
                                //        if( c曲リストノード.r親ノード != null )
                                //        {
                                //            sb.Append( string.Format( "(in#{0:D3}):", c曲リストノード.r親ノード.nID ) );
                                //        }
                                //        else
                                //        {
                                //            sb.Append( "(onRoot):" );
                                //        }
                                //        sb.Append( " SONG, File=" + c曲リストノード.arスコア[ 0 ].ファイル情報.ファイルの絶対パス );
                                //        sb.Append( ", Size=" + c曲リストノード.arスコア[ 0 ].ファイル情報.ファイルサイズ );
                                //        sb.Append( ", LastUpdate=" + c曲リストノード.arスコア[ 0 ].ファイル情報.最終更新日時 );
                                //        Trace.TraceInformation( sb.ToString() );
                                //    }
                                //    finally
                                //    {
                                //        Trace.Unindent();
                                //    }
                                }
                            }
                        }
                        #endregion
                    }
				}
			}
			//-----------------------------
			#endregion

			foreach( DirectoryInfo infoDir in info.GetDirectories() )
			{
				SlowOrSuspendSearchTask();		// #27060 中断要求があったら、解除要求が来るまで待機, #PREMOVIE再生中は検索負荷を落とす

				#region [ a. "dtxfiles." で始まるフォルダの場合 ]
				//-----------------------------
				if( infoDir.Name.ToLower().StartsWith( "dtxfiles." ) )
				{
					C曲リストノード c曲リストノード = new C曲リストノード();
					c曲リストノード.eノード種別 = C曲リストノード.Eノード種別.BOX;
					c曲リストノード.bDTXFilesで始まるフォルダ名のBOXである = true;
					c曲リストノード.strタイトル = infoDir.Name.Substring( 9 );
					c曲リストノード.nスコア数 = 1;
					c曲リストノード.r親ノード = node親;


					// 一旦、上位BOXのスキン情報をコピー (後でbox.defの記載にて上書きされる場合がある)
					c曲リストノード.strSkinPath = ( c曲リストノード.r親ノード == null ) ?
						"" : c曲リストノード.r親ノード.strSkinPath;

					c曲リストノード.strBreadcrumbs = ( c曲リストノード.r親ノード == null ) ?
						c曲リストノード.strタイトル : c曲リストノード.r親ノード.strBreadcrumbs + " > " + c曲リストノード.strタイトル;

		
					c曲リストノード.list子リスト = new List<C曲リストノード>();
					c曲リストノード.arスコア[ 0 ] = new Cスコア();
					c曲リストノード.arスコア[ 0 ].ファイル情報.フォルダの絶対パス = infoDir.FullName + @"\";
					c曲リストノード.arスコア[ 0 ].譜面情報.タイトル = c曲リストノード.strタイトル;
					listノードリスト.Add(c曲リストノード);

                    var dtxfilesBoxdef = CBoxDef.Get(infoDir);
					if( dtxfilesBoxdef != null )
					{
						if( dtxfilesBoxdef.Title != null )
						{
							c曲リストノード.strタイトル = dtxfilesBoxdef.Title;
						}
						if( dtxfilesBoxdef.Genre != null )
						{
							c曲リストノード.strジャンル = dtxfilesBoxdef.Genre;
						}
						if( dtxfilesBoxdef.ForeColor != null )
						{
							c曲リストノード.ForeColor = dtxfilesBoxdef.ForeColor.Value;
						}
                        if (dtxfilesBoxdef.BackColor != null)
                        {
                            c曲リストノード.BackColor = dtxfilesBoxdef.BackColor.Value;
                        }
                    }
					if( TJAPlayer3.ConfigIni.bLog曲検索ログ出力 )
					{
						Trace.Indent();
						try
						{
							StringBuilder sb = new StringBuilder( 0x100 );
							sb.Append( string.Format( "nID#{0:D3}", c曲リストノード.nID ) );
							if( c曲リストノード.r親ノード != null )
							{
								sb.Append( string.Format( "(in#{0:D3}):", c曲リストノード.r親ノード.nID ) );
							}
							else
							{
								sb.Append( "(onRoot):" );
							}
							sb.Append( " BOX, Title=" + c曲リストノード.strタイトル );
							sb.Append( ", Folder=" + c曲リストノード.arスコア[ 0 ].ファイル情報.フォルダの絶対パス );
							sb.Append( ", SkinPath=" + c曲リストノード.strSkinPath );
							Trace.TraceInformation( sb.ToString() );
						}
						finally
						{
							Trace.Unindent();
						}
					}
					if( b子BOXへ再帰する )
					{
						this.t曲を検索してリストを作成する( infoDir.FullName + @"\", b子BOXへ再帰する, c曲リストノード.list子リスト, c曲リストノード );
					}

                    break;
                }
				//-----------------------------
				#endregion

				#region [ b.box.def を含むフォルダの場合  ]
				//-----------------------------
                var boxdef = CBoxDef.Get(infoDir);
				if( boxdef != null )
				{
					C曲リストノード c曲リストノード = new C曲リストノード();
					c曲リストノード.eノード種別 = C曲リストノード.Eノード種別.BOX;
					c曲リストノード.bDTXFilesで始まるフォルダ名のBOXである = false;
					c曲リストノード.strタイトル = boxdef.Title ?? "";
					c曲リストノード.strジャンル = boxdef.Genre ?? "";

                    if (boxdef.ForeColor != null)
                    {
                        c曲リストノード.ForeColor = boxdef.ForeColor.Value;
                    }
                    if (boxdef.BackColor != null)
                    {
                        c曲リストノード.BackColor = boxdef.BackColor.Value;
                    }

                    switch (CStrジャンルtoNum.ForAC15SortOrder(c曲リストノード.strジャンル))
                    {
                        case EジャンルAC15SortOrder.JPOP:
                            c曲リストノード.ForeColor = TJAPlayer3.Skin.SongSelect_ForeColor_JPOP;
                            c曲リストノード.BackColor = TJAPlayer3.Skin.SongSelect_BackColor_JPOP;
                            break;
                        case EジャンルAC15SortOrder.アニメ:
                            c曲リストノード.ForeColor = TJAPlayer3.Skin.SongSelect_ForeColor_Anime;
                            c曲リストノード.BackColor = TJAPlayer3.Skin.SongSelect_BackColor_Anime;
                            break;
                        case EジャンルAC15SortOrder.ボーカロイド:
                            c曲リストノード.ForeColor = TJAPlayer3.Skin.SongSelect_ForeColor_VOCALOID;
                            c曲リストノード.BackColor = TJAPlayer3.Skin.SongSelect_BackColor_VOCALOID;
                            break;
                        case EジャンルAC15SortOrder.どうよう:
                            c曲リストノード.ForeColor = TJAPlayer3.Skin.SongSelect_ForeColor_Children;
                            c曲リストノード.BackColor = TJAPlayer3.Skin.SongSelect_BackColor_Children;
                            break;
                        case EジャンルAC15SortOrder.バラエティ:
                            c曲リストノード.ForeColor = TJAPlayer3.Skin.SongSelect_ForeColor_Variety;
                            c曲リストノード.BackColor = TJAPlayer3.Skin.SongSelect_BackColor_Variety;
                            break;
                        case EジャンルAC15SortOrder.クラシック:
                            c曲リストノード.ForeColor = TJAPlayer3.Skin.SongSelect_ForeColor_Classic;
                            c曲リストノード.BackColor = TJAPlayer3.Skin.SongSelect_BackColor_Classic;
                            break;
                        case EジャンルAC15SortOrder.ゲームミュージック:
                            c曲リストノード.ForeColor = TJAPlayer3.Skin.SongSelect_ForeColor_GameMusic;
                            c曲リストノード.BackColor = TJAPlayer3.Skin.SongSelect_BackColor_GameMusic;
                            break;
                        case EジャンルAC15SortOrder.ナムコオリジナル:
                            c曲リストノード.ForeColor = TJAPlayer3.Skin.SongSelect_ForeColor_Namco;
                            c曲リストノード.BackColor = TJAPlayer3.Skin.SongSelect_BackColor_Namco;
                            break;
                        default:
                            break;
                    }



                    c曲リストノード.nスコア数 = 1;
					c曲リストノード.arスコア[ 0 ] = new Cスコア();
					c曲リストノード.arスコア[ 0 ].ファイル情報.フォルダの絶対パス = infoDir.FullName + @"\";
					c曲リストノード.arスコア[ 0 ].譜面情報.タイトル = boxdef.Title ?? "";
					c曲リストノード.r親ノード = node親;

					c曲リストノード.strBreadcrumbs = ( c曲リストノード.r親ノード == null ) ?
						c曲リストノード.strタイトル : c曲リストノード.r親ノード.strBreadcrumbs + " > " + c曲リストノード.strタイトル;
	
					
					c曲リストノード.list子リスト = new List<C曲リストノード>();
					listノードリスト.Add( c曲リストノード );
					if( TJAPlayer3.ConfigIni.bLog曲検索ログ出力 )
					{
						Trace.TraceInformation( "box.def検出 : {0}", infoDir.FullName + @"\box.def" );
						Trace.Indent();
						try
						{
							StringBuilder sb = new StringBuilder( 0x400 );
							sb.Append( string.Format( "nID#{0:D3}", c曲リストノード.nID ) );
							if( c曲リストノード.r親ノード != null )
							{
								sb.Append( string.Format( "(in#{0:D3}):", c曲リストノード.r親ノード.nID ) );
							}
							else
							{
								sb.Append( "(onRoot):" );
							}
							sb.Append( "BOX, Title=" + c曲リストノード.strタイトル );
							if( ( c曲リストノード.strジャンル != null ) && ( c曲リストノード.strジャンル.Length > 0 ) )
							{
								sb.Append( ", Genre=" + c曲リストノード.strジャンル );
							}
                            if (c曲リストノード.ForeColor != TJAPlayer3.Skin.SongSelect_ForeColor)
                            {
                                sb.Append(", ForeColor=" + c曲リストノード.ForeColor.ToString());
                            }
                            if (c曲リストノード.BackColor != TJAPlayer3.Skin.SongSelect_ForeColor)
                            {
                                sb.Append(", BackColor=" + c曲リストノード.BackColor.ToString());
                            }
                            Trace.TraceInformation( sb.ToString() );
						}
						finally
						{
							Trace.Unindent();
						}
					}
					if( b子BOXへ再帰する )
					{
						this.t曲を検索してリストを作成する( infoDir.FullName + @"\", b子BOXへ再帰する, c曲リストノード.list子リスト, c曲リストノード );
					}
				}
				//-----------------------------
				#endregion

				#region [ c.通常フォルダの場合 ]
				//-----------------------------
				else
				{
					this.t曲を検索してリストを作成する( infoDir.FullName + @"\", b子BOXへ再帰する, listノードリスト, node親 );
				}
				//-----------------------------
				#endregion
			}
		}

        private static IEnumerable<FileInfo> GetPlaylistFileInfos(IEnumerable<FileInfo> fileInfos)
        {
            return fileInfos
                .Where(o => o.Extension.ToUpperInvariant() == ".T3U8")
                .OrderBy(o => o.Name)
                .SelectMany(GetPlaylistFileInfos);

            IEnumerable<FileInfo> GetPlaylistFileInfos(FileInfo playlistFileInfo)
            {
                return File.ReadAllLines(playlistFileInfo.FullName, Encoding.UTF8)
                    .Where(o => !string.IsNullOrEmpty(o))
                    .Select(o => new FileInfo(Path.Combine(playlistFileInfo.DirectoryName, o)));
            }
        }

        //-----------------
		#endregion
		#region [ スコアキャッシュを曲リストに反映する ]
		//-----------------
		public void tスコアキャッシュを曲リストに反映する()
		{
			this.nスコアキャッシュから反映できたスコア数 = 0;
			this.tスコアキャッシュを曲リストに反映する( this.list曲ルート );
		}
		private void tスコアキャッシュを曲リストに反映する( List<C曲リストノード> ノードリスト )
		{
			using( List<C曲リストノード>.Enumerator enumerator = ノードリスト.GetEnumerator() )
			{
				while( enumerator.MoveNext() )
				{
					SlowOrSuspendSearchTask();		// #27060 中断要求があったら、解除要求が来るまで待機, #PREMOVIE再生中は検索負荷を落とす

					C曲リストノード node = enumerator.Current;
					if( node.eノード種別 == C曲リストノード.Eノード種別.BOX )
					{
						this.tスコアキャッシュを曲リストに反映する( node.list子リスト );
					}
					else if( ( node.eノード種別 == C曲リストノード.Eノード種別.SCORE ) || ( node.eノード種別 == C曲リストノード.Eノード種別.SCORE_MIDI ) )
					{
						Predicate<Cスコア> match = null;
						for( int lv = 0; lv < (int)Difficulty.Total; lv++ )
						{
							if( node.arスコア[ lv ] != null )
							{
								if( match == null )
								{
									match = delegate( Cスコア sc )
									{
										return
											(
											( sc.ファイル情報.ファイルの絶対パス.Equals( node.arスコア[ lv ].ファイル情報.ファイルの絶対パス )
											&& sc.ファイル情報.ファイルサイズ.Equals( node.arスコア[ lv ].ファイル情報.ファイルサイズ ) )
											&& ( sc.ファイル情報.最終更新日時.Equals( node.arスコア[ lv ].ファイル情報.最終更新日時 )
											&& sc.ScoreIni情報.ファイルサイズ.Equals( node.arスコア[ lv ].ScoreIni情報.ファイルサイズ ) ) )
											&& sc.ScoreIni情報.最終更新日時.Equals( node.arスコア[ lv ].ScoreIni情報.最終更新日時 );
									};
								}
								int nMatched = this.listSongsDB.FindIndex( match );
								if( nMatched == -1 )
								{
//Trace.TraceInformation( "songs.db に存在しません。({0})", node.arスコア[ lv ].ファイル情報.ファイルの絶対パス );
									if ( TJAPlayer3.ConfigIni.bLog曲検索ログ出力 )
									{
										Trace.TraceInformation( "songs.db に存在しません。({0})", node.arスコア[ lv ].ファイル情報.ファイルの絶対パス );
									}
								}
								else
								{
									node.arスコア[ lv ].譜面情報 = this.listSongsDB[ nMatched ].譜面情報;
									node.arスコア[ lv ].bSongDBにキャッシュがあった = true;
									if( TJAPlayer3.ConfigIni.bLog曲検索ログ出力 )
									{
										Trace.TraceInformation( "songs.db から転記しました。({0})", node.arスコア[ lv ].ファイル情報.ファイルの絶対パス );
									}
									this.nスコアキャッシュから反映できたスコア数++;
								}
							}
						}
					}
				}
			}
		}
		private Cスコア tSongsDBからスコアを１つ読み込む( BinaryReader br )
		{
			Cスコア cスコア = new Cスコア();
			cスコア.ファイル情報.ファイルの絶対パス = br.ReadString();
			cスコア.ファイル情報.フォルダの絶対パス = br.ReadString();
			cスコア.ファイル情報.最終更新日時 = new DateTime( br.ReadInt64() );
			cスコア.ファイル情報.ファイルサイズ = br.ReadInt64();
			cスコア.ScoreIni情報.最終更新日時 = new DateTime( br.ReadInt64() );
			cスコア.ScoreIni情報.ファイルサイズ = br.ReadInt64();
			cスコア.譜面情報.タイトル = br.ReadString();
			cスコア.譜面情報.Preimage = br.ReadString();
			cスコア.譜面情報.Premovie = br.ReadString();
			cスコア.譜面情報.Presound = br.ReadString();
			cスコア.譜面情報.Backgound = br.ReadString();
			cスコア.譜面情報.レベル.Drums = br.ReadInt32();
			cスコア.譜面情報.レベル.Guitar = br.ReadInt32();
			cスコア.譜面情報.レベル.Bass = br.ReadInt32();
			cスコア.譜面情報.最大ランク.Drums = br.ReadInt32();
			cスコア.譜面情報.最大ランク.Guitar = br.ReadInt32();
			cスコア.譜面情報.最大ランク.Bass = br.ReadInt32();
			cスコア.譜面情報.最大スキル.Drums = br.ReadDouble();
			cスコア.譜面情報.最大スキル.Guitar = br.ReadDouble();
			cスコア.譜面情報.最大スキル.Bass = br.ReadDouble();
			cスコア.譜面情報.フルコンボ.Drums = br.ReadBoolean();
			cスコア.譜面情報.フルコンボ.Guitar = br.ReadBoolean();
			cスコア.譜面情報.フルコンボ.Bass = br.ReadBoolean();
			cスコア.譜面情報.演奏回数.Drums = br.ReadInt32();
			cスコア.譜面情報.演奏回数.Guitar = br.ReadInt32();
			cスコア.譜面情報.演奏回数.Bass = br.ReadInt32();
			cスコア.譜面情報.演奏履歴.行1 = br.ReadString();
			cスコア.譜面情報.演奏履歴.行2 = br.ReadString();
			cスコア.譜面情報.演奏履歴.行3 = br.ReadString();
			cスコア.譜面情報.演奏履歴.行4 = br.ReadString();
			cスコア.譜面情報.演奏履歴.行5 = br.ReadString();
			cスコア.譜面情報.演奏履歴.行6 = br.ReadString();
			cスコア.譜面情報.演奏履歴.行7 = br.ReadString();
			cスコア.譜面情報.レベルを非表示にする = br.ReadBoolean();
			cスコア.譜面情報.曲種別 = (CDTX.E種別) br.ReadInt32();
			cスコア.譜面情報.Bpm = br.ReadDouble();
			cスコア.譜面情報.Duration = br.ReadInt32();
            cスコア.譜面情報.strBGMファイル名 = br.ReadString();
            cスコア.譜面情報.SongVol = br.ReadInt32();
		    var hasSongIntegratedLoudness = br.ReadBoolean();
		    var songIntegratedLoudness = br.ReadDouble();
		    var integratedLoudness = hasSongIntegratedLoudness ? new Lufs(songIntegratedLoudness) : default(Lufs?);
		    var hasSongPeakLoudness = br.ReadBoolean();
		    var songPeakLoudness = br.ReadDouble();
		    var peakLoudness = hasSongPeakLoudness ? new Lufs(songPeakLoudness) : default(Lufs?);
		    var songLoudnessMetadata = hasSongIntegratedLoudness
		        ? new LoudnessMetadata(integratedLoudness.Value, peakLoudness)
		        : default(LoudnessMetadata?);
		    cスコア.譜面情報.SongLoudnessMetadata = songLoudnessMetadata;
            cスコア.譜面情報.nデモBGMオフセット = br.ReadInt32();
            cスコア.譜面情報.b譜面分岐[0] = br.ReadBoolean();
            cスコア.譜面情報.b譜面分岐[1] = br.ReadBoolean();
            cスコア.譜面情報.b譜面分岐[2] = br.ReadBoolean();
            cスコア.譜面情報.b譜面分岐[3] = br.ReadBoolean();
            cスコア.譜面情報.b譜面分岐[4] = br.ReadBoolean();
            cスコア.譜面情報.b譜面分岐[5] = br.ReadBoolean();
            cスコア.譜面情報.b譜面分岐[6] = br.ReadBoolean();
            cスコア.譜面情報.ハイスコア = br.ReadInt32();
            cスコア.譜面情報.nハイスコア[0] = br.ReadInt32();
            cスコア.譜面情報.nハイスコア[1] = br.ReadInt32();
            cスコア.譜面情報.nハイスコア[2] = br.ReadInt32();
            cスコア.譜面情報.nハイスコア[3] = br.ReadInt32();
            cスコア.譜面情報.nハイスコア[4] = br.ReadInt32();
            cスコア.譜面情報.nハイスコア[5] = br.ReadInt32();
            cスコア.譜面情報.nハイスコア[6] = br.ReadInt32();
            cスコア.譜面情報.strサブタイトル = br.ReadString();
            cスコア.譜面情報.nレベル[0] = br.ReadInt32();
            cスコア.譜面情報.nレベル[1] = br.ReadInt32();
            cスコア.譜面情報.nレベル[2] = br.ReadInt32();
            cスコア.譜面情報.nレベル[3] = br.ReadInt32();
            cスコア.譜面情報.nレベル[4] = br.ReadInt32();
            cスコア.譜面情報.nレベル[5] = br.ReadInt32();
            cスコア.譜面情報.nレベル[6] = br.ReadInt32();
		    var hasSongRating = br.ReadBoolean();
		    var songRating = br.ReadInt32();
		    cスコア.譜面情報.Rating = hasSongRating
		        ? (SongRating) songRating
		        : SongRating.Unset;

            //Debug.WriteLine( "songs.db: " + cスコア.ファイル情報.ファイルの絶対パス );
            return cスコア;
		}
		//-----------------
		#endregion
		#region [ SongsDBになかった曲をファイルから読み込んで反映する ]
		//-----------------
		public void tSongsDBになかった曲をファイルから読み込んで反映する()
		{
			this.nファイルから反映できたスコア数 = 0;
			this.tSongsDBになかった曲をファイルから読み込んで反映する( this.list曲ルート );
		}
		private void tSongsDBになかった曲をファイルから読み込んで反映する( List<C曲リストノード> ノードリスト )
		{
			foreach( C曲リストノード c曲リストノード in ノードリスト )
			{
				SlowOrSuspendSearchTask();		// #27060 中断要求があったら、解除要求が来るまで待機, #PREMOVIE再生中は検索負荷を落とす

				if( c曲リストノード.eノード種別 == C曲リストノード.Eノード種別.BOX )
				{
					this.tSongsDBになかった曲をファイルから読み込んで反映する( c曲リストノード.list子リスト );
				}
				else if( ( c曲リストノード.eノード種別 == C曲リストノード.Eノード種別.SCORE )
					  || ( c曲リストノード.eノード種別 == C曲リストノード.Eノード種別.SCORE_MIDI ) )
				{
					for( int i = 0; i < (int)Difficulty.Total; i++ )
					{
						if( ( c曲リストノード.arスコア[ i ] != null ) && !c曲リストノード.arスコア[ i ].bSongDBにキャッシュがあった )
						{
							#region [ DTX ファイルのヘッダだけ読み込み、Cスコア.譜面情報 を設定する ]
							//-----------------
							string path = c曲リストノード.arスコア[ i ].ファイル情報.ファイルの絶対パス;
							if( File.Exists( path ) )
							{
								try
								{
									CDTX cdtx = new CDTX( c曲リストノード.arスコア[ i ].ファイル情報.ファイルの絶対パス, true, 0, 0, 0 );
                                    if( File.Exists( c曲リストノード.arスコア[ i ].ファイル情報.フォルダの絶対パス + "set.def" ) )
									    cdtx = new CDTX( c曲リストノード.arスコア[ i ].ファイル情報.ファイルの絶対パス, true, 0, 0, 1 );

									c曲リストノード.arスコア[ i ].譜面情報.タイトル = cdtx.TITLE;
                                    
									
                                    c曲リストノード.arスコア[ i ].譜面情報.Preimage = cdtx.PREIMAGE;
									c曲リストノード.arスコア[ i ].譜面情報.Presound = cdtx.PREVIEW;
									c曲リストノード.arスコア[ i ].譜面情報.Backgound = ( ( cdtx.BACKGROUND != null ) && ( cdtx.BACKGROUND.Length > 0 ) ) ? cdtx.BACKGROUND : cdtx.BACKGROUND_GR;
									c曲リストノード.arスコア[ i ].譜面情報.レベル.Drums = cdtx.LEVEL.Drums;
									c曲リストノード.arスコア[ i ].譜面情報.レベル.Guitar = cdtx.LEVEL.Guitar;
									c曲リストノード.arスコア[ i ].譜面情報.レベル.Bass = cdtx.LEVEL.Bass;
									c曲リストノード.arスコア[ i ].譜面情報.レベルを非表示にする = cdtx.HIDDENLEVEL;
									c曲リストノード.arスコア[ i ].譜面情報.Bpm = cdtx.BPM;
									c曲リストノード.arスコア[ i ].譜面情報.Duration = 0;	//  (cdtx.listChip == null)? 0 : cdtx.listChip[ cdtx.listChip.Count - 1 ].n発声時刻ms;
                                    c曲リストノード.arスコア[ i ].譜面情報.strBGMファイル名 = cdtx.strBGM_PATH;
                                    c曲リストノード.arスコア[ i ].譜面情報.SongVol = cdtx.SongVol;
                                    c曲リストノード.arスコア[ i ].譜面情報.SongLoudnessMetadata = cdtx.SongLoudnessMetadata;
								    c曲リストノード.arスコア[ i ].譜面情報.nデモBGMオフセット = cdtx.nデモBGMオフセット;
                                    c曲リストノード.arスコア[ i ].譜面情報.b譜面分岐[0] = cdtx.bHIDDENBRANCH ? false : cdtx.bHasBranch[ 0 ];
                                    c曲リストノード.arスコア[ i ].譜面情報.b譜面分岐[1] = cdtx.bHIDDENBRANCH ? false : cdtx.bHasBranch[ 1 ];
                                    c曲リストノード.arスコア[ i ].譜面情報.b譜面分岐[2] = cdtx.bHIDDENBRANCH ? false : cdtx.bHasBranch[ 2 ];
                                    c曲リストノード.arスコア[ i ].譜面情報.b譜面分岐[3] = cdtx.bHIDDENBRANCH ? false : cdtx.bHasBranch[ 3 ];
                                    c曲リストノード.arスコア[i].譜面情報.b譜面分岐[4] = cdtx.bHIDDENBRANCH ? false : cdtx.bHasBranch[4];
                                    c曲リストノード.arスコア[i].譜面情報.b譜面分岐[5] = cdtx.bHIDDENBRANCH ? false : cdtx.bHasBranch[5];
                                    c曲リストノード.arスコア[i].譜面情報.b譜面分岐[6] = cdtx.bHIDDENBRANCH ? false : cdtx.bHasBranch[6];
                                    c曲リストノード.arスコア[ i ].譜面情報.strサブタイトル = cdtx.SUBTITLE;
                                    c曲リストノード.arスコア[ i ].譜面情報.nレベル[0] = cdtx.LEVELtaiko[0];
                                    c曲リストノード.arスコア[ i ].譜面情報.nレベル[1] = cdtx.LEVELtaiko[1];
                                    c曲リストノード.arスコア[ i ].譜面情報.nレベル[2] = cdtx.LEVELtaiko[2];
                                    c曲リストノード.arスコア[ i ].譜面情報.nレベル[3] = cdtx.LEVELtaiko[3];
                                    c曲リストノード.arスコア[ i ].譜面情報.nレベル[4] = cdtx.LEVELtaiko[4];
                                    c曲リストノード.arスコア[i].譜面情報.nレベル[5] = cdtx.LEVELtaiko[5];
                                    c曲リストノード.arスコア[i].譜面情報.nレベル[6] = cdtx.LEVELtaiko[6];
								    c曲リストノード.arスコア[i].譜面情報.Rating = SongRatingController.GetRating(path);
                                    this.nファイルから反映できたスコア数++;
									cdtx.On非活性化();
//Debug.WriteLine( "★" + this.nファイルから反映できたスコア数 + " " + c曲リストノード.arスコア[ i ].譜面情報.タイトル );
									#region [ 曲検索ログ出力 ]
									//-----------------
									if( TJAPlayer3.ConfigIni.bLog曲検索ログ出力 )
									{
										StringBuilder sb = new StringBuilder( 0x400 );
										sb.Append( string.Format( "曲データファイルから譜面情報を転記しました。({0})", path ) );
										sb.Append( "(title=" + c曲リストノード.arスコア[ i ].譜面情報.タイトル );
										sb.Append( ", preimage=" + c曲リストノード.arスコア[ i ].譜面情報.Preimage );
										sb.Append( ", premovie=" + c曲リストノード.arスコア[ i ].譜面情報.Premovie );
										sb.Append( ", presound=" + c曲リストノード.arスコア[ i ].譜面情報.Presound );
										sb.Append( ", background=" + c曲リストノード.arスコア[ i ].譜面情報.Backgound );
										sb.Append( ", lvDr=" + c曲リストノード.arスコア[ i ].譜面情報.レベル.Drums );
										sb.Append( ", lvGt=" + c曲リストノード.arスコア[ i ].譜面情報.レベル.Guitar );
										sb.Append( ", lvBs=" + c曲リストノード.arスコア[ i ].譜面情報.レベル.Bass );
										sb.Append( ", lvHide=" + c曲リストノード.arスコア[ i ].譜面情報.レベルを非表示にする );
										sb.Append( ", type=" + c曲リストノード.arスコア[ i ].譜面情報.曲種別 );
										sb.Append( ", bpm=" + c曲リストノード.arスコア[ i ].譜面情報.Bpm );
									//	sb.Append( ", duration=" + c曲リストノード.arスコア[ i ].譜面情報.Duration );
										Trace.TraceInformation( sb.ToString() );
									}
									//-----------------
									#endregion
								}
								catch( Exception exception )
								{
									Trace.TraceError( exception.ToString() );
									c曲リストノード.arスコア[ i ] = null;
									c曲リストノード.nスコア数--;
									this.n検索されたスコア数--;
									Trace.TraceError( "曲データファイルの読み込みに失敗しました。({0})", path );
								}
							}
							//-----------------
							#endregion

							#region [ 対応する .score.ini が存在していれば読み込み、Cスコア.譜面情報 に追加設定する ]
							//-----------------
                            try
                            {
                                var scoreIniPath = c曲リストノード.arスコア[ i ].ファイル情報.ファイルの絶対パス + ".score.ini";
                                if( File.Exists( scoreIniPath ) )
                                    this.tScoreIniを読み込んで譜面情報を設定する( scoreIniPath, c曲リストノード.arスコア[ i ] );
                                else
                                {
                                    string[] dtxscoreini = Directory.GetFiles(c曲リストノード.arスコア[i].ファイル情報.フォルダの絶対パス, "*.dtx.score.ini");
                                    if (dtxscoreini.Length != 0 && File.Exists(dtxscoreini[0]))
                                    {
                                        this.tScoreIniを読み込んで譜面情報を設定する(dtxscoreini[0], c曲リストノード.arスコア[i]);
                                    }
                                }
                            }
                            catch (Exception e)
                            {
                                Trace.TraceError( e.ToString() );
                                Trace.TraceError( "例外が発生しましたが処理を継続します。 (c8b6538c-46a1-403e-8cc3-fc7e7ff914fb)" );
                            }

							//-----------------
							#endregion
						}
					}
				}
			}
		}
		//-----------------
		#endregion
		#region [ 曲リストへ後処理を適用する ]
		//-----------------
		public void t曲リストへ後処理を適用する()
		{
			listStrBoxDefSkinSubfolderFullName = new List<string>();
			if ( TJAPlayer3.Skin.strBoxDefSkinSubfolders != null )
			{
				foreach ( string b in TJAPlayer3.Skin.strBoxDefSkinSubfolders )
				{
					listStrBoxDefSkinSubfolderFullName.Add( b );
				}
			}

			this.t曲リストへ後処理を適用する( this.list曲ルート );

			#region [ skin名で比較して、systemスキンとboxdefスキンに重複があれば、boxdefスキン側を削除する ]
			string[] systemSkinNames = CSkin.GetSkinName( TJAPlayer3.Skin.strSystemSkinSubfolders );
			List<string> l = new List<string>( listStrBoxDefSkinSubfolderFullName );
			foreach ( string boxdefSkinSubfolderFullName in l )
			{
				if ( Array.BinarySearch( systemSkinNames,
					CSkin.GetSkinName( boxdefSkinSubfolderFullName ),
					StringComparer.InvariantCultureIgnoreCase ) >= 0 )
				{
					listStrBoxDefSkinSubfolderFullName.Remove( boxdefSkinSubfolderFullName );
				}
			}
			#endregion
			string[] ba = listStrBoxDefSkinSubfolderFullName.ToArray();
			Array.Sort( ba );
			TJAPlayer3.Skin.strBoxDefSkinSubfolders = ba;
		}
		private void t曲リストへ後処理を適用する( List<C曲リストノード> ノードリスト )
		{
			#region [ リストに１つ以上の曲があるなら RANDOM BOX を入れる ]
			//-----------------------------
			if( ノードリスト.Count > 0 )
			{
				C曲リストノード itemRandom = new C曲リストノード();
				itemRandom.eノード種別 = C曲リストノード.Eノード種別.RANDOM;
				itemRandom.strタイトル = "ランダムに曲をえらぶ";
				itemRandom.nスコア数 = (int)Difficulty.Total;
				itemRandom.r親ノード = ノードリスト[ 0 ].r親ノード;
                
                itemRandom.strBreadcrumbs = ( itemRandom.r親ノード == null ) ?
					itemRandom.strタイトル :  itemRandom.r親ノード.strBreadcrumbs + " > " + itemRandom.strタイトル;

				for( int i = 0; i < (int)Difficulty.Total; i++ )
				{
					itemRandom.arスコア[ i ] = new Cスコア();
					itemRandom.arスコア[ i ].譜面情報.タイトル = string.Format( "< RANDOM SELECT Lv.{0} >", i + 1 );
					itemRandom.ar難易度ラベル[ i ] = string.Format( "L{0}", i + 1 );
				}
				ノードリスト.Add( itemRandom );

				#region [ ログ出力 ]
				//-----------------------------
				if( TJAPlayer3.ConfigIni.bLog曲検索ログ出力 )
				{
					StringBuilder sb = new StringBuilder( 0x100 );
					sb.Append( string.Format( "nID#{0:D3}", itemRandom.nID ) );
					if( itemRandom.r親ノード != null )
					{
						sb.Append( string.Format( "(in#{0:D3}):", itemRandom.r親ノード.nID ) );
					}
					else
					{
						sb.Append( "(onRoot):" );
					}
					sb.Append( " RANDOM" );
					Trace.TraceInformation( sb.ToString() );
				}
				//-----------------------------
				#endregion
			}
			//-----------------------------
			#endregion

			// すべてのノードについて…
			foreach( C曲リストノード c曲リストノード in ノードリスト )
			{
				SlowOrSuspendSearchTask();		// #27060 中断要求があったら、解除要求が来るまで待機, #PREMOVIE再生中は検索負荷を落とす

				#region [ BOXノードなら子リストに <<BACK を入れ、子リストに後処理を適用する ]
				//-----------------------------
				if( c曲リストノード.eノード種別 == C曲リストノード.Eノード種別.BOX )
				{
					C曲リストノード itemBack = new C曲リストノード();
					itemBack.eノード種別 = C曲リストノード.Eノード種別.BACKBOX;
					itemBack.strタイトル = "とじる";
					itemBack.nスコア数 = 1;
					itemBack.r親ノード = c曲リストノード;

					itemBack.strSkinPath = ( c曲リストノード.r親ノード == null ) ?
						"" : c曲リストノード.r親ノード.strSkinPath;

					if ( itemBack.strSkinPath != "" && !listStrBoxDefSkinSubfolderFullName.Contains( itemBack.strSkinPath ) )
					{
						listStrBoxDefSkinSubfolderFullName.Add( itemBack.strSkinPath );
					}

					itemBack.strBreadcrumbs = ( itemBack.r親ノード == null ) ?
						itemBack.strタイトル : itemBack.r親ノード.strBreadcrumbs + " > " + itemBack.strタイトル;

					itemBack.arスコア[ 0 ] = new Cスコア();
					itemBack.arスコア[ 0 ].ファイル情報.フォルダの絶対パス = "";
					itemBack.arスコア[ 0 ].譜面情報.タイトル = itemBack.strタイトル;
					c曲リストノード.list子リスト.Insert( 0, itemBack );

					#region [ ログ出力 ]
					//-----------------------------
					if( TJAPlayer3.ConfigIni.bLog曲検索ログ出力 )
					{
						StringBuilder sb = new StringBuilder( 0x100 );
						sb.Append( string.Format( "nID#{0:D3}", itemBack.nID ) );
						if( itemBack.r親ノード != null )
						{
							sb.Append( string.Format( "(in#{0:D3}):", itemBack.r親ノード.nID ) );
						}
						else
						{
							sb.Append( "(onRoot):" );
						}
						sb.Append( " BACKBOX" );
						Trace.TraceInformation( sb.ToString() );
					}
					//-----------------------------
					#endregion

					this.t曲リストへ後処理を適用する( c曲リストノード.list子リスト );
					continue;
				}
				//-----------------------------
				#endregion

				#region [ ノードにタイトルがないなら、最初に見つけたスコアのタイトルを設定する ]
				//-----------------------------
				if( string.IsNullOrEmpty( c曲リストノード.strタイトル ) )
				{
					for( int j = 0; j < (int)Difficulty.Total; j++ )
					{
						if( ( c曲リストノード.arスコア[ j ] != null ) && !string.IsNullOrEmpty( c曲リストノード.arスコア[ j ].譜面情報.タイトル ) )
						{
							c曲リストノード.strタイトル = c曲リストノード.arスコア[ j ].譜面情報.タイトル;

							if( TJAPlayer3.ConfigIni.bLog曲検索ログ出力 )
								Trace.TraceInformation( "タイトルを設定しました。(nID#{0:D3}, title={1})", c曲リストノード.nID, c曲リストノード.strタイトル );

							break;
						}
					}
				}
				//-----------------------------
				#endregion
			}

			#region [ ノードをソートする ]
			//-----------------------------
            if( TJAPlayer3.ConfigIni.nDefaultSongSort == 0 )
            {
			    t曲リストのソート1_絶対パス順( ノードリスト );
            }
            else if( TJAPlayer3.ConfigIni.nDefaultSongSort == 1 )
            {
                t曲リストのソート9_ジャンル順( ノードリスト, E楽器パート.TAIKO, 1, 0 );
            }
            else if( TJAPlayer3.ConfigIni.nDefaultSongSort == 2 )
            {
                t曲リストのソート9_ジャンル順( ノードリスト, E楽器パート.TAIKO, 2, 0 );
            }
			//-----------------------------
			#endregion
		}
		//-----------------
		#endregion
		#region [ スコアキャッシュをSongsDBに出力する ]
		//-----------------
		public void tスコアキャッシュをSongsDBに出力する( string SongsDBファイル名 )
		{
			this.nSongsDBへ出力できたスコア数 = 0;
			try
			{
				BinaryWriter bw = new BinaryWriter( new FileStream( SongsDBファイル名, FileMode.Create, FileAccess.Write ) );
				bw.Write( SONGSDB_VERSION );
				this.tSongsDBにリストを１つ出力する( bw, this.list曲ルート );
				bw.Close();
			}
			catch (Exception e)
			{
				Trace.TraceError( "songs.dbの出力に失敗しました。" );
				Trace.TraceError( e.ToString() );
				Trace.TraceError( "例外が発生しましたが処理を継続します。 (ca70d133-f092-4351-8ebd-0906d8f1cffa)" );
			}
		}
		private void tSongsDBにノードを１つ出力する( BinaryWriter bw, C曲リストノード node )
		{
			for( int i = 0; i < (int)Difficulty.Total; i++ )
			{
				// ここではsuspendに応じないようにしておく(深い意味はない。ファイルの書き込みオープン状態を長時間維持したくないだけ)
				//if ( this.bIsSuspending )		// #27060 中断要求があったら、解除要求が来るまで待機
				//{
				//	autoReset.WaitOne();
				//}

				if( node.arスコア[ i ] != null )
				{
					bw.Write( node.arスコア[ i ].ファイル情報.ファイルの絶対パス );
					bw.Write( node.arスコア[ i ].ファイル情報.フォルダの絶対パス );
					bw.Write( node.arスコア[ i ].ファイル情報.最終更新日時.Ticks );
					bw.Write( node.arスコア[ i ].ファイル情報.ファイルサイズ );
					bw.Write( node.arスコア[ i ].ScoreIni情報.最終更新日時.Ticks );
					bw.Write( node.arスコア[ i ].ScoreIni情報.ファイルサイズ );
					bw.Write( node.arスコア[ i ].譜面情報.タイトル );
					bw.Write( node.arスコア[ i ].譜面情報.Preimage );
					bw.Write( node.arスコア[ i ].譜面情報.Premovie );
					bw.Write( node.arスコア[ i ].譜面情報.Presound );
					bw.Write( node.arスコア[ i ].譜面情報.Backgound );
					bw.Write( node.arスコア[ i ].譜面情報.レベル.Drums );
					bw.Write( node.arスコア[ i ].譜面情報.レベル.Guitar );
					bw.Write( node.arスコア[ i ].譜面情報.レベル.Bass );
					bw.Write( node.arスコア[ i ].譜面情報.最大ランク.Drums );
					bw.Write( node.arスコア[ i ].譜面情報.最大ランク.Guitar );
					bw.Write( node.arスコア[ i ].譜面情報.最大ランク.Bass );
					bw.Write( node.arスコア[ i ].譜面情報.最大スキル.Drums );
					bw.Write( node.arスコア[ i ].譜面情報.最大スキル.Guitar );
					bw.Write( node.arスコア[ i ].譜面情報.最大スキル.Bass );
					bw.Write( node.arスコア[ i ].譜面情報.フルコンボ.Drums );
					bw.Write( node.arスコア[ i ].譜面情報.フルコンボ.Guitar );
					bw.Write( node.arスコア[ i ].譜面情報.フルコンボ.Bass );
					bw.Write( node.arスコア[ i ].譜面情報.演奏回数.Drums );
					bw.Write( node.arスコア[ i ].譜面情報.演奏回数.Guitar );
					bw.Write( node.arスコア[ i ].譜面情報.演奏回数.Bass );
					bw.Write( node.arスコア[ i ].譜面情報.演奏履歴.行1 );
					bw.Write( node.arスコア[ i ].譜面情報.演奏履歴.行2 );
					bw.Write( node.arスコア[ i ].譜面情報.演奏履歴.行3 );
					bw.Write( node.arスコア[ i ].譜面情報.演奏履歴.行4 );
					bw.Write( node.arスコア[ i ].譜面情報.演奏履歴.行5 );
                    bw.Write(node.arスコア[i].譜面情報.演奏履歴.行6);
                    bw.Write(node.arスコア[i].譜面情報.演奏履歴.行7);
                    bw.Write( node.arスコア[ i ].譜面情報.レベルを非表示にする );
					bw.Write( (int) node.arスコア[ i ].譜面情報.曲種別 );
					bw.Write( node.arスコア[ i ].譜面情報.Bpm );
					bw.Write( node.arスコア[ i ].譜面情報.Duration );
                    bw.Write( node.arスコア[ i ].譜面情報.strBGMファイル名 );
                    bw.Write( node.arスコア[ i ].譜面情報.SongVol );
				    var songLoudnessMetadata = node.arスコア[ i ].譜面情報.SongLoudnessMetadata;
				    bw.Write( songLoudnessMetadata.HasValue );
                    bw.Write( songLoudnessMetadata?.Integrated.ToDouble() ?? 0.0 );
                    bw.Write( songLoudnessMetadata?.TruePeak.HasValue ?? false );
                    bw.Write( songLoudnessMetadata?.TruePeak?.ToDouble() ?? 0.0 );
				    bw.Write( node.arスコア[ i ].譜面情報.nデモBGMオフセット );
                    bw.Write( node.arスコア[ i ].譜面情報.b譜面分岐[0] );
                    bw.Write( node.arスコア[ i ].譜面情報.b譜面分岐[1] );
                    bw.Write( node.arスコア[ i ].譜面情報.b譜面分岐[2] );
                    bw.Write( node.arスコア[ i ].譜面情報.b譜面分岐[3] );
                    bw.Write( node.arスコア[ i ].譜面情報.b譜面分岐[4] );
                    bw.Write(node.arスコア[i].譜面情報.b譜面分岐[5]);
                    bw.Write( node.arスコア[ i ].譜面情報.b譜面分岐[6] );
                    bw.Write( node.arスコア[ i ].譜面情報.ハイスコア );
                    bw.Write( node.arスコア[ i ].譜面情報.nハイスコア[0] );
                    bw.Write( node.arスコア[ i ].譜面情報.nハイスコア[1] );
                    bw.Write( node.arスコア[ i ].譜面情報.nハイスコア[2] );
                    bw.Write( node.arスコア[ i ].譜面情報.nハイスコア[3] );
                    bw.Write( node.arスコア[ i ].譜面情報.nハイスコア[4] );
                    bw.Write(node.arスコア[i].譜面情報.nハイスコア[5]);
                    bw.Write(node.arスコア[i].譜面情報.nハイスコア[6]);
                    bw.Write( node.arスコア[ i ].譜面情報.strサブタイトル );
                    bw.Write( node.arスコア[ i ].譜面情報.nレベル[0] );
                    bw.Write( node.arスコア[ i ].譜面情報.nレベル[1] );
                    bw.Write( node.arスコア[ i ].譜面情報.nレベル[2] );
                    bw.Write( node.arスコア[ i ].譜面情報.nレベル[3] );
                    bw.Write( node.arスコア[ i ].譜面情報.nレベル[4] );
                    bw.Write(node.arスコア[i].譜面情報.nレベル[5]);
                    bw.Write(node.arスコア[i].譜面情報.nレベル[6]);
				    var songRating = node.arスコア[i].譜面情報.Rating;
                    bw.Write(songRating.HasValue);
                    bw.Write(songRating.HasValue ? (int) songRating : 0);
                    this.nSongsDBへ出力できたスコア数++;
				}
			}
		}
		private void tSongsDBにリストを１つ出力する( BinaryWriter bw, List<C曲リストノード> list )
		{
			foreach( C曲リストノード c曲リストノード in list )
			{
				if(    ( c曲リストノード.eノード種別 == C曲リストノード.Eノード種別.SCORE )
					|| ( c曲リストノード.eノード種別 == C曲リストノード.Eノード種別.SCORE_MIDI ) )
				{
					this.tSongsDBにノードを１つ出力する( bw, c曲リストノード );
				}
				if( c曲リストノード.list子リスト != null )
				{
					this.tSongsDBにリストを１つ出力する( bw, c曲リストノード.list子リスト );
				}
			}
		}
		//-----------------
		#endregion
		
		#region [ 曲リストソート ]
		//-----------------

	    public static void t曲リストのソート1_絶対パス順( List<C曲リストノード> ノードリスト )
	    {
	        t曲リストのソート1_絶対パス順(ノードリスト, E楽器パート.TAIKO, 1, 0);

	        foreach( C曲リストノード c曲リストノード in ノードリスト )
	        {
	            if( ( c曲リストノード.list子リスト != null ) && ( c曲リストノード.list子リスト.Count > 1 ) )
	            {
	                t曲リストのソート1_絶対パス順( c曲リストノード.list子リスト );
	            }
	        }
	    }

	    public static void t曲リストのソート1_絶対パス順( List<C曲リストノード> ノードリスト, E楽器パート part, int order, params object[] p )
	    {
            var comparer = new ComparerChain<C曲リストノード>(
                new C曲リストノードComparerノード種別(),
                new C曲リストノードComparer絶対パス(order),
                new C曲リストノードComparerタイトル(order));

	        ノードリスト.Sort( comparer );
	    }

	    public static void t曲リストのソート2_タイトル順( List<C曲リストノード> ノードリスト, E楽器パート part, int order, params object[] p )
	    {
	        var comparer = new ComparerChain<C曲リストノード>(
	            new C曲リストノードComparerノード種別(),
	            new C曲リストノードComparerタイトル(order),
	            new C曲リストノードComparer絶対パス(order));

	        ノードリスト.Sort( comparer );
	    }

		/// <summary>
		/// 
		/// </summary>
		/// <param name="ノードリスト"></param>
		/// <param name="part"></param>
		/// <param name="order">1=Ascend -1=Descend</param>
		public static void t曲リストのソート3_演奏回数の多い順( List<C曲リストノード> ノードリスト, E楽器パート part, int order, params object[] p )
		{
			order = -order;
			int nL12345 = (int) p[ 0 ];
			if ( part != E楽器パート.UNKNOWN )
			{
				ノードリスト.Sort( delegate( C曲リストノード n1, C曲リストノード n2 )
				{
					#region [ 共通処理 ]
					if( ( n1.eノード種別 == C曲リストノード.Eノード種別.BOX ) && ( n2.eノード種別 == C曲リストノード.Eノード種別.BOX ) )
					{
						return order * n1.arスコア[ 0 ].ファイル情報.フォルダの絶対パス.CompareTo( n2.arスコア[ 0 ].ファイル情報.フォルダの絶対パス );
					}
					#endregion
					int nSumPlayCountN1 = 0, nSumPlayCountN2 = 0;
//					for( int i = 0; i <(int)Difficulty.Total; i++ )
//					{
						if( n1.arスコア[ nL12345 ] != null )
						{
							nSumPlayCountN1 += n1.arスコア[ nL12345 ].譜面情報.演奏回数[ (int) part ];
						}
						if( n2.arスコア[ nL12345 ] != null )
						{
							nSumPlayCountN2 += n2.arスコア[ nL12345 ].譜面情報.演奏回数[ (int) part ];
						}
//					}
					var num = nSumPlayCountN2 - nSumPlayCountN1;
					if( num != 0 )
					{
						return order * num;
					}
					return order * n1.strタイトル.CompareTo( n2.strタイトル );
				} );
				foreach ( C曲リストノード c曲リストノード in ノードリスト )
				{
					int nSumPlayCountN1 = 0;
//					for ( int i = 0; i < 5; i++ )
//					{
						if ( c曲リストノード.arスコア[ nL12345 ] != null )
						{
							nSumPlayCountN1 += c曲リストノード.arスコア[ nL12345 ].譜面情報.演奏回数[ (int) part ];
						}
//					}
// Debug.WriteLine( nSumPlayCountN1 + ":" + c曲リストノード.strタイトル );
				}
			}
		}
		public static void t曲リストのソート4_LEVEL順( List<C曲リストノード> ノードリスト, E楽器パート part, int order, params object[] p )
		{
			order = -order;
			int nL12345 = (int)p[ 0 ];
			if ( part != E楽器パート.UNKNOWN )
			{
				ノードリスト.Sort( delegate( C曲リストノード n1, C曲リストノード n2 )
				{
					#region [ 共通処理 ]
					if ( ( n1.eノード種別 == C曲リストノード.Eノード種別.BOX ) && ( n2.eノード種別 == C曲リストノード.Eノード種別.BOX ) )
					{
						return order * n1.arスコア[ 0 ].ファイル情報.フォルダの絶対パス.CompareTo( n2.arスコア[ 0 ].ファイル情報.フォルダの絶対パス );
					}
					#endregion
					int nSumPlayCountN1 = 0, nSumPlayCountN2 = 0;
					if ( n1.arスコア[ nL12345 ] != null )
					{
						nSumPlayCountN1 = n1.nLevel[ nL12345 ];
					}
					if ( n2.arスコア[ nL12345 ] != null )
					{
						nSumPlayCountN2 = n2.nLevel[ nL12345 ];
					}
					var num = nSumPlayCountN2 - nSumPlayCountN1;
					if ( num != 0 )
					{
						return order * num;
					}
					return order * n1.strタイトル.CompareTo( n2.strタイトル );
				} );
				foreach ( C曲リストノード c曲リストノード in ノードリスト )
				{
					int nSumPlayCountN1 = 0;
					if ( c曲リストノード.arスコア[ nL12345 ] != null )
					{
						nSumPlayCountN1 = c曲リストノード.nLevel[ nL12345 ];
					}
// Debug.WriteLine( nSumPlayCountN1 + ":" + c曲リストノード.strタイトル );
				}
			}
		}
		public static void t曲リストのソート5_BestRank順( List<C曲リストノード> ノードリスト, E楽器パート part, int order, params object[] p )
		{
			order = -order;
			int nL12345 = (int) p[ 0 ];
			if ( part != E楽器パート.UNKNOWN )
			{
				ノードリスト.Sort( delegate( C曲リストノード n1, C曲リストノード n2 )
				{
					#region [ 共通処理 ]
					if ( ( n1.eノード種別 == C曲リストノード.Eノード種別.BOX ) && ( n2.eノード種別 == C曲リストノード.Eノード種別.BOX ) )
					{
						return order * n1.arスコア[ 0 ].ファイル情報.フォルダの絶対パス.CompareTo( n2.arスコア[ 0 ].ファイル情報.フォルダの絶対パス );
					}
					#endregion
					int nSumPlayCountN1 = 0, nSumPlayCountN2 = 0;
					bool isFullCombo1 = false, isFullCombo2 = false;
					if ( n1.arスコア[ nL12345 ] != null )
					{
						isFullCombo1 = n1.arスコア[ nL12345 ].譜面情報.フルコンボ[ (int) part ];
						nSumPlayCountN1 = n1.arスコア[ nL12345 ].譜面情報.最大ランク[ (int) part ];
					}
					if ( n2.arスコア[ nL12345 ] != null )
					{
						isFullCombo2 = n2.arスコア[ nL12345 ].譜面情報.フルコンボ[ (int) part ];
						nSumPlayCountN2 = n2.arスコア[ nL12345 ].譜面情報.最大ランク[ (int) part ];
					}
					if ( isFullCombo1 ^ isFullCombo2 )
					{
						if ( isFullCombo1 ) return order; else return -order;
					}
					var num = nSumPlayCountN2 - nSumPlayCountN1;
					if ( num != 0 )
					{
						return order * num;
					}
					return order * n1.strタイトル.CompareTo( n2.strタイトル );
				} );
				foreach ( C曲リストノード c曲リストノード in ノードリスト )
				{
					int nSumPlayCountN1 = 0;
					if ( c曲リストノード.arスコア[ nL12345 ] != null )
					{
						nSumPlayCountN1 = c曲リストノード.arスコア[ nL12345 ].譜面情報.最大ランク[ (int) part ];
					}
// Debug.WriteLine( nSumPlayCountN1 + ":" + c曲リストノード.strタイトル );
				}
			}
		}
		public static void t曲リストのソート6_SkillPoint順( List<C曲リストノード> ノードリスト, E楽器パート part, int order, params object[] p )
		{
			order = -order;
			int nL12345 = (int) p[ 0 ];
			if ( part != E楽器パート.UNKNOWN )
			{
				ノードリスト.Sort( delegate( C曲リストノード n1, C曲リストノード n2 )
				{
					#region [ 共通処理 ]
					if ( ( n1.eノード種別 == C曲リストノード.Eノード種別.BOX ) && ( n2.eノード種別 == C曲リストノード.Eノード種別.BOX ) )
					{
						return order * n1.arスコア[ 0 ].ファイル情報.フォルダの絶対パス.CompareTo( n2.arスコア[ 0 ].ファイル情報.フォルダの絶対パス );
					}
					#endregion
					double nSumPlayCountN1 = 0, nSumPlayCountN2 = 0;
					if ( n1.arスコア[ nL12345 ] != null )
					{
						nSumPlayCountN1 = n1.arスコア[ nL12345 ].譜面情報.最大スキル[ (int) part ];
					}
					if ( n2.arスコア[ nL12345 ] != null )
					{
						nSumPlayCountN2 = n2.arスコア[ nL12345 ].譜面情報.最大スキル[ (int) part ];
					}
					double d = nSumPlayCountN2 - nSumPlayCountN1;
					if ( d != 0 )
					{
						return order * System.Math.Sign(d);
					}
					return order * n1.strタイトル.CompareTo( n2.strタイトル );
				} );
				foreach ( C曲リストノード c曲リストノード in ノードリスト )
				{
					double nSumPlayCountN1 = 0;
					if ( c曲リストノード.arスコア[ nL12345 ] != null )
					{
						nSumPlayCountN1 = c曲リストノード.arスコア[ nL12345 ].譜面情報.最大スキル[ (int) part ];
					}
// Debug.WriteLine( nSumPlayCountN1 + ":" + c曲リストノード.strタイトル );
				}
			}
		}
		public static void t曲リストのソート7_更新日時順( List<C曲リストノード> ノードリスト, E楽器パート part, int order, params object[] p )
		{
			int nL12345 = (int) p[ 0 ];
			if ( part != E楽器パート.UNKNOWN )
			{
				ノードリスト.Sort( delegate( C曲リストノード n1, C曲リストノード n2 )
				{
					#region [ 共通処理 ]
					if ( ( n1.eノード種別 == C曲リストノード.Eノード種別.BOX ) && ( n2.eノード種別 == C曲リストノード.Eノード種別.BOX ) )
					{
						return order * n1.arスコア[ 0 ].ファイル情報.フォルダの絶対パス.CompareTo( n2.arスコア[ 0 ].ファイル情報.フォルダの絶対パス );
					}
					#endregion
					DateTime nSumPlayCountN1 = DateTime.Parse("0001/01/01 12:00:01.000");
					DateTime nSumPlayCountN2 = DateTime.Parse("0001/01/01 12:00:01.000");
					if ( n1.arスコア[ nL12345 ] != null )
					{
						nSumPlayCountN1 = n1.arスコア[ nL12345 ].ファイル情報.最終更新日時;
					}
					if ( n2.arスコア[ nL12345 ] != null )
					{
						nSumPlayCountN2 = n2.arスコア[ nL12345 ].ファイル情報.最終更新日時;
					}
					int d = nSumPlayCountN1.CompareTo(nSumPlayCountN2);
					if ( d != 0 )
					{
						return order * System.Math.Sign( d );
					}
					return order * n1.strタイトル.CompareTo( n2.strタイトル );
				} );
				foreach ( C曲リストノード c曲リストノード in ノードリスト )
				{
					DateTime nSumPlayCountN1 = DateTime.Parse( "0001/01/01 12:00:01.000" );
					if ( c曲リストノード.arスコア[ nL12345 ] != null )
					{
						nSumPlayCountN1 = c曲リストノード.arスコア[ nL12345 ].ファイル情報.最終更新日時;
					}
// Debug.WriteLine( nSumPlayCountN1 + ":" + c曲リストノード.strタイトル );
				}
			}
		}

	    public static void t曲リストのソート9_ジャンル順(List<C曲リストノード> ノードリスト, E楽器パート part, int order, params object[] p)
	    {
	        try
	        {
	            var acGenreComparer = order == 1
	                ? (IComparer<C曲リストノード>) new C曲リストノードComparerAC8_14()
	                : new C曲リストノードComparerAC15();

	            var comparer = new ComparerChain<C曲リストノード>(
	                new C曲リストノードComparerノード種別(),
                    new C曲リストノードComparerPlaylistIndex(),
	                acGenreComparer,
	                new C曲リストノードComparer絶対パス(1),
	                new C曲リストノードComparerタイトル(1));

	            ノードリスト.Sort( comparer );
	        }
	        catch (Exception ex)
	        {
	            Trace.TraceError(ex.ToString());
	            Trace.TraceError("例外が発生しましたが処理を継続します。 (bca6dda7-76ad-42fc-a415-250f52c0b17d)");
	        }
	    }

#if TEST_SORTBGM
		public static void t曲リストのソート9_BPM順( List<C曲リストノード> ノードリスト, E楽器パート part, int order, params object[] p )
		{
			order = -order;
			int nL12345 = (int) p[ 0 ];
			if ( part != E楽器パート.UNKNOWN )
			{
				ノードリスト.Sort( delegate( C曲リストノード n1, C曲リストノード n2 )
				{
        #region [ 共通処理 ]
					if ( n1 == n2 )
					{
						return 0;
					}
					int num = this.t比較0_共通( n1, n2 );
					if ( num != 0 )
					{
						return num;
					}
					if ( ( n1.eノード種別 == C曲リストノード.Eノード種別.BOX ) && ( n2.eノード種別 == C曲リストノード.Eノード種別.BOX ) )
					{
						return order * n1.arスコア[ 0 ].ファイル情報.フォルダの絶対パス.CompareTo( n2.arスコア[ 0 ].ファイル情報.フォルダの絶対パス );
					}
        #endregion
					double dBPMn1 = 0.0, dBPMn2 = 0.0;
					if ( n1.arスコア[ nL12345 ] != null )
					{
						dBPMn1 = n1.arスコア[ nL12345 ].譜面情報.bpm;
					}
					if ( n2.arスコア[ nL12345 ] != null )
					{
						dBPMn2 = n2.arスコア[ nL12345 ].譜面情報.bpm;
					}
					double d = dBPMn1- dBPMn2;
					if ( d != 0 )
					{
						return order * System.Math.Sign( d );
					}
					return order * n1.strタイトル.CompareTo( n2.strタイトル );
				} );
				foreach ( C曲リストノード c曲リストノード in ノードリスト )
				{
					double dBPM = 0;
					if ( c曲リストノード.arスコア[ nL12345 ] != null )
					{
						dBPM = c曲リストノード.arスコア[ nL12345 ].譜面情報.bpm;
					}
Debug.WriteLine( dBPM + ":" + c曲リストノード.strタイトル );
				}
			}
		}
#endif

	    public static void t曲リストのソート10_Rating順(List<C曲リストノード> ノードリスト, E楽器パート part, int itemIndex, params object[] p)
	    {
	        try
	        {
	            var comparer = new ComparerChain<C曲リストノード>(
	                new C曲リストノードComparerノード種別(),
	                new C曲リストノードComparerRating(itemIndex),
                    new C曲リストノードComparerPlaylistIndex(),
	                new C曲リストノードComparerAC15(),
	                new C曲リストノードComparer絶対パス(1),
	                new C曲リストノードComparerタイトル(1));

	            ノードリスト.Sort( comparer );
	        }
	        catch (Exception ex)
	        {
	            Trace.TraceError(ex.ToString());
	            Trace.TraceError("例外が発生しましたが処理を継続します。 (bca6dda7-76ad-42fc-a415-250f52c0b17e)");
	        }
	    }
        
        //-----------------
        #endregion
        #region [ .score.ini を読み込んで Cスコア.譜面情報に設定する ]
        //-----------------
        public void tScoreIniを読み込んで譜面情報を設定する( string strScoreIniファイルパス, Cスコア score )
		{
			if( !File.Exists( strScoreIniファイルパス ) )
				return;

			try
			{
				var ini = new CScoreIni( strScoreIniファイルパス );
				ini.t全演奏記録セクションの整合性をチェックし不整合があればリセットする();

				for( int n楽器番号 = 0; n楽器番号 < 3; n楽器番号++ )
				{
					int n = ( n楽器番号 * 2 ) + 1;	// n = 0～5

					#region socre.譜面情報.最大ランク[ n楽器番号 ] = ... 
					//-----------------
					if( ini.stセクション[ n ].b演奏にMIDI入力を使用した ||
						ini.stセクション[ n ].b演奏にキーボードを使用した ||
						ini.stセクション[ n ].b演奏にジョイパッドを使用した ||
						ini.stセクション[ n ].b演奏にマウスを使用した )
					{
						// (A) 全オートじゃないようなので、演奏結果情報を有効としてランクを算出する。

						score.譜面情報.最大ランク[ n楽器番号 ] =
							CScoreIni.tランク値を計算して返す( 
								ini.stセクション[ n ].n全チップ数,
								ini.stセクション[ n ].nPerfect数, 
								ini.stセクション[ n ].nGreat数,
								ini.stセクション[ n ].nGood数, 
								ini.stセクション[ n ].nPoor数,
								ini.stセクション[ n ].nMiss数 );
					}
					else
					{
						// (B) 全オートらしいので、ランクは無効とする。

						score.譜面情報.最大ランク[ n楽器番号 ] = (int) CScoreIni.ERANK.UNKNOWN;
					}
					//-----------------
					#endregion
					score.譜面情報.最大スキル[ n楽器番号 ] = ini.stセクション[ n ].db演奏型スキル値;
					score.譜面情報.フルコンボ[ n楽器番号 ] = ini.stセクション[ n ].bフルコンボである;
                    score.譜面情報.ハイスコア = (int)ini.stセクション.HiScoreDrums.nスコア;
                    for( int i = 0; i < (int)Difficulty.Total; i++ )
                    {
                        score.譜面情報.nハイスコア[ i ] = (int)ini.stセクション.HiScoreDrums.nハイスコア[ i ];
                    }
				}
				score.譜面情報.演奏回数.Drums = ini.stファイル.PlayCountDrums;
				score.譜面情報.演奏回数.Guitar = ini.stファイル.PlayCountGuitar;
				score.譜面情報.演奏回数.Bass = ini.stファイル.PlayCountBass;
				for( int i = 0; i < (int)Difficulty.Total; i++ )
					score.譜面情報.演奏履歴[ i ] = ini.stファイル.History[ i ];
			}
			catch (Exception e)
			{
				Trace.TraceError( "演奏記録ファイルの読み込みに失敗しました。[{0}]", strScoreIniファイルパス );
				Trace.TraceError( e.ToString() );
				Trace.TraceError( "例外が発生しましたが処理を継続します。 (801f823d-a952-4809-a1bb-cf6a56194f5c)" );
			}
		}
		//-----------------
		#endregion

		// その他

		#region [ private ]
		//-----------------
		private const string SONGSDB_VERSION = "SongsDB6";
		private List<string> listStrBoxDefSkinSubfolderFullName;

		/// <summary>
		/// 検索を中断_スローダウンする
		/// </summary>
		private void SlowOrSuspendSearchTask()
		{
			if ( this.bIsSuspending )		// #27060 中断要求があったら、解除要求が来るまで待機
			{
				autoReset.WaitOne();
			}
			if ( this.bIsSlowdown && ++this.searchCount > 10 )			// #27060 #PREMOVIE再生中は検索負荷を下げる
			{
				Thread.Sleep( 100 );
				this.searchCount = 0;
			}
		}

		//-----------------
		#endregion
	}
}
　
