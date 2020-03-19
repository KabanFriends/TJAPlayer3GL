using System;
using System.Diagnostics;
using System.Net;
using System.Windows.Forms;
using Newtonsoft.Json;

namespace TJAPlayer3.Updates
{
    public static class UpdateChecker
    {
        public static void CheckForAndOfferUpdate()
        {
            try
            {
                var release = Deserialize(GetLatestReleaseJson());
                if (ShouldOfferUpdate(TJAPlayer3.AppDisplayThreePartVersion, release.TagName))
                {
                    OfferUpdate(release.Name, release.HtmlUrl);
                }
            }
            catch (Exception e)
            {
                Trace.WriteLine($"{nameof(UpdateChecker)}.{nameof(CheckForAndOfferUpdate)} encountered an exception while checking for an update: {e}");
            }
        }

        private static string GetLatestReleaseJson()
        {
            var client = new WebClientWithTimeout(TimeSpan.FromSeconds(2));
            client.Headers.Add("User-Agent", "twopointzero/TJAPlayer3");

            return client.DownloadString(
                "https://api.github.com/repos/twopointzero/tjaplayer3/releases/latest");
        }

        public static GitHubRelease Deserialize(string releaseJson)
        {
            return JsonConvert.DeserializeObject<GitHubRelease>(releaseJson);
        }

        public static bool ShouldOfferUpdate(string appDisplayThreePartVersion, string gitHubReleaseTagName)
        {
            return appDisplayThreePartVersion != gitHubReleaseTagName;
        }

        private static void OfferUpdate(string releaseName, string releaseUrl)
        {
            var messageBoxText =
                $"{releaseName} is available for download.\n\n" +
                "Would you like to open the download page in your browser?";
            var dialogResult = MessageBox.Show(
                messageBoxText,
                "A TJAPlayer3 update is available.",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);
            if (dialogResult == DialogResult.Yes)
            {
                Process.Start(releaseUrl);
            }
        }

        private sealed class WebClientWithTimeout : WebClient
        {
            private readonly TimeSpan _timeout;

            public WebClientWithTimeout(TimeSpan timeout)
            {
                _timeout = timeout;
            }

            protected override WebRequest GetWebRequest(Uri address)
            {
                var webRequest = base.GetWebRequest(address);
                webRequest.Timeout = (int) _timeout.TotalMilliseconds;
                return webRequest;
            }
        }

        public sealed class GitHubRelease
        {
            public string Name { get; set; }

            [JsonProperty("tag_name")]
            public string TagName { get; set; }

            [JsonProperty("html_url")]
            public string HtmlUrl { get; set; }
        }
    }
}
