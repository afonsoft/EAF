using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Eaf.Middleware.Net.Emailing
{
    /// <summary>
    /// Override de template de e-mail por tenant e cultura. Quando existe, o corpo
    /// armazenado aqui substitui o template padrão na resolução de envio.
    /// </summary>
    [Table("EafEmailTemplateOverrides")]
    public class EmailTemplateOverride : FullAuditedEntity, IMayHaveTenant
    {
        /// <summary>Tenant dono do override; <c>null</c> para override de host.</summary>
        public virtual int? TenantId { get; set; }

        /// <summary>Nome do template (ver <see cref="EmailTemplateDefinitions"/>).</summary>
        [Required]
        [MaxLength(128)]
        public virtual string Name { get; set; }

        /// <summary>Cultura (ex.: "pt-BR"); string vazia = cultura neutra/padrão.</summary>
        [Required]
        [MaxLength(10)]
        public virtual string Culture { get; set; }

        /// <summary>Corpo HTML do template com placeholders <c>{NOME}</c>.</summary>
        [Required]
        public virtual string Body { get; set; }
    }
}
