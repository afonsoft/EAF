namespace Eaf.Middleware.Configuration.Host.Dto
{
    /// <summary>
    /// Representa a classe GeneralSettingsEditDto.
    /// </summary>
    public class GeneralSettingsEditDto
    {
        /// <summary>
        /// Obtém ou define Timezone.
        /// </summary>
        public string Timezone { get; set; }

        /// <summary>
        /// This value is only used for comparing user's timezone to default timezone
        /// </summary>
        public string TimezoneForComparison { get; set; }
    }
}