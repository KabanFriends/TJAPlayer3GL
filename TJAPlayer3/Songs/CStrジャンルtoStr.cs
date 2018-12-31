namespace TJAPlayer3
{
    internal static class CStrジャンルtoStr
    {
        internal static string ForTextureFileName( string genreName )
        {
            switch (genreName)
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