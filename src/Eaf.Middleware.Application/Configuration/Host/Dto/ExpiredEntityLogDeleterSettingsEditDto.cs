namespace Eaf.Middleware.Configuration.Host.Dto
{
    /// <summary>
    /// Representa a classe ExpiredEntityLogDeleterSettingsEditDto.
    /// </summary>
    public class ExpiredEntityLogDeleterSettingsEditDto
    {
        public int? ExpiredDays { get; set; }
        public bool? Enabled { get; set; }
        public int? DeletedQuantity { get; set; }
    }
}