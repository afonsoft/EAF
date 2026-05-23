namespace Eaf.Middleware.Net.Emailing
{
    /// <summary>
    /// Representa a interface IEmailTemplateProvider.
    /// </summary>
    public interface IEmailTemplateProvider
    {
        string GetDefaultTemplate(int? tenantId);
    }
}