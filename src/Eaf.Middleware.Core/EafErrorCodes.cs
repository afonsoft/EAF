namespace Eaf.Middleware
{
    /// <summary>
    /// Stable, machine-readable error codes used across EAF public error responses.
    /// </summary>
    public static class EafErrorCodes
    {
        /// <summary>
        /// The caller is not authenticated.
        /// </summary>
        public const string NotAuthenticated = "not_authenticated";

        /// <summary>
        /// The caller is authenticated but lacks the required permission.
        /// </summary>
        public const string NotAuthorized = "not_authorized";

        /// <summary>
        /// The request failed validation or business rules.
        /// </summary>
        public const string ValidationFailed = "validation_failed";

        /// <summary>
        /// The requested operation could not be completed and may be retried later.
        /// </summary>
        public const string TemporarilyUnavailable = "temporarily_unavailable";

        /// <summary>
        /// The request was rate limited.
        /// </summary>
        public const string RateLimited = "rate_limited";
    }
}