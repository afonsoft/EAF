using Eaf.MailKit.Domain;
using System.Threading.Tasks;

namespace Eaf.MailKit.Emailing
{
    /// <summary>
    /// Repositório abstrato para leitura de templates de e-mail.
    /// </summary>
    public interface IEmailTemplateStore
    {
        /// <summary>
        /// Busca um template pelo nome, com fallback para o template do host.
        /// </summary>
        /// <param name="name">Nome do template.</param>
        /// <param name="tenantId">Identificador do tenant (opcional).</param>
        Task<EmailTemplate> FindAsync(string name, int? tenantId);
    }
}
