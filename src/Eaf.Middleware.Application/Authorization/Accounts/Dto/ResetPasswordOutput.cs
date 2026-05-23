namespace Eaf.Middleware.Authorization.Accounts.Dto
{
    /// <summary>
    /// Representa a classe ResetPasswordOutput.
    /// </summary>
    public class ResetPasswordOutput
    {
        /// <summary>
        /// Obtém ou define CanLogin.
        /// </summary>
        public bool CanLogin { get; set; }

        /// <summary>
        /// Obtém ou define UserName.
        /// </summary>
        public string UserName { get; set; }
    }
}