using Abp.Dependency;
using Eaf.MailKit.Domain;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace Eaf.MailKit.Emailing
{
    /// <summary>
    /// Implementação em memória do repositório de templates de e-mail.
    /// Útil para testes e para cenários onde os templates são carregados de outros sources.
    /// </summary>
    public class InMemoryEmailTemplateStore : IEmailTemplateStore, ISingletonDependency
    {
        private readonly ConcurrentDictionary<string, EmailTemplate> _templates = new();

        /// <summary>
        /// Adiciona ou substitui um template na memória.
        /// </summary>
        /// <param name="template">Template a ser armazenado.</param>
        public void Add(EmailTemplate template)
        {
            var key = BuildKey(template.TenantId, template.Name);
            _templates[key] = template;
        }

        /// <summary>
        /// Busca um template pelo nome, com fallback para o host.
        /// </summary>
        /// <param name="name">Nome do template.</param>
        /// <param name="tenantId">Identificador do tenant (opcional).</param>
        public Task<EmailTemplate> FindAsync(string name, int? tenantId)
        {
            var key = BuildKey(tenantId, name);
            if (_templates.TryGetValue(key, out var template))
            {
                return Task.FromResult(template);
            }

            if (tenantId.HasValue)
            {
                var hostKey = BuildKey(null, name);
                if (_templates.TryGetValue(hostKey, out template))
                {
                    return Task.FromResult(template);
                }
            }

            return Task.FromResult<EmailTemplate>(null);
        }

        private static string BuildKey(int? tenantId, string name)
        {
            return $"{tenantId?.ToString() ?? "host"}:{name}";
        }
    }
}
