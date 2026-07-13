namespace Eaf.Middleware.Core.Authentication
{
    /// <summary>
    /// Interface for external login provider settings.
    /// </summary>
    public interface IExternalLoginProviderSettings
    {
        /// <summary>
        /// Determines whether the settings are valid.
        /// </summary>
        /// <returns>true if the settings are valid; otherwise, false.</returns>
        bool IsValid();
    }
}
