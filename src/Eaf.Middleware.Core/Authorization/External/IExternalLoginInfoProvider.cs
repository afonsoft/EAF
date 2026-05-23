namespace Eaf.Middleware.Core.Authentication.External
{
    /// <summary>
    /// Representa a interface IExternalLoginInfoProvider.
    /// </summary>
    public interface IExternalLoginInfoProvider
    {
        string Name { get; }

        ExternalLoginProviderInfo GetExternalLoginInfo();
    }
}