using FDK;

namespace TJAPlayer3
{
    internal class FastRender
    {
        public static void Render()
        {
            NullCheckAndRender(TJAPlayer3.Tx.Chara_10Combo);
            NullCheckAndRender(TJAPlayer3.Tx.Chara_10Combo_Maxed);
            NullCheckAndRender(TJAPlayer3.Tx.Chara_GoGoStart);
            NullCheckAndRender(TJAPlayer3.Tx.Chara_GoGoStart_Maxed);
            NullCheckAndRender(TJAPlayer3.Tx.Chara_Normal);
            NullCheckAndRender(TJAPlayer3.Tx.Chara_Normal_Cleared);
            NullCheckAndRender(TJAPlayer3.Tx.Chara_Become_Cleared);
            NullCheckAndRender(TJAPlayer3.Tx.Chara_Become_Maxed);
            NullCheckAndRender(TJAPlayer3.Tx.Chara_Balloon_Breaking);
            NullCheckAndRender(TJAPlayer3.Tx.Chara_Balloon_Broke);
            NullCheckAndRender(TJAPlayer3.Tx.Chara_Balloon_Miss);
            NullCheckAndRender(TJAPlayer3.Tx.Dancer);
            NullCheckAndRender(TJAPlayer3.Tx.Effects_GoGoSplash);
            NullCheckAndRender(TJAPlayer3.Tx.Runner);
            NullCheckAndRender(TJAPlayer3.Tx.Mob);
            NullCheckAndRender(TJAPlayer3.Tx.PuchiChara);
        }

        private static void NullCheckAndRender(CTexture[][] textures)
        {
            if (textures == null)
            {
                return;
            }

            foreach (var innerTextures in textures)
            {
                NullCheckAndRender(innerTextures);
            }
        }

        private static void NullCheckAndRender(CTexture[] textures)
        {
            if (textures == null)
            {
                return;
            }

            foreach (var texture in textures)
            {
                NullCheckAndRender(texture);
            }
        }

        private static void NullCheckAndRender(CTexture texture)
        {
            if (texture == null)
            {
                return;
            }

            var originalOpacity = texture.Opacity;

            texture.Opacity = 0;
            texture.t2D描画(TJAPlayer3.app.Device, 0, 0);
            texture.Opacity = originalOpacity;
        }
    }
}
