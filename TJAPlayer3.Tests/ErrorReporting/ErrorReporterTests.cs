using NUnit.Framework;
using TJAPlayer3.ErrorReporting;

namespace TJAPlayer3.Tests.ErrorReporting
{
    [TestFixture]
    public sealed class ErrorReporterTests
    {
        [Test]
        [TestCase("This is only a test", "pPi+NdUkNVp81f+/9Vi7dvgVdtr6f6WpdqqjVD8ptCo=")]
        public void TestToSha256InBase64(string input, string expected)
        {
            var actual = ErrorReporter.ToSha256InBase64(input);

            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        [TestCase("v0.0.0 (unknown informational version)", "development")]
        [TestCase("4.8.0-update-notification.1+166.Branch.feature-update-notification.Sha.f000e042234cb00e7e03642d21bc0527603b267d", "development")]
        [TestCase("4.8.0-alpha.167+Branch.develop.Sha.761661a92b7780d680b1b71ac69ba2da5682d904", "alpha")]
        [TestCase("4.8.0-beta.1+0.Branch.release-4.8.0.Sha.761661a92b7780d680b1b71ac69ba2da5682d904", "beta")]
        [TestCase("4.8.0+Branch.master.Sha.8ebbb008b20ab366beedb03c53989e42c52cfb97", "production")]
        public void TestGetEnvironment(string informationalVersion, string environment)
        {
            Assert.That(ErrorReporter.GetEnvironment(informationalVersion), Is.EqualTo(environment));
        }
    }
}
