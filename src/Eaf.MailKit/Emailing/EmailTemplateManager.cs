using Abp.Dependency;
using Abp.UI;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Eaf.MailKit.Emailing
{
    /// <summary>
    /// Implementação padrão do gerenciador de templates de e-mail.
    /// </summary>
    public class EmailTemplateManager : IEmailTemplateManager, ITransientDependency
    {
        private readonly IEmailTemplateStore _templateStore;

        /// <summary>
        /// EmailTemplateManager.
        /// </summary>
        /// <param name="templateStore">Repositório de templates.</param>
        public EmailTemplateManager(IEmailTemplateStore templateStore)
        {
            _templateStore = templateStore;
        }

        /// <summary>
        /// Retorna o corpo do template, com fallback por tenant.
        /// </summary>
        /// <param name="name">Nome do template.</param>
        /// <param name="tenantId">Identificador do tenant (opcional).</param>
        public virtual async Task<string> GetTemplateAsync(string name, int? tenantId = null)
        {
            var template = await _templateStore.FindAsync(name, tenantId);
            if (template == null)
            {
                throw new UserFriendlyException($"Template not found: {name}");
            }

            return template.Body;
        }

        /// <summary>
        /// Renderiza o template aplicando os valores do modelo.
        /// </summary>
        /// <param name="name">Nome do template.</param>
        /// <param name="model">Objeto ou dicionário com os valores dos placeholders.</param>
        /// <param name="tenantId">Identificador do tenant (opcional).</param>
        public virtual async Task<string> RenderAsync(string name, object model, int? tenantId = null)
        {
            var body = await GetTemplateAsync(name, tenantId);
            return ReplacePlaceholders(body, model);
        }

        private static string ReplacePlaceholders(string template, object model)
        {
            if (model == null)
            {
                return template;
            }

            var dictionary = model as IDictionary<string, object>;
            var genericDictionary = model as IDictionary;

            return Regex.Replace(template, @"\{\{(\w+)\}\}", match =>
            {
                var key = match.Groups[1].Value;
                string value = null;

                if (dictionary != null && dictionary.ContainsKey(key))
                {
                    value = dictionary[key]?.ToString();
                }
                else if (genericDictionary != null && genericDictionary.Contains(key))
                {
                    value = genericDictionary[key]?.ToString();
                }
                else
                {
                    var property = model.GetType().GetProperty(key, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
                    value = property?.GetValue(model)?.ToString();
                }

                return value ?? string.Empty;
            });
        }
    }
}
