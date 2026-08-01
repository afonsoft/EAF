namespace Eaf.Middleware.Authorization.Accounts.Dto
{
    /// <summary>
    /// Representa a classe RegisterOutput.
    /// </summary>
    public class RegisterOutput
    {
        /// <summary>
        /// Indica se o usuário pode logar imediatamente.
        /// </summary>
        public bool CanLogin { get; set; }

        /// <summary>
        /// Id do tenant criado ou selecionado.
        /// </summary>
        public int? TenantId { get; set; }

        /// <summary>
        /// Nome técnico do tenant criado ou selecionado.
        /// </summary>
        public string TenancyName { get; set; }
    }
}
