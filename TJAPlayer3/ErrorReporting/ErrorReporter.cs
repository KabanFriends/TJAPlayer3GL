using System;
using Eto.Forms;

namespace TJAPlayer3.ErrorReporting
{
    public static class ErrorReporter
    {
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
                MessageBoxButtons.OK, MessageBoxType.Error);
        }
    }
}