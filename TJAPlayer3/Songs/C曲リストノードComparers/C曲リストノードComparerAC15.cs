using System.Collections.Generic;

namespace TJAPlayer3.C曲リストノードComparers
{
    internal sealed class C曲リストノードComparerAC15 : IComparer<C曲リストノード>
    {
        public int Compare(C曲リストノード n1, C曲リストノード n2)
        {
            return CStrジャンルtoNum.ForAC15SortOrder(n1.strジャンル).CompareTo(CStrジャンルtoNum.ForAC15SortOrder(n2.strジャンル));
        }
    }
}