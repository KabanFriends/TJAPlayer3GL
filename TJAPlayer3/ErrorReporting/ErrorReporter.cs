using System;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using Sentry;
using Sentry.Protocol;

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
            using (SentrySdk.Init(o =>
            {
                try
                {
                    o.Dsn = new Dsn("https://d13a7e78ae024f678e110c64bbf7e7f2@sentry.io/3365560");
                }
                catch
                {
                    // ignored
                }

                try
                {
                    o.Environment = GetEnvironment(TJAPlayer3.AppInformationalVersion);
                }
                catch
                {
                    // ignored
                }

                try
                {
                    o.ServerName = ToSha256InBase64(Environment.MachineName);
                }
                catch
                {
                    // ignored
                }

                try
                {
                    o.ShutdownTimeout = TimeSpan.FromSeconds(5);
                }
                catch
                {
                    // ignored
                }
            }))
            {
                try
                {
                    try
                    {
                        SentrySdk.ConfigureScope(scope =>
                        {
                            scope.User.Username = ToSha256InBase64(Environment.UserName);
                        });
                    }
                    catch
                    {
                        // ignored
                    }

                    Application.ThreadException += (sender, args) =>
                    {
                        ReportError(args.Exception);
                    };

                    TaskScheduler.UnobservedTaskException += (sender, args) =>
                    {
                        ReportError(args.Exception);
                    };

                    action();
                }
                catch (Exception e)
                {
                    ReportError(e);

                    var task = SentrySdk.FlushAsync(TimeSpan.FromSeconds(5));

                    try
                    {
                        NotifyUserOfError(e);
                    }
                    catch
                    {
                        // ignored
                    }

                    task.Wait();
                }
            }
        }

        public static string ToSha256InBase64(string value)
        {
            using (var sha256 = SHA256.Create())
            {
                var utf8Bytes = Encoding.UTF8.GetBytes(value);
                var sha256Bytes = sha256.ComputeHash(utf8Bytes);
                return Convert.ToBase64String(sha256Bytes);
            }
        }

        public static string GetEnvironment(string informationalVersion)
        {
            switch (Regex.Match(informationalVersion, @"(?<=^.+?[+-])\w+").Value)
            {
                case "Branch":
                {
                    return EnvironmentProduction;
                }
                case "beta":
                {
                    return EnvironmentBeta;
                }
                case "alpha":
                {
                    return EnvironmentAlpha;
                }
                default:
                {
                    return EnvironmentDevelopment;
                }
            }
        }

        private static void ReportError(Exception e)
        {
            Trace.WriteLine("");
            Trace.WriteLine(e);
            Trace.WriteLine("");
            Trace.WriteLine("エラーだゴメン！（涙");

#if !DEBUG
            try
            {
                SentrySdk.WithScope(scope =>
                {
                    void SetSkinNameTag(string skinName, string scopeTagKindPart)
                    {
                        if (skinName != null)
                        {
                            scope.SetTag($"skin.{scopeTagKindPart}.name", ToSha256InBase64(skinName));
                        }
                    }

                    var boxDefSkinNameOrFallback = GetCurrentSkinNameOrFallback("box.def", CSkin.GetCurrentBoxDefSkinName);
                    var systemSkinNameOrFallback = GetCurrentSkinNameOrFallback("system", CSkin.GetCurrentSystemSkinName);

                    SetSkinNameTag(boxDefSkinNameOrFallback, "boxdef");
                    SetSkinNameTag(systemSkinNameOrFallback, "system");

                    var level = ShouldCaptureAtErrorLevel(e, boxDefSkinNameOrFallback, systemSkinNameOrFallback)
                        ? SentryLevel.Error
                        : SentryLevel.Warning;

                    SentrySdk.CaptureEvent(new SentryEvent(e) {Level = level});
                });
            }
            catch (TimeoutException)
            {
                Trace.WriteLine("Timeout encountered when attempting to report an error to Sentry");
            }
            catch (Exception exception)
            {
                Trace.WriteLine("Unexpected exception encountered when attempting to report an error: " + exception);
            }
#endif
        }

        private static string GetCurrentSkinNameOrFallback(string kind, Func<string> func)
        {
            try
            {
                return func();
            }
            catch (Exception e)
            {
                Trace.WriteLine($"Unexpected exception encountered when attempting to get the current {kind} skin name: " + e);

                return GetCurrentSkinNameOrFallbackFallbackForExceptionEncountered;
            }
        }

        private static bool ShouldCaptureAtErrorLevel(Exception e, string boxDefSkinNameOrFallback, string systemSkinNameOrFallback)
        {
            // The goal is to reduce noise from custom skin users as they are prone to frequently encountering specific exception types.

            // If the error is of any other type then report it.
            if (!(e is SlimDX.Direct3D9.Direct3D9Exception || e is AccessViolationException || e is OutOfMemoryException))
            {
                return true;
            }

            // Otherwise only report that error type if the user is running SimpleStyle,
            // if an error occurred when trying to determine their skin,
            // or if the system skin name is as-yet unknown.

            bool IsSimpleStyleOrFallbackForExceptionEncountered(string value) =>
                value == "SimpleStyle" || value == GetCurrentSkinNameOrFallbackFallbackForExceptionEncountered;

            return IsSimpleStyleOrFallbackForExceptionEncountered(boxDefSkinNameOrFallback) ||
                   IsSimpleStyleOrFallbackForExceptionEncountered(systemSkinNameOrFallback) ||
                   string.IsNullOrWhiteSpace(systemSkinNameOrFallback);
        }

        private static void NotifyUserOfError(Exception exception)
        {
            var messageBoxText =
                "An error has occurred and was automatically reported.\n\n" +
                "If you wish, you can provide additional information, look for similar issues, etc. by visiting our GitHub Issues page.\n\n" +
                "Would you like the error details copied to the clipboard and your browser opened?\n\n" +
                exception;
            var dialogResult = MessageBox.Show(
                messageBoxText,
                $"{TJAPlayer3.AppDisplayNameWithThreePartVersion} Error",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Error);
            if (dialogResult == DialogResult.Yes)
            {
                Clipboard.SetText(exception.ToString());
                Process.Start("https://github.com/twopointzero/TJAPlayer3/issues");
            }
        }
    }
}