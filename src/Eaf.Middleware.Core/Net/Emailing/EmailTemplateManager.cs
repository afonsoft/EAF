using Abp.Dependency;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.UI;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Eaf.Middleware.Net.Emailing
{
    /// <summary>
    /// Resolve templates de e-mail considerando overrides por tenant/cultura,
    /// valida placeholders e gera previews com dados de exemplo.
    /// </summary>
    public interface IEmailTemplateManager
    {
        /// <summary>
        /// Retorna o corpo do template: override (tenant, nome, cultura) →
        /// override (tenant, nome, cultura neutra) → template padrão.
        /// </summary>
        Task<string> ResolveBodyAsync(int? tenantId, string name, string culture);

        /// <summary>
        /// Valida placeholders do corpo contra a definição. Lança
        /// <see cref="UserFriendlyException"/> para placeholders desconhecidos
        /// ou obrigatórios ausentes.
        /// </summary>
        void ValidateBody(string name, string body);

        /// <summary>Renderiza o corpo substituindo placeholders por dados de exemplo.</summary>
        string RenderPreview(string name, string body);
    }

    /// <summary>
    /// Implementação de <see cref="IEmailTemplateManager"/>.
    /// </summary>
    public class EmailTemplateManager : IEmailTemplateManager, ITransientDependency
    {
        private static readonly Regex PlaceholderRegex = new Regex(@"\{([A-Z_]+)\}", RegexOptions.Compiled);

        private readonly IEmailTemplateProvider _templateProvider;
        private readonly IEmailTemplateDefinitionProvider _definitionProvider;
        private readonly IRepository<EmailTemplateOverride> _overrideRepository;
        private readonly ICurrentUnitOfWorkProvider _unitOfWorkProvider;

        /// <summary>
        /// EmailTemplateManager.
        /// </summary>
        public EmailTemplateManager(
            IEmailTemplateProvider templateProvider,
            IEmailTemplateDefinitionProvider definitionProvider,
            IRepository<EmailTemplateOverride> overrideRepository,
            ICurrentUnitOfWorkProvider unitOfWorkProvider)
        {
            _templateProvider = templateProvider;
            _definitionProvider = definitionProvider;
            _overrideRepository = overrideRepository;
            _unitOfWorkProvider = unitOfWorkProvider;
        }

        /// <inheritdoc />
        public virtual async Task<string> ResolveBodyAsync(int? tenantId, string name, string culture)
        {
            List<EmailTemplateOverride> matches;
            using (_unitOfWorkProvider.Current.DisableFilter(AbpDataFilters.MayHaveTenant))
            {
                matches = await _overrideRepository.GetAllListAsync(o => o.TenantId == tenantId && o.Name == name);
            }

            if (!string.IsNullOrEmpty(culture))
            {
                var exact = matches.FirstOrDefault(o => o.Culture == culture);
                if (exact != null)
                {
                    return exact.Body;
                }
            }

            var neutral = matches.FirstOrDefault(o => o.Culture == string.Empty);
            if (neutral != null)
            {
                return neutral.Body;
            }

            return _templateProvider.GetDefaultTemplate(tenantId);
        }

        /// <inheritdoc />
        public virtual void ValidateBody(string name, string body)
        {
            var definition = _definitionProvider.GetOrNull(name)
                ?? throw new UserFriendlyException($"Unknown email template '{name}'.");

            var used = PlaceholderRegex.Matches(body ?? string.Empty)
                .Select(m => m.Groups[1].Value)
                .ToHashSet();

            var unknown = used.Where(p => !definition.Placeholders.Contains(p)).ToList();
            if (unknown.Count > 0)
            {
                throw new UserFriendlyException(
                    $"Unknown placeholder(s): {string.Join(", ", unknown.Select(p => "{" + p + "}"))}. " +
                    $"Allowed: {string.Join(", ", definition.Placeholders.Select(p => "{" + p + "}"))}.");
            }

            var missing = definition.RequiredPlaceholders.Where(p => !used.Contains(p)).ToList();
            if (missing.Count > 0)
            {
                throw new UserFriendlyException(
                    $"Required placeholder(s) missing: {string.Join(", ", missing.Select(p => "{" + p + "}"))}.");
            }
        }

        /// <inheritdoc />
        public virtual string RenderPreview(string name, string body)
        {
            var sample = new Dictionary<string, string>
            {
                [EmailTemplateDefinitions.EmailTitle] = "Sample email title",
                [EmailTemplateDefinitions.EmailSubTitle] = "Sample email subtitle",
                [EmailTemplateDefinitions.EmailBody] = "<p>Sample email body content.</p>",
                [EmailTemplateDefinitions.ThisYear] = System.DateTime.Now.Year.ToString(),
            };

            var rendered = body ?? string.Empty;
            foreach (var pair in sample)
            {
                rendered = rendered.Replace("{" + pair.Key + "}", pair.Value);
            }

            return rendered;
        }
    }
}
