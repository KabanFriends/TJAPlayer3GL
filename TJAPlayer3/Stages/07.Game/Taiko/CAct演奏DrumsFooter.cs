using FDK;

namespace TJAPlayer3
{
    internal class CAct演奏DrumsFooter : CActivity
    {
        /// <summary>
        /// フッター
        /// </summary>
        public CAct演奏DrumsFooter()
        {
            base.b活性化してない = true;
        }

        public override int On進行描画()
        {
            TJAPlayer3.Tx.Mob_Footer?.t2D描画(TJAPlayer3.app.Device, 0, 720 - TJAPlayer3.Tx.Mob_Footer.szテクスチャサイズ.Height);

            return base.On進行描画();
        }
    }
}
