using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace TJAPlayer3
{
    internal static class SongRatingController
    {
        private static readonly IDictionary<string, SongRating> RatingsByAbsoluteTjaPath =
            new Dictionary<string, SongRating>();

        public static SongRating Toggle(string absoluteTjaPath, SongRating ratingToggled)
        {
            var rating = GetRating(absoluteTjaPath);

            var newRating = rating == ratingToggled ? SongRating.Unset : ratingToggled;

            SetRating(absoluteTjaPath, newRating);

            return newRating;
        }

        public static SongRating GetRating(string absoluteTjaPath)
        {
            if (RatingsByAbsoluteTjaPath.TryGetValue(absoluteTjaPath, out var rating))
            {
                return rating;
            }

            rating = GetRatingImpl(absoluteTjaPath);
            RatingsByAbsoluteTjaPath.Add(absoluteTjaPath, rating);
            return rating;
        }

        private static void SetRating(string absoluteTjaPath, SongRating rating)
        {
            RatingsByAbsoluteTjaPath[absoluteTjaPath] = rating;
            SetRatingImpl(absoluteTjaPath, rating);
        }

        private static SongRating GetRatingImpl(string absoluteTjaPath)
        {
            var absoluteRatingPath = GetAbsoluteRatingPath(absoluteTjaPath);

            if (!File.Exists(absoluteRatingPath))
            {
                return SongRating.Unset;
            }

            var lines = File.ReadAllLines(absoluteRatingPath, Encoding.UTF8);

            return (SongRating)int.Parse(lines[0]);
        }

        private static void SetRatingImpl(string absoluteTjaPath, SongRating rating)
        {
            var absoluteRatingPath = GetAbsoluteRatingPath(absoluteTjaPath);

            File.Delete(absoluteRatingPath);

            if (rating == SongRating.Unset)
            {
                return;
            }

            var lines = new[]
            {
                ((int)rating).ToString(),
                DateTimeOffset.UtcNow.ToString("O")
            };

            File.WriteAllLines(absoluteRatingPath, lines, Encoding.UTF8);
        }

        private static string GetAbsoluteRatingPath(string absoluteTjaPath)
        {
            return absoluteTjaPath + ".rating";
        }
    }
}