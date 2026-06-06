using Abp;
using Abp.Application.Services;
using Abp.IdentityFramework;
using Abp.Runtime.Session;
using Abp.Threading;
using Eaf.Middleware.Authorization.Users;
using Eaf.Middleware.Localization;
using Eaf.Middleware.MultiTenancy;
using Microsoft.AspNetCore.Identity;
using System;
using System.Globalization;
using System.Threading.Tasks;

namespace Eaf.Middleware
{
    /// <summary>
    /// Classe base para todos os serviços de aplicação do Middleware EAF.
    /// Fornece funcionalidades comuns como acesso ao usuário atual, tenant e validação de erros.
    /// </summary>
    public abstract class MiddlewareAppServiceBase : ApplicationService
    {
        /// <summary>
        /// Inicializa uma nova instância da classe MiddlewareAppServiceBase.
        /// Configura o nome da fonte de localização padrão.
        /// </summary>
        protected MiddlewareAppServiceBase()
        {
            LocalizationSourceName = MiddlewareAppConsts.LocalizationSourceName;
        }

        /// <summary>
        /// Obtém a string localizada com fallback para múltiplos sources.
        /// </summary>
        /// <param name="name">Chave de localização</param>
        /// <returns>Texto localizado</returns>
        protected override string L(string name)
        {
            return MiddlewareLocalizationHelper.Localize(LocalizationManager, name);
        }

        /// <summary>
        /// Obtém a string localizada com fallback para múltiplos sources com formatação.
        /// </summary>
        /// <param name="name">Chave de localização</param>
        /// <param name="args">Argumentos de formatação</param>
        /// <returns>Texto localizado formatado</returns>
        protected override string L(string name, params object[] args)
        {
            return MiddlewareLocalizationHelper.Localize(LocalizationManager, name, args);
        }

        /// <summary>
        /// Obtém a string localizada com fallback para múltiplos sources para uma cultura específica.
        /// </summary>
        /// <param name="name">Chave de localização</param>
        /// <param name="culture">Cultura para localização</param>
        /// <returns>Texto localizado</returns>
        protected override string L(string name, CultureInfo culture)
        {
            return MiddlewareLocalizationHelper.Localize(LocalizationManager, name, culture);
        }

        /// <summary>
        /// Obtém ou define o gerenciador de tenants.
        /// </summary>
        public TenantManager TenantManager { get; set; }

        /// <summary>
        /// Obtém ou define o gerenciador de usuários.
        /// </summary>
        public UserManager UserManager { get; set; }

        /// <summary>
        /// Verifica e lança exceções apropriadas para erros do Identity.
        /// </summary>
        /// <param name="identityResult">Resultado da operação do Identity a ser verificado</param>
        protected virtual void CheckErrors(IdentityResult identityResult)
        {
            identityResult.CheckErrors(LocalizationManager);
        }

        /// <summary>
        /// Obtém o tenant atual de forma síncrona.
        /// </summary>
        /// <returns>Instância do tenant atual</returns>
        protected virtual Tenant GetCurrentTenant()
        {
            using (CurrentUnitOfWork.SetTenantId(null))
            {
                return TenantManager.GetById(AbpSession.GetTenantId());
            }
        }

        /// <summary>
        /// Obtém o tenant atual de forma assíncrona.
        /// </summary>
        /// <returns>Task contendo a instância do tenant atual</returns>
        protected virtual Task<Tenant> GetCurrentTenantAsync()
        {
            using (CurrentUnitOfWork.SetTenantId(null))
            {
                return TenantManager.GetByIdAsync(AbpSession.GetTenantId());
            }
        }

        /// <summary>
        /// Obtém o usuário atual de forma síncrona.
        /// </summary>
        /// <returns>Instância do usuário atual</returns>
        /// <exception cref="AbpException">Lançada quando não há usuário atual</exception>
        [Obsolete("Use GetCurrentUserAsync instead. Sync-over-async causes thread pool starvation.")]
        protected virtual User GetCurrentUser()
        {
            return AsyncHelper.RunSync(GetCurrentUserAsync);
        }

        /// <summary>
        /// Obtém o usuário atual de forma assíncrona.
        /// </summary>
        /// <returns>Task contendo a instância do usuário atual</returns>
        /// <exception cref="AbpException">Lançada quando não há usuário atual</exception>
        protected virtual async Task<User> GetCurrentUserAsync()
        {
            var user = await UserManager.FindByIdAsync(AbpSession.UserId.ToString());
            if (user == null)
            {
                throw new AbpException("There is no current user!");
            }

            return user;
        }
    }
}