using System.Collections.ObjectModel;

namespace Eaf.Middleware.Editions.Dto
{
    //Mapped in CustomDtoMapper
    /// <summary>
    /// Representa a classe LocalizableComboboxItemSourceDto.
    /// </summary>
    public class LocalizableComboboxItemSourceDto
    {
        public Collection<LocalizableComboboxItemDto> Items { get; set; }
    }
}