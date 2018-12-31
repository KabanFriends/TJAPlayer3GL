namespace TJAPlayer3
{
    internal static class CStrジャンルtoNum
    {
        internal static int ForAC8_14SortOrder( string strジャンル )
        {
            switch( strジャンル )
            {
                case "アニメ":
                    return 0;
                case "J-POP":
                    return 1;
                case "ゲームミュージック":
                    return 2;
                case "ナムコオリジナル":
                    return 3;
                case "クラシック":
                    return 4;
                case "どうよう":
                    return 5;
                case "バラエティ":
                    return 6;
                case "ボーカロイド":
                case "VOCALOID":
                    return 7;
                default:
                    return 8;
            }
        }

        internal static int ForAC15SortOrder( string strジャンル )
        {
            switch (strジャンル)
            {
                case "J-POP":
                    return 0;
                case "アニメ":
                    return 1;
                case "ボーカロイド":
                case "VOCALOID":
                    return 2;
                case "どうよう":
                    return 3;
                case "バラエティ":
                    return 4;
                case "クラシック":
                    return 5;
                case "ゲームミュージック":
                    return 6;
                case "ナムコオリジナル":
                    return 7;
                default:
                    return 8;
            }
        }

        internal static int ForBarGenreIndex( string strジャンル )
        {
            return ForGenreBackIndex( strジャンル );
        }

        internal static int ForFrameBoxIndex( string strジャンル )
        {
            return ForGenreBackIndex( strジャンル );
        }

        internal static int ForGenreBackIndex( string strジャンル )
        {
            switch ( strジャンル )
            {
                case "J-POP":
                    return 1;
                case "アニメ":
                    return 2;
                case "ゲームミュージック":
                    return 3;
                case "ナムコオリジナル":
                    return 4;
                case "クラシック":
                    return 5;
                case "バラエティ":
                    return 6;
                case "どうよう":
                    return 7;
                case "ボーカロイド":
                case "VOCALOID":
                    return 8;
                default:
                    return 0;
            }
        }

        internal static int ForGenreTextIndex( string strジャンル )
        {
            return ForAC8_14SortOrder( strジャンル );
        }
        
        internal static string ForTextureFileName( string genreName )
        {
            switch (genreName)
            {
                case "アニメ":
                    return "Anime";
                case "J-POP":
                    return "J-POP";
                case "ゲームミュージック":
                    return "Game";
                case "ナムコオリジナル":
                    return "Namco";
                case "クラシック":
                    return "Classic";
                case "どうよう":
                    return "Child";
                case "バラエティ":
                    return "Variety";
                case "ボーカロイド":
                case "VOCALOID":
                    return "Vocaloid";
                default:
                    return null;
            }
        }
    }
}