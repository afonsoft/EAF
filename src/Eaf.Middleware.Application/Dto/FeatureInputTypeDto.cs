using Abp.Runtime.Validation;
using System.Collections.Generic;

namespace Eaf.Middleware.Editions.Dto
{
    //Mapped in CustomDtoMapper
    /// <summary>
    /// Representa a classe FeatureInputTypeDto.
    /// </summary>
    public class FeatureInputTypeDto
    {
        public IDictionary<string, object> Attributes { get; set; }
        /// <summary>
        /// Obtém ou define ItemSource.
        /// </summary>
        public LocalizableComboboxItemSourceDto ItemSource { get; set; }
        /// <summary>
        /// Obtém ou define Name.
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Obtém ou define Validator.
        /// </summary>
        public IValueValidator Validator { get; set; }
    }
}