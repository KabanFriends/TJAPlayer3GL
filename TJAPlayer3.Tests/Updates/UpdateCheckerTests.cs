using NUnit.Framework;
using TJAPlayer3.Updates;

namespace TJAPlayer3.Tests.Updates
{
    [TestFixture]
    public sealed class UpdateCheckerTests
    {
        [Test]
        public void TestDeserialize()
        {
            const string releaseJson =
                "{\"name\": \"TJAPlayer3 v4.7.1\", \"tag_name\": \"v4.7.1\", \"html_url\": \"https://github.com/twopointzero/TJAPlayer3/releases/tag/v4.7.1\"}";

            var actual = UpdateChecker.Deserialize(releaseJson);

            Assert.That(actual, Is.Not.Null);
            Assert.That(actual.HtmlUrl, Is.EqualTo("https://github.com/twopointzero/TJAPlayer3/releases/tag/v4.7.1"));
            Assert.That(actual.Name, Is.EqualTo("TJAPlayer3 v4.7.1"));
            Assert.That(actual.TagName, Is.EqualTo("v4.7.1"));
        }

        [Test]
        [TestCase("v4.7.1", "v4.7.0", true)]
        [TestCase("v4.7.1", "v4.7.1", false)]
        [TestCase("v4.7.1", "v4.8.0", true)]
        public void TestShouldOfferUpdate(
            string appDisplayThreePartVersion,
            string gitHubReleaseTagName,
            bool shouldOfferUpgrade)
        {
            Assert.That(UpdateChecker.ShouldOfferUpdate(appDisplayThreePartVersion, gitHubReleaseTagName), Is.EqualTo(shouldOfferUpgrade));
        }
    }
}
