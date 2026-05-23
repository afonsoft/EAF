using Abp.Application.Services.Dto;
using Abp.Extensions;
using Abp.Localization;
using Abp.Runtime.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Eaf.Middleware.Localization.Dto
{
    /// <summary>
    /// Representa a classe GetLanguageTextsInput.
    /// </summary>
    public class GetLanguageTextsInput : IPagedResultRequest, ISortedResultRequest, IShouldNormalize
    {
        [StringLength(ApplicationLanguage.MaxNameLength)]
        public string BaseLanguageName { get; set; }

        /// <summary>
        /// Obtém ou define FilterText.
        /// </summary>
        public string FilterText { get; set; }

        [Range(0, int.MaxValue)]
        public int MaxResultCount { get; set; } //0: Unlimited.

        [Range(0, int.MaxValue)]
        public int SkipCount { get; set; }

        /// <summary>
        /// Obtém ou define Sorting.
        /// </summary>
        public string Sorting { get; set; }

        [Required]
        [MaxLength(ApplicationLanguageText.MaxSourceNameLength)]
        public string SourceName { get; set; }

        [Required]
        [StringLength(ApplicationLanguage.MaxNameLength, MinimumLength = 2)]
        public string TargetLanguageName { get; set; }

        /// <summary>
        /// Obtém ou define TargetValueFilter.
        /// </summary>
        public string TargetValueFilter { get; set; }

        /// <summary>
        /// Normalize.
        /// </summary>
        public void Normalize()
        {
            if (TargetValueFilter.IsNullOrEmpty())
            {
                TargetValueFilter = "ALL";
            }
        }
    }
}