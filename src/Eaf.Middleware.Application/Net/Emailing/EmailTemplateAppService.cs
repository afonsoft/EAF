using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Eaf.Middleware.Application.Net.Emailing.Dto;
using Eaf.Middleware.Authorization;
using Eaf.Middleware.Net.Emailing;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Eaf.Middleware.Application.Net.Emailing
{
    /// <summary>
    /// Gerenciamento de templates de e-mail: lista definições × culturas,
    /// edita overrides por tenant, preview e reset para o padrão.
    /// </summary>
    public interface IEmailTemplateAppService
    {
        /// <summary>Lista definições × culturas (override + neutra) do tenant atual.</summary>
        Task<ListResultDto<EmailTemplateListItemDto>> GetTemplates();

        /// <summary>Retorna corpo atual (override ou padrão) para edição.</summary>
        Task<EmailTemplateForEditDto> GetForEdit(EmailTemplateIdInput input);

        /// <summary>Cria ou atualiza o override do tenant atual.</summary>
        Task CreateOrUpdateOverride(CreateOrUpdateEmailTemplateInput input);

        /// <summary>Remove o override, restaurando o padrão.</summary>
        Task ResetToDefault(EmailTemplateIdInput input);

        /// <summary>Renderiza preview com dados de exemplo.</summary>
        Task<EmailTemplatePreviewDto> GetPreview(EmailTemplatePreviewInput input);
    }

    /// <summary>
    /// Implementação de <see cref="IEmailTemplateAppService"/>.
    /// </summary>
    [AbpAuthorize(MiddlewarePermissions.Pages_Administration_EmailTemplates)]
    public class EmailTemplateAppService : MiddlewareAppServiceBase, IEmailTemplateAppService
    {
        private readonly IEmailTemplateDefinitionProvider _definitionProvider;
        private readonly IEmailTemplateManager _templateManager;
        private readonly IEmailTemplateProvider _defaultProvider;
        private readonly IRepository<EmailTemplateOverride> _overrideRepository;

        /// <summary>
        /// EmailTemplateAppService.
        /// </summary>
        public EmailTemplateAppService(
            IEmailTemplateDefinitionProvider definitionProvider,
            IEmailTemplateManager templateManager,
            IEmailTemplateProvider defaultProvider,
            IRepository<EmailTemplateOverride> overrideRepository)
        {
            _definitionProvider = definitionProvider;
            _templateManager = templateManager;
            _defaultProvider = defaultProvider;
            _overrideRepository = overrideRepository;
        }

        private int? TenantId => AbpSession.TenantId;

        /// <inheritdoc />
        public async Task<ListResultDto<EmailTemplateListItemDto>> GetTemplates()
        {
            var overrides = await _overrideRepository.GetAll()
                .IgnoreQueryFilters()
                .Where(o => o.TenantId == TenantId)
                .ToListAsync();

            var items = new List<EmailTemplateListItemDto>();
            foreach (var definition in _definitionProvider.GetDefinitions())
            {
                items.Add(new EmailTemplateListItemDto
                {
                    Name = definition.Name,
                    DisplayNameLocalizationKey = definition.DisplayNameLocalizationKey,
                    Culture = string.Empty,
                    IsOverride = overrides.Any(o => o.Name == definition.Name && o.Culture == string.Empty),
                    LastModificationTime = overrides.FirstOrDefault(o => o.Name == definition.Name && o.Culture == string.Empty)?.LastModificationTime,
                });

                foreach (var o in overrides.Where(o => o.Name == definition.Name && o.Culture != string.Empty).OrderBy(o => o.Culture))
                {
                    items.Add(new EmailTemplateListItemDto
                    {
                        Name = definition.Name,
                        DisplayNameLocalizationKey = definition.DisplayNameLocalizationKey,
                        Culture = o.Culture,
                        IsOverride = true,
                        LastModificationTime = o.LastModificationTime,
                    });
                }
            }

            return new ListResultDto<EmailTemplateListItemDto>(items);
        }

        /// <inheritdoc />
        public async Task<EmailTemplateForEditDto> GetForEdit(EmailTemplateIdInput input)
        {
            var definition = _definitionProvider.GetOrNull(input.Name)
                ?? throw new Abp.UI.UserFriendlyException($"Unknown email template '{input.Name}'.");

            var culture = input.Culture ?? string.Empty;
            var existing = await FindOverride(input.Name, culture);

            return new EmailTemplateForEditDto
            {
                Name = definition.Name,
                Culture = culture,
                Body = existing?.Body ?? _defaultProvider.GetDefaultTemplate(TenantId),
                Placeholders = definition.Placeholders.ToList(),
                RequiredPlaceholders = definition.RequiredPlaceholders.ToList(),
                IsOverride = existing != null,
            };
        }

        /// <inheritdoc />
        [AbpAuthorize(MiddlewarePermissions.Pages_Administration_EmailTemplates)]
        public async Task CreateOrUpdateOverride(CreateOrUpdateEmailTemplateInput input)
        {
            _templateManager.ValidateBody(input.Name, input.Body);

            var culture = input.Culture ?? string.Empty;
            var existing = await FindOverride(input.Name, culture);
            if (existing == null)
            {
                await _overrideRepository.InsertAsync(new EmailTemplateOverride
                {
                    TenantId = TenantId,
                    Name = input.Name,
                    Culture = culture,
                    Body = input.Body,
                });
            }
            else
            {
                existing.Body = input.Body;
            }
        }

        /// <inheritdoc />
        public async Task ResetToDefault(EmailTemplateIdInput input)
        {
            var existing = await FindOverride(input.Name, input.Culture ?? string.Empty);
            if (existing != null)
            {
                await _overrideRepository.DeleteAsync(existing);
            }
        }

        /// <inheritdoc />
        public async Task<EmailTemplatePreviewDto> GetPreview(EmailTemplatePreviewInput input)
        {
            var body = input.Body;
            if (body == null)
            {
                var existing = await FindOverride(input.Name, input.Culture ?? string.Empty);
                body = existing?.Body ?? _defaultProvider.GetDefaultTemplate(TenantId);
            }
            else
            {
                _templateManager.ValidateBody(input.Name, body);
            }

            return new EmailTemplatePreviewDto { Html = _templateManager.RenderPreview(input.Name, body) };
        }

        private Task<EmailTemplateOverride> FindOverride(string name, string culture)
        {
            return _overrideRepository.GetAll()
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(o => o.TenantId == TenantId && o.Name == name && o.Culture == culture);
        }
    }
}
