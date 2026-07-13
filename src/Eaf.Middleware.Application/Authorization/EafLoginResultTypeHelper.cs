using Abp;
using Abp.Authorization;
using Abp.Dependency;
using Abp.Domain.Services;
using Abp.UI;
using Eaf.Middleware.Localization;
using System.Globalization;

namespace Eaf.Middleware.Authorization
{
    /// <summary>
    /// Representa a classe AbpLoginResultTypeHelper.
    /// </summary>
    public class AbpLoginResultTypeHelper : DomainService, ITransientDependency
    {
        private const string LoginFailedKey = "LoginFailed";

        /// <summary>
        /// Inicializa uma nova instância da classe AbpLoginResultTypeHelper.
        /// </summary>
        public AbpLoginResultTypeHelper()
        {
            LocalizationSourceName = MiddlewareAppConsts.LocalizationSourceName;
        }

        /// <summary>
        /// Obtém a string localizada com fallback para múltiplos sources.
        /// </summary>
        protected override string L(string name)
        {
            return MiddlewareLocalizationHelper.Localize(LocalizationManager, name);
        }

        /// <summary>
        /// Obtém a string localizada com fallback para múltiplos sources com formatação.
        /// </summary>
        protected override string L(string name, params object[] args)
        {
            return MiddlewareLocalizationHelper.Localize(LocalizationManager, name, args);
        }

        /// <summary>
        /// Obtém a string localizada com fallback para múltiplos sources para uma cultura específica.
        /// </summary>
        protected override string L(string name, CultureInfo culture)
        {
            return MiddlewareLocalizationHelper.Localize(LocalizationManager, name, culture);
        }

        /// <summary>
        /// CreateExceptionForFailedLoginAttempt.
        /// </summary>
        /// <param name="result">Parâmetro result.</param>
        /// <param name="usernameOrEmailAddress">Parâmetro usernameOrEmailAddress.</param>
        /// <param name="tenancyName">Parâmetro tenancyName.</param>
        /// <returns>Resultado da operação.</returns>
        public UserFriendlyException CreateExceptionForFailedLoginAttempt(AbpLoginResultType result, string usernameOrEmailAddress, string tenancyName)
        {
            Logger.ErrorFormat("CreateExceptionForFailedLoginAttempt {0}", result);
            switch (result)
            {
                case AbpLoginResultType.Success:
                    return new UserFriendlyException("Don't call this method with a success result!");

                case AbpLoginResultType.InvalidUserNameOrEmailAddress:
                    return new UserFriendlyException(AbpLoginResultType.InvalidUserNameOrEmailAddress.GetHashCode(), L(LoginFailedKey), L("InvalidUserNameOrPassword"));

                case AbpLoginResultType.InvalidPassword:
                    return new UserFriendlyException(AbpLoginResultType.InvalidPassword.GetHashCode(), L(LoginFailedKey), L("InvalidUserNameOrPassword"));

                case AbpLoginResultType.InvalidTenancyName:
                    return new UserFriendlyException(AbpLoginResultType.InvalidTenancyName.GetHashCode(), L(LoginFailedKey), L("ThereIsNoTenantDefinedWithName{0}", tenancyName));

                case AbpLoginResultType.TenantIsNotActive:
                    return new UserFriendlyException(AbpLoginResultType.TenantIsNotActive.GetHashCode(), L(LoginFailedKey), L("TenantIsNotActive", tenancyName));

                case AbpLoginResultType.UserIsNotActive:
                    return new UserFriendlyException(AbpLoginResultType.UserIsNotActive.GetHashCode(), L(LoginFailedKey), L("UserIsNotActiveAndCanNotLogin", usernameOrEmailAddress));

                case AbpLoginResultType.UserEmailIsNotConfirmed:
                    return new UserFriendlyException(AbpLoginResultType.UserEmailIsNotConfirmed.GetHashCode(), L(LoginFailedKey), L("UserEmailIsNotConfirmedAndCanNotLogin"));

                case AbpLoginResultType.LockedOut:
                    return new UserFriendlyException(AbpLoginResultType.LockedOut.GetHashCode(), L(LoginFailedKey), L("UserLockedOutMessage"));

                default: //Can not fall to default actually. But other result types can be added in the future and we may forget to handle it
                    Logger.WarnFormat("Unhandled login fail reason: {0}", result);
                    return new UserFriendlyException(L(LoginFailedKey));
            }
        }



        /// <summary>
        /// CreateLocalizedMessageForFailedLoginAttempt.
        /// </summary>
        /// <param name="result">Parâmetro result.</param>
        /// <param name="usernameOrEmailAddress">Parâmetro usernameOrEmailAddress.</param>
        /// <param name="tenancyName">Parâmetro tenancyName.</param>
        /// <returns>Resultado da operação.</returns>
        public string CreateLocalizedMessageForFailedLoginAttempt(AbpLoginResultType result, string usernameOrEmailAddress, string tenancyName)
        {
            switch (result)
            {
                case AbpLoginResultType.Success:
                    throw new AbpException("Don't call this method with a success result!");
                case AbpLoginResultType.InvalidUserNameOrEmailAddress:
                case AbpLoginResultType.InvalidPassword:
                    return L("InvalidUserNameOrPassword");

                case AbpLoginResultType.InvalidTenancyName:
                    return L("ThereIsNoTenantDefinedWithName{0}", tenancyName);

                case AbpLoginResultType.TenantIsNotActive:
                    return L("TenantIsNotActive", tenancyName);

                case AbpLoginResultType.UserIsNotActive:
                    return L("UserIsNotActiveAndCanNotLogin", usernameOrEmailAddress);

                case AbpLoginResultType.UserEmailIsNotConfirmed:
                    return L("UserEmailIsNotConfirmedAndCanNotLogin");

                case AbpLoginResultType.LockedOut:
                    return L("UserLockedOutMessage");

                default: //Can not fall to default actually. But other result types can be added in the future and we may forget to handle it
                    Logger.WarnFormat("Unhandled login fail reason: {0}", result);
                    return L(LoginFailedKey);
            }
        }
    }
}