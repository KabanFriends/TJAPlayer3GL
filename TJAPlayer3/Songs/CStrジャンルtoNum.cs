namespace TJAPlayer3
{
    internal static class CStrジャンルtoNum
    {
        internal static int ForAC8_14SortOrder( string strジャンル )
        {
            switch( strジャンル )
            {
                case CStrジャンル.アニメ:
                    return 0;
                case CStrジャンル.JPOP:
                    return 1;
                case CStrジャンル.ゲームミュージック:
                    return 2;
                case CStrジャンル.ナムコオリジナル:
                    return 3;
                case CStrジャンル.クラシック:
                    return 4;
                case CStrジャンル.どうよう:
                    return 5;
                case CStrジャンル.バラエティ:
                    return 6;
                case CStrジャンル.ボーカロイドJP:
                case CStrジャンル.ボーカロイドEN:
                    return 7;
                default:
                    return 8;
            }
        }

        internal static int ForAC15SortOrder( string strジャンル )
        {
            switch ( strジャンル )
            {
                case CStrジャンル.JPOP:
                    return 0;
                case CStrジャンル.アニメ:
                    return 1;
                case CStrジャンル.ボーカロイドJP:
                case CStrジャンル.ボーカロイドEN:
                    return 2;
                case CStrジャンル.どうよう:
                    return 3;
                case CStrジャンル.バラエティ:
                    return 4;
                case CStrジャンル.クラシック:
                    return 5;
                case CStrジャンル.ゲームミュージック:
                    return 6;
                case CStrジャンル.ナムコオリジナル:
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
                case CStrジャンル.JPOP:
                    return 1;
                case CStrジャンル.アニメ:
                    return 2;
                case CStrジャンル.ゲームミュージック:
                    return 3;
                case CStrジャンル.ナムコオリジナル:
                    return 4;
                case CStrジャンル.クラシック:
                    return 5;
                case CStrジャンル.バラエティ:
                    return 6;
                case CStrジャンル.どうよう:
                    return 7;
                case CStrジャンル.ボーカロイドJP:
                case CStrジャンル.ボーカロイドEN:
                    return 8;
                default:
                    return 0;
            }
        }

        internal static int ForGenreTextIndex( string strジャンル )
        {
            return ForAC8_14SortOrder( strジャンル );
        }
    }
}