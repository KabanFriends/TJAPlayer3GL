namespace TJAPlayer3
{
    public static class CStrジャンルtoStr
    {
        public static string ForTextureFileName( string strジャンル )
        {
            switch (strジャンル)
            {
                case CStrジャンル.アニメ:
                    return "Anime";
                case CStrジャンル.JPOP:
                    return CStrジャンル.JPOP;
                case CStrジャンル.ゲームミュージック:
                    return "Game";
                case CStrジャンル.ナムコオリジナル:
                    return "Namco";
                case CStrジャンル.クラシック:
                    return "Classic";
                case CStrジャンル.どうよう:
                    return "Child";
                case CStrジャンル.バラエティ:
                    return "Variety";
                case CStrジャンル.ボーカロイドJP:
                case CStrジャンル.ボーカロイドEN:
                    return "Vocaloid";
                default:
                    return null;
            }
        }
    }
}