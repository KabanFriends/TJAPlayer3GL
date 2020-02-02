using System.Linq;
using FDK;

namespace TJAPlayer3
{
    internal static class SongRatingControlHandler
    {
        public static void Handle(IInputDevice keyboard, CActSelect曲リスト act曲リスト, CEnumSongs enumSongs)
        {
            var 曲リストノード = act曲リスト.r現在選択中の曲;

            if (曲リストノード == null)
            {
                return;
            }

            if (曲リストノード.eノード種別 != C曲リストノード.Eノード種別.SCORE)
            {
                return;
            }

            if (!keyboard.bキーが押されている((int) SlimDX.DirectInput.Key.LeftControl) &&
                !keyboard.bキーが押されている((int) SlimDX.DirectInput.Key.RightControl))
            {
                return;
            }

            var ratingKeyPressed = GetSongRatingKeyPressed(keyboard);

            if (ratingKeyPressed == null)
            {
                return;
            }

            var スコア = 曲リストノード.arスコア.FirstOrDefault(o => o != null);

            if (スコア == null)
            {
                return;
            }

            var absoluteTjaPath = スコア.ファイル情報.ファイルの絶対パス;

            var newRating = SongRatingController.Toggle(absoluteTjaPath, ratingKeyPressed.Value);

            foreach (var cスコア in 曲リストノード.arスコア)
            {
                if (cスコア == null)
                {
                    continue;
                }

                cスコア.譜面情報.Rating = newRating;
            }

            act曲リスト.OnSelectedSongRatingChanged(newRating);
        }

        private static SongRating? GetSongRatingKeyPressed(IInputDevice keyboard)
        {
            if (keyboard.bキーが押された((int) SlimDX.DirectInput.Key.D0))
            {
                return SongRating.Unset;
            }

            if (keyboard.bキーが押された((int) SlimDX.DirectInput.Key.D1))
            {
                return SongRating.One;
            }

            if (keyboard.bキーが押された((int) SlimDX.DirectInput.Key.D2))
            {
                return SongRating.Two;
            }

            if (keyboard.bキーが押された((int) SlimDX.DirectInput.Key.D3))
            {
                return SongRating.Three;
            }

            if (keyboard.bキーが押された((int) SlimDX.DirectInput.Key.D4))
            {
                return SongRating.Four;
            }

            if (keyboard.bキーが押された((int) SlimDX.DirectInput.Key.D5))
            {
                return SongRating.Five;
            }

            return null;
        }
    }
}
