using NUnit.Framework;

namespace TJAPlayer3.Tests.Songs
{
    [TestFixture]
    public sealed class CStrジャンルtoNumTests
    {
        [Test]
        [TestCase("アニメ", 0)]
        [TestCase("J-POP", 1)]
        [TestCase("ゲームミュージック", 2)]
        [TestCase("ナムコオリジナル", 3)]
        [TestCase("クラシック", 4)]
        [TestCase("どうよう", 5)]
        [TestCase("バラエティ", 6)]
        [TestCase("ボーカロイド", 7)]
        [TestCase("VOCALOID", 7)]
        [TestCase(null, 8)]
        [TestCase("", 8)]
        [TestCase(" ", 8)]
        [TestCase("unknown value", 8)]
        [TestCase(" アニメ", 8)]
        [TestCase(" アニメ ", 8)]
        [TestCase("アニメ ", 8)]
        public void TestForAC8_14SortOrder(string strジャンル, int expected)
        {
            var actual = CStrジャンルtoNum.ForAC8_14SortOrder(strジャンル);

            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        [TestCase("J-POP", EジャンルAC15SortOrder.JPOP)]
        [TestCase("アニメ", EジャンルAC15SortOrder.アニメ)]
        [TestCase("ボーカロイド", EジャンルAC15SortOrder.ボーカロイド)]
        [TestCase("VOCALOID", EジャンルAC15SortOrder.ボーカロイド)]
        [TestCase("どうよう", EジャンルAC15SortOrder.どうよう)]
        [TestCase("バラエティ", EジャンルAC15SortOrder.バラエティ)]
        [TestCase("クラシック", EジャンルAC15SortOrder.クラシック)]
        [TestCase("ゲームミュージック", EジャンルAC15SortOrder.ゲームミュージック)]
        [TestCase("ナムコオリジナル", EジャンルAC15SortOrder.ナムコオリジナル)]
        [TestCase(null, EジャンルAC15SortOrder.Unknown)]
        [TestCase("", EジャンルAC15SortOrder.Unknown)]
        [TestCase(" ", EジャンルAC15SortOrder.Unknown)]
        [TestCase("unknown value", EジャンルAC15SortOrder.Unknown)]
        [TestCase(" アニメ", EジャンルAC15SortOrder.Unknown)]
        [TestCase(" アニメ ", EジャンルAC15SortOrder.Unknown)]
        [TestCase("アニメ ", EジャンルAC15SortOrder.Unknown)]
        public void TestForAC15SortOrder(string strジャンル, EジャンルAC15SortOrder expected)
        {
            var actual = CStrジャンルtoNum.ForAC15SortOrder(strジャンル);

            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        [TestCase(null, 0)]
        [TestCase("", 0)]
        [TestCase(" ", 0)]
        [TestCase("unknown value", 0)]
        [TestCase(" アニメ", 0)]
        [TestCase(" アニメ ", 0)]
        [TestCase("アニメ ", 0)]
        [TestCase("J-POP", 1)]
        [TestCase("アニメ", 2)]
        [TestCase("ゲームミュージック", 3)]
        [TestCase("ナムコオリジナル", 4)]
        [TestCase("クラシック", 5)]
        [TestCase("バラエティ", 6)]
        [TestCase("どうよう", 7)]
        [TestCase("ボーカロイド", 8)]
        [TestCase("VOCALOID", 8)]
        public void TestForBarGenreIndex(string strジャンル, int expected)
        {
            var actual = CStrジャンルtoNum.ForBarGenreIndex(strジャンル);

            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        [TestCase(null, 0)]
        [TestCase("", 0)]
        [TestCase(" ", 0)]
        [TestCase("unknown value", 0)]
        [TestCase(" アニメ", 0)]
        [TestCase(" アニメ ", 0)]
        [TestCase("アニメ ", 0)]
        [TestCase("J-POP", 1)]
        [TestCase("アニメ", 2)]
        [TestCase("ゲームミュージック", 3)]
        [TestCase("ナムコオリジナル", 4)]
        [TestCase("クラシック", 5)]
        [TestCase("バラエティ", 6)]
        [TestCase("どうよう", 7)]
        [TestCase("ボーカロイド", 8)]
        [TestCase("VOCALOID", 8)]
        public void TestForFrameBoxIndex(string strジャンル, int expected)
        {
            var actual = CStrジャンルtoNum.ForFrameBoxIndex(strジャンル);

            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        [TestCase(null, 0)]
        [TestCase("", 0)]
        [TestCase(" ", 0)]
        [TestCase("unknown value", 0)]
        [TestCase(" アニメ", 0)]
        [TestCase(" アニメ ", 0)]
        [TestCase("アニメ ", 0)]
        [TestCase("J-POP", 1)]
        [TestCase("アニメ", 2)]
        [TestCase("ゲームミュージック", 3)]
        [TestCase("ナムコオリジナル", 4)]
        [TestCase("クラシック", 5)]
        [TestCase("バラエティ", 6)]
        [TestCase("どうよう", 7)]
        [TestCase("ボーカロイド", 8)]
        [TestCase("VOCALOID", 8)]
        public void TestForGenreBackIndex(string strジャンル, int expected)
        {
            var actual = CStrジャンルtoNum.ForGenreBackIndex(strジャンル);

            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        [TestCase("アニメ", 0)]
        [TestCase("J-POP", 1)]
        [TestCase("ゲームミュージック", 2)]
        [TestCase("ナムコオリジナル", 3)]
        [TestCase("クラシック", 4)]
        [TestCase("どうよう", 5)]
        [TestCase("バラエティ", 6)]
        [TestCase("ボーカロイド", 7)]
        [TestCase("VOCALOID", 7)]
        [TestCase(null, 8)]
        [TestCase("", 8)]
        [TestCase(" ", 8)]
        [TestCase("unknown value", 8)]
        [TestCase(" アニメ", 8)]
        [TestCase(" アニメ ", 8)]
        [TestCase("アニメ ", 8)]
        public void TestForGenreTextIndex(string strジャンル, int expected)
        {
            var actual = CStrジャンルtoNum.ForGenreTextIndex(strジャンル);

            Assert.That(actual, Is.EqualTo(expected));
        }
    }
}