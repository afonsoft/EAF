namespace Eaf.Middleware.Url
{
    /// <summary>
    /// Representa a interface IAppUrlService.
    /// </summary>
    public interface IAppUrlService
    {
        string CreateEmailActivationUrlFormat(int? tenantId);

        string CreateEmailActivationUrlFormat(string tenancyName);

        string CreatePasswordResetUrlFormat(int? tenantId);

        string CreatePasswordResetUrlFormat(string tenancyName);
    }
}