using System.Collections.Generic;

namespace TJAPlayer3.C曲リストノードComparers
{
    internal sealed class C曲リストノードComparerPlaylistIndex : IComparer<C曲リストノード>
    {
        public int Compare(C曲リストノード n1, C曲リストノード n2)
        {
            return (n1.nIndex ?? int.MaxValue).CompareTo( n2.nIndex ?? int.MaxValue );
        }
    }
}