using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Localization;
using Eaf.Middleware.Localization.Dto;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Eaf.Middleware.Localization
{
    /// <summary>
    /// Representa a interface ILanguageAppService.
    /// </summary>
    public interface ILanguageAppService : IApplicationService
    {
        Task CreateOrUpdateLanguage(CreateOrUpdateLanguageInput input);

        Task DeleteLanguage(EntityDto input);

        Task<List<LanguageInfo>> GetAllLanguages();

        Task<GetLanguageForEditOutput> GetLanguageForEdit(NullableIdDto input);

        Task<GetLanguagesOutput> GetLanguages(GetLanguagesInput input);

        Task<PagedResultDto<LanguageTextListDto>> GetLanguageTexts(GetLanguageTextsInput input);

        Task SetDefaultLanguage(SetDefaultLanguageInput input);

        Task UpdateLanguageText(UpdateLanguageTextInput input);
    }
}