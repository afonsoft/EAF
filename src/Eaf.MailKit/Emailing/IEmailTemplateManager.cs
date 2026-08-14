using System.Threading.Tasks;

namespace Eaf.MailKit.Emailing
{
    /// <summary>
    /// Gerenciador de templates de e-mail.
    /// </summary>
    public interface IEmailTemplateManager
    {
        /// <summary>
        /// Retorna o corpo do template, com fallback por tenant.
        /// </summary>
        /// <param name="name">Nome do template.</param>
        /// <param name="tenantId">Identificador do tenant (opcional).</param>
        Task<string> GetTemplateAsync(string name, int? tenantId = null);

        /// <summary>
        /// Renderiza o template aplicando os valores do modelo.
        /// </summary>
        /// <param name="name">Nome do template.</param>
        /// <param name="model">Objeto ou dicionário com os valores dos placeholders.</param>
        /// <param name="tenantId">Identificador do tenant (opcional).</param>
        Task<string> RenderAsync(string name, object model, int? tenantId = null);
    }
}
