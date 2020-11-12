using System;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using Sentry;

namespace TJAPlayer3.ErrorReporting
{
    public static class ErrorReporter
    {
        private const string EnvironmentAlpha = "alpha";
        private const string EnvironmentBeta = "beta";
        private const string EnvironmentDevelopment = "development";
        private const string EnvironmentProduction = "production";

        public const string GetCurrentSkinNameOrFallbackFallbackForExceptionEncountered = "[GetCurrentSkinNameOrNull exception encountered]";

        public static void WithErrorReporting(Action action)
        {
            var appInformationalVersion = TJAPlayer3.AppInformationalVersion;

#if DEBUG
            action();
#else
            try
            {
                action();
            }
            catch(Exception e)
            {
                NotifyUserOfError(e);
            }
#endif
        }

        private static void NotifyUserOfError(Exception exception)
        {
            var messageBoxText =
                "An error has occurred.\n" +
                "Technical information:" +
                exception;
            var dialogResult = MessageBox.Show(
                messageBoxText,
                $"{TJAPlayer3.AppDisplayNameWithThreePartVersion} Error",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        }
    }
}