﻿using System;
using System.Collections.Generic;
using System.Drawing;

namespace TJAPlayer3
{
	[Serializable]
	internal class C曲リストノード
	{
		// プロパティ

        public int? nIndex;
        public Eノード種別 eノード種別 = Eノード種別.UNKNOWN;
		public enum Eノード種別
		{
			SCORE,
			SCORE_MIDI,
			BOX,
			BACKBOX,
			RANDOM,
			UNKNOWN
		}
		public int nID { get; private set; }
		public Cスコア[] arスコア = new Cスコア[(int)Difficulty.Total];
		public string[] ar難易度ラベル = new string[(int)Difficulty.Total];
		public bool bDTXFilesで始まるフォルダ名のBOXである;
        public Color ForeColor = TJAPlayer3.Skin.SongSelect_ForeColor;
        public Color BackColor = TJAPlayer3.Skin.SongSelect_BackColor;
		public List<C曲リストノード> listランダム用ノードリスト;
		public List<C曲リストノード> list子リスト;
		public int nGood範囲ms = -1;
		public int nGreat範囲ms = -1;
		public int nPerfect範囲ms = -1;
		public int nPoor範囲ms = -1;
		public int nスコア数;
		public C曲リストノード r親ノード;
		public Stack<int> stackランダム演奏番号 = new Stack<int>();
		public string strジャンル = "";
		public string strタイトル = "";
        public string strサブタイトル = "";
		public string strBreadcrumbs = "";		// #27060 2011.2.27 yyagi; MUSIC BOXのパンくずリスト (曲リスト構造内の絶対位置捕捉のために使う)
		public string strSkinPath = "";			// #28195 2012.5.4 yyagi; box.defでのスキン切り替え対応
        public int[] nLevel = new int[(int)Difficulty.Total]{ 0, 0, 0, 0, 0, 0, 0 };
		
		// コンストラクタ

		public C曲リストノード()
		{
			this.nID = id++;
		}


		// その他

		#region [ private ]
		//-----------------
		private static int id;
		//-----------------
		#endregion
	}
}
