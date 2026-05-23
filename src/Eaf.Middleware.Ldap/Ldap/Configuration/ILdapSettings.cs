using System.Threading.Tasks;

namespace Eaf.Middleware.Ldap.Configuration
{
    /// <summary>
    /// Used to obtain current values of LDAP settings. This abstraction allows to define a
    /// different source for settings than SettingManager (see default implementation: <see cref="LdapSettings"/>).
    /// </summary>
    public interface ILdapSettings
    {
        Task<string> GetContainer(int? tenantId);

        Task<object> GetContextType(int? tenantId);

        Task<string> GetDomain(int? tenantId);

        Task<bool> GetIsEnabled(int? tenantId);

        Task<string> GetPassword(int? tenantId);

        Task<string> GetUserName(int? tenantId);
    }
}