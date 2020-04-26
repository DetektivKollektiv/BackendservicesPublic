using Amazon.Lambda.Core;

namespace DatektivKollektiv.Shared
{
    /// <summary>
    /// Extension class for the <see cref="ILambdaLogger"/>
    /// </summary>
    public static class LoggingExtensions
    {
        private const string ERROR_PATTERN = "ERROR: {0}";
        private const string WARNING_PATTERN = "WARNING: {0}";
        private const string INFO_PATTERN = "INFO: {0}";
        private const string DEBUG_PATTERN = "DEBUG: {0}";

        /// <summary>
        /// Creates a log entry prefixed with "ERROR:"
        /// </summary>
        /// <param name="logger">Instance of the <see cref="ILambdaLogger"/></param>
        /// <param name="message">The error message</param>
        public static void LogError(this ILambdaLogger logger, string message)
        {
            logger.LogLine(string.Format(ERROR_PATTERN, message));
        }

        /// <summary>
        /// Creates a log entry prefixed with "WARNING:"
        /// </summary>
        /// <param name="logger">Instance of the <see cref="ILambdaLogger"/></param>
        /// <param name="message">The warning message</param>
        public static void LogWarning(this ILambdaLogger logger, string message)
        {
            logger.LogLine(string.Format(WARNING_PATTERN, message));
        }

        /// <summary>
        /// Creates a log entry prefixed with "INFO:"
        /// </summary>
        /// <param name="logger">Instance of the <see cref="ILambdaLogger"/></param>
        /// <param name="message">The info message</param>
        public static void LogInfo(this ILambdaLogger logger, string message)
        {
            logger.LogLine(string.Format(INFO_PATTERN, message));
        }

        /// <summary>
        /// Creates a log entry prefixed with "DEBUG:"
        /// </summary>
        /// <param name="logger">Instance of the <see cref="ILambdaLogger"/></param>
        /// <param name="message">The debug message</param>
        public static void LogDebug(this ILambdaLogger logger, string message)
        {
            logger.LogLine(string.Format(DEBUG_PATTERN, message));
        }
    }
}
