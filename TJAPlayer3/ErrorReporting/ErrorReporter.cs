using System;
using System.Diagnostics;
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

        public static void WithErrorReporting(Action action)
        {
            var appInformationalVersion = TJAPlayer3.AppInformationalVersion;

            using (SentrySdk.Init(o =>
            {
                o.Dsn = new Dsn("https://d13a7e78ae024f678e110c64bbf7e7f2@sentry.io/3365560");
                o.Environment = GetEnvironment(appInformationalVersion);
                o.ShutdownTimeout = TimeSpan.FromSeconds(5);
            }))
            {
                try
                {
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
                SentrySdk.CaptureException(e);
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