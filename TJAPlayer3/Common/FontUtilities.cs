using System;
using System.Diagnostics;
using System.Drawing;

namespace TJAPlayer3.Common
{
    internal class FontUtilities
    {
        public const string FallbackFontName = "MS UI Gothic";

        public static FontFamily GetFontFamilyOrFallback(string fontName)
        {
            if (string.IsNullOrWhiteSpace(fontName))
            {
                fontName = FallbackFontName;
            }

            try
            {
                return new FontFamily(fontName);
            }
            catch (ArgumentException e)
            {
                Trace.TraceError(e.Message);

                try
                {
                    return new FontFamily(FallbackFontName);
                }
                catch (ArgumentException fallbackException)
                {
                    throw new ArgumentException(
                        $"Japanese Language Pack, or manual {FallbackFontName} font family, installation is required.",
                        fallbackException);
                }
            }
        }
    }
}
