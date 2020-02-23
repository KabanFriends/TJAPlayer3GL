using System;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using Rollbar;

namespace TJAPlayer3.ErrorReporting
{
    public static class ErrorReporter
    {
        private const string EnvironmentAlpha = "alpha";
        private const string EnvironmentBeta = "beta";
        private const string EnvironmentDevelopment = "development";
        private const string EnvironmentProduction = "production";

        public static void WithErrorReporting(Action action)
        {
            try
            {
                var appInformationalVersion = TJAPlayer3.AppInformationalVersion;

                var codeVersion = GetShaFromInformationalVersion(appInformationalVersion);

                var rollbarConfig = new RollbarConfig("a4c98d82d6534bdab3fd9583029314e0")
                {
                    CaptureUncaughtExceptions = true,
                    Environment = GetEnvironment(appInformationalVersion),
                    RethrowExceptionsAfterReporting = false,
                    Transform = payload =>
                    {
                        if (codeVersion != null)
                        {
                            payload.Data.CodeVersion = codeVersion;
                        }
                        payload.Data.Platform = "client";
                    }
                };
                RollbarLocator.RollbarInstance.Configure(rollbarConfig);

                Application.ThreadException += (sender, args) =>
                {
                    ReportError(args.Exception);
                };

                AppDomain.CurrentDomain.UnhandledException += (sender, args) =>
                {
                    ReportError(args.ExceptionObject as Exception);
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
                RollbarLocator.RollbarInstance.AsBlockingLogger(TimeSpan.FromSeconds(5)).Error(e);
            }
            catch (TimeoutException)
            {
                Trace.WriteLine("Timeout encountered when attempting to report an error to Rollbar");
            }
            catch (Exception exception)
            {
                Trace.WriteLine("Unexpected exception encountered when attempting to report an error: " + exception);
            }
#endif
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

        public static string GetShaFromInformationalVersion(string informationalVersion)
        {
            var match = Regex.Match(informationalVersion, @"(?<=\.)[0-9a-f]{40}$");
            return match.Value == "" ? null : match.Value;
        }
    }
}