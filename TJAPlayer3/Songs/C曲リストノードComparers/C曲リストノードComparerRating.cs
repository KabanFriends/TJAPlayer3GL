using System;
using System.Collections.Generic;
using System.Linq;

namespace TJAPlayer3.C曲リストノードComparers
{
    internal sealed class C曲リストノードComparerRating : IComparer<C曲リストノード>
    {
        private static readonly KeyValuePair<string, Func<SongRating, int>>[] ToComparables = new[]
        {
            new KeyValuePair<string, Func<SongRating, int>>("0, 1, 2,...", ToComparableByIndividualRatingsAscending), 
            new KeyValuePair<string, Func<SongRating, int>>("5, 4, 3,...", ToComparableByIndividualRatingsDescending), 
            new KeyValuePair<string, Func<SongRating, int>>("1-5", (rating) => ToComparable(rating, false, 1, 5)), 
            new KeyValuePair<string, Func<SongRating, int>>("2-5", (rating) => ToComparable(rating, false, 2, 5)), 
            new KeyValuePair<string, Func<SongRating, int>>("3-5", (rating) => ToComparable(rating, false, 3, 5)), 
            new KeyValuePair<string, Func<SongRating, int>>("4-5", (rating) => ToComparable(rating, false, 4, 5)), 
            new KeyValuePair<string, Func<SongRating, int>>("5", (rating) => ToComparable(rating, false, 5, 5)), 
            new KeyValuePair<string, Func<SongRating, int>>("0 & 1-5", (rating) => ToComparable(rating, true, 1, 5)), 
            new KeyValuePair<string, Func<SongRating, int>>("0 & 2-5", (rating) => ToComparable(rating, true, 2, 5)), 
            new KeyValuePair<string, Func<SongRating, int>>("0 & 3-5", (rating) => ToComparable(rating, true, 3, 5)), 
            new KeyValuePair<string, Func<SongRating, int>>("0 & 4-5", (rating) => ToComparable(rating, true, 4, 5)), 
            new KeyValuePair<string, Func<SongRating, int>>("0 & 5", (rating) => ToComparable(rating, true, 5, 5)), 
            new KeyValuePair<string, Func<SongRating, int>>("0", (rating) => ToComparable(rating, true, -1, -1)), 
        };

        private readonly Dictionary<object, int> _memoizedComparables = new Dictionary<object, int>();

        private readonly Func<SongRating, int> _toComparable;

        public C曲リストノードComparerRating(int itemIndex)
        {
            _toComparable = ToComparables[itemIndex].Value;
        }

        public int Compare(C曲リストノード x, C曲リストノード y)
        {
            return ToComparable(x).CompareTo(ToComparable(y));
        }

        private int ToComparable(C曲リストノード value)
        {
            if (!_memoizedComparables.TryGetValue(value, out var comparable))
            {
                comparable = _toComparable(GetRating(value));
                _memoizedComparables.Add(value, comparable);
            }

            return comparable;
        }

        private static SongRating GetRating(C曲リストノード x)
        {
            return x.arスコア.FirstOrDefault(o => o != null)?.譜面情報.Rating ?? SongRating.Unset;
        }

        private static int ToComparableByIndividualRatingsAscending(SongRating songRating)
        {
            return (int) songRating;
        }

        private static int ToComparableByIndividualRatingsDescending(SongRating songRating)
        {
            return -(int) songRating;
        }

        private static int ToComparable(SongRating rating, bool group1IncludesUnset, int group1Min, int group1Max)
        {
            if (rating == SongRating.Unset)
            {
                return group1IncludesUnset ? 1 : 2;
            }

            var intRating = (int) rating;

            if (group1Min <= intRating && intRating <= group1Max)
            {
                return 1;
            }

            return 2;
        }

        public static string[] GetItemListValues()
        {
            return ToComparables.Select(o => o.Key).ToArray();
        }
    }
}