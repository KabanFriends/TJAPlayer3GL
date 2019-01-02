using NUnit.Framework;

namespace TJAPlayer3.Tests.Songs
{
    [TestFixture]
    public sealed class CStrジャンルtoStrTests
    {
        [Test]
        [TestCase("アニメ", "Anime")]
        [TestCase("J-POP", "J-POP")]
        [TestCase("ゲームミュージック", "Game")]
        [TestCase("ナムコオリジナル", "Namco")]
        [TestCase("クラシック", "Classic")]
        [TestCase("どうよう", "Child")]
        [TestCase("バラエティ", "Variety")]
        [TestCase("ボーカロイド", "Vocaloid")]
        [TestCase("VOCALOID", "Vocaloid")]
        [TestCase(null, null)]
        [TestCase("", null)]
        [TestCase(" ", null)]
        [TestCase("unknown value", null)]
        [TestCase(" アニメ", null)]
        [TestCase(" アニメ ", null)]
        [TestCase("アニメ ", null)]
        public void TestForTextureFileName(string strジャンル, string expected)
        {
            var actual = CStrジャンルtoStr.ForTextureFileName(strジャンル);

            Assert.That(actual, Is.EqualTo(expected));
        }
    }
}