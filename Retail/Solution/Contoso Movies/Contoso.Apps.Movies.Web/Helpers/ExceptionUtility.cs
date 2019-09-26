using System;

namespace Contoso.Apps.Movies.Web.Helpers
{
    // Create our own utility for exceptions.
    public sealed class ExceptionUtility
    {
        // All methods are static, so this can be private
        private ExceptionUtility()
        { }

        // Log an Exception to Application Insights.
        public static void LogException(Exception exc, string source)
        {
            TelemetryHelper.TrackException(exc);
        }

    }
}