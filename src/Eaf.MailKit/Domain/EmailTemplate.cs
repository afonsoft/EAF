using Abp.Domain.Entities;

namespace Eaf.MailKit.Domain
{
    /// <summary>
    /// Entidade que representa um template de e-mail.
    /// </summary>
    public class EmailTemplate : Entity<int>
    {
        /// <summary>
        /// Nome identificador do template (ex.: Welcome).
        /// </summary>
        public virtual string Name { get; set; }

        /// <summary>
        /// Assunto do e-mail.
        /// </summary>
        public virtual string Subject { get; set; }

        /// <summary>
        /// Corpo do template, com placeholders no formato {{Nome}}.
        /// </summary>
        public virtual string Body { get; set; }

        /// <summary>
        /// Identificador do tenant (null para o host).
        /// </summary>
        public virtual int? TenantId { get; set; }

        /// <summary>
        /// Indica se o template foi excluído logicamente.
        /// </summary>
        public virtual bool IsDeleted { get; set; }
    }
}
