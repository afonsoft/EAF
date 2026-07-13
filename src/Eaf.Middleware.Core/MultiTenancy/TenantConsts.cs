namespace Eaf.Middleware.MultiTenancy
{
    /// <summary>
    /// Representa a classe TenantConsts.
    /// </summary>
    public class TenantConsts // NOSONAR
    {
        public const string DefaultTenantName = "Default";
        public const int MaxNameLength = 128;
        public const string TenancyNameRegex = "^[a-zA-Z][a-zA-Z0-9_-]{1,}$";
    }
}