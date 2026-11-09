using Abp.Dependency;
using System.Collections.Generic;
using System.Linq;

namespace Eaf.Middleware.Net.Emailing
{
    /// <summary>
    /// Definição de um template de e-mail do sistema: nome, chave de exibição
    /// e conjunto de placeholders válidos (chaves sem as chaves <c>{}</c>).
    /// </summary>
    public class EmailTemplateDefinition
    {
        /// <summary>Nome único do template.</summary>
        public string Name { get; set; }

        /// <summary>Chave de localização do nome de exibição.</summary>
        public string DisplayNameLocalizationKey { get; set; }

        /// <summary>Placeholders permitidos no corpo do template.</summary>
        public IReadOnlyList<string> Placeholders { get; set; }

        /// <summary>Placeholders que precisam estar presentes no corpo.</summary>
        public IReadOnlyList<string> RequiredPlaceholders { get; set; }
    }

    /// <summary>Nomes e placeholders dos templates conhecidos do sistema.</summary>
    public static class EmailTemplateDefinitions
    {
        /// <summary>Template de layout padrão usado por <c>UserEmailer</c>.</summary>
        public const string Default = "Default";

        /// <summary>Placeholder do título do e-mail.</summary>
        public const string EmailTitle = "EMAIL_TITLE";

        /// <summary>Placeholder do subtítulo do e-mail.</summary>
        public const string EmailSubTitle = "EMAIL_SUB_TITLE";

        /// <summary>Placeholder do corpo HTML do e-mail.</summary>
        public const string EmailBody = "EMAIL_BODY";

        /// <summary>Placeholder do ano corrente.</summary>
        public const string ThisYear = "THIS_YEAR";

        /// <summary>Definições registradas do sistema.</summary>
        public static readonly IReadOnlyList<EmailTemplateDefinition> All = new List<EmailTemplateDefinition>
        {
            new EmailTemplateDefinition
            {
                Name = Default,
                DisplayNameLocalizationKey = "EmailTemplate_Default",
                Placeholders = new[] { EmailTitle, EmailSubTitle, EmailBody, ThisYear },
                RequiredPlaceholders = new[] { EmailBody },
            },
        };
    }

    /// <summary>
    /// Fornece as definições de templates de e-mail registradas.
    /// </summary>
    public interface IEmailTemplateDefinitionProvider
    {
        /// <summary>Retorna todas as definições registradas.</summary>
        IReadOnlyList<EmailTemplateDefinition> GetDefinitions();

        /// <summary>Retorna a definição por nome ou <c>null</c>.</summary>
        EmailTemplateDefinition GetOrNull(string name);
    }

    /// <summary>
    /// Implementação padrão baseada em <see cref="EmailTemplateDefinitions.All"/>.
    /// </summary>
    public class EmailTemplateDefinitionProvider : IEmailTemplateDefinitionProvider, ISingletonDependency
    {
        /// <inheritdoc />
        public IReadOnlyList<EmailTemplateDefinition> GetDefinitions()
        {
            return EmailTemplateDefinitions.All;
        }

        /// <inheritdoc />
        public EmailTemplateDefinition GetOrNull(string name)
        {
            return EmailTemplateDefinitions.All.FirstOrDefault(d => d.Name == name);
        }
    }
}
