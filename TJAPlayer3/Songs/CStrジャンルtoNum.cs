namespace TJAPlayer3
{
    public static class CStrジャンルtoNum
    {
        public static int ForAC8_14SortOrder( string strジャンル )
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

        public static int ForAC15SortOrder( string strジャンル )
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

        public static int ForBarGenreIndex( string strジャンル )
        {
            return ForGenreBackIndex( strジャンル );
        }

        public static int ForFrameBoxIndex( string strジャンル )
        {
            return ForGenreBackIndex( strジャンル );
        }

        public static int ForGenreBackIndex( string strジャンル )
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

        public static int ForGenreTextIndex( string strジャンル )
        {
            return ForAC8_14SortOrder( strジャンル );
        }
    }
}