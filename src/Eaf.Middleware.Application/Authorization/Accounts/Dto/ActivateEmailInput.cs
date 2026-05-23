using Abp.Runtime.Security;
using Abp.Runtime.Validation;
using System;
using System.Web;

namespace Eaf.Middleware.Authorization.Accounts.Dto
{
    /// <summary>
    /// Representa a classe ActivateEmailInput.
    /// </summary>
    public class ActivateEmailInput : IShouldNormalize
    {
        /// <summary>
        /// Encrypted values for {TenantId}, {UserId} and {ConfirmationCode}
        /// </summary>
        public string c { get; set; }

        /// <summary>
        /// Obtém ou define ConfirmationCode.
        /// </summary>
        public string ConfirmationCode { get; set; }
        /// <summary>
        /// Obtém ou define UserId.
        /// </summary>
        public long UserId { get; set; }

        /// <summary>
        /// Normalize.
        /// </summary>
        public void Normalize()
        {
            ResolveParameters();
        }

        protected virtual void ResolveParameters()
        {
            if (!string.IsNullOrEmpty(c))
            {
                var parameters = SimpleStringCipher.Instance.Decrypt(c);
                var query = HttpUtility.ParseQueryString(parameters);

                if (query["userId"] != null)
                {
                    UserId = Convert.ToInt32(query["userId"]);
                }

                if (query["confirmationCode"] != null)
                {
                    ConfirmationCode = query["confirmationCode"];
                }
            }
        }
    }
}