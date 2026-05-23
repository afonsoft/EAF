using System;

namespace Eaf.Middleware.Url
{
    /// <summary>
    /// Representa a classe NullAppUrlService.
    /// </summary>
    public class NullAppUrlService : IAppUrlService
    {
        private NullAppUrlService()
        {
        }

        /// <summary>
        /// Obtém ou define Instance.
        /// </summary>
        public static IAppUrlService Instance { get; } = new NullAppUrlService();

        /// <summary>
        /// CreateEmailActivationUrlFormat.
        /// </summary>
        /// <param name="tenantId">Parâmetro tenantId.</param>
        /// <returns>Resultado da operação.</returns>
        public string CreateEmailActivationUrlFormat(int? tenantId)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// CreateEmailActivationUrlFormat.
        /// </summary>
        /// <param name="tenancyName">Parâmetro tenancyName.</param>
        /// <returns>Resultado da operação.</returns>
        public string CreateEmailActivationUrlFormat(string tenancyName)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// CreatePasswordResetUrlFormat.
        /// </summary>
        /// <param name="tenantId">Parâmetro tenantId.</param>
        /// <returns>Resultado da operação.</returns>
        public string CreatePasswordResetUrlFormat(int? tenantId)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// CreatePasswordResetUrlFormat.
        /// </summary>
        /// <param name="tenancyName">Parâmetro tenancyName.</param>
        /// <returns>Resultado da operação.</returns>
        public string CreatePasswordResetUrlFormat(string tenancyName)
        {
            throw new NotImplementedException();
        }
    }
}