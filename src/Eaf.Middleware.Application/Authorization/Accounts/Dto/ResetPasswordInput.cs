using Abp.Auditing;
using Abp.Runtime.Security;
using Abp.Runtime.Validation;
using System;
using System.Web;

namespace Eaf.Middleware.Authorization.Accounts.Dto
{
    /// <summary>
    /// Representa a classe ResetPasswordInput.
    /// </summary>
    public class ResetPasswordInput : IShouldNormalize
    {
        /// <summary>
        /// Obtém ou define AuthenticationSource.
        /// </summary>
        public string AuthenticationSource { get; set; }

        /// <summary>
        /// Encrypted values for {TenantId}, {UserId} and {ResetCode}
        /// </summary>
        public string c { get; set; }

        [DisableAuditing]
        public string Password { get; set; }

        /// <summary>
        /// Obtém ou define ResetCode.
        /// </summary>
        public string ResetCode { get; set; }
        /// <summary>
        /// Obtém ou define ReturnUrl.
        /// </summary>
        public string ReturnUrl { get; set; }
        /// <summary>
        /// Obtém ou define SingleSignIn.
        /// </summary>
        public string SingleSignIn { get; set; }
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

                if (query["resetCode"] != null)
                {
                    ResetCode = query["resetCode"];
                }

                if (query["authenticationSource"] != null)
                {
                    AuthenticationSource = query["authenticationSource"];
                }
            }
        }
    }
}