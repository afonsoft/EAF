namespace Eaf.Middleware.Security
{
    /// <summary>
    /// Representa a classe PasswordComplexitySetting.
    /// </summary>
    public class PasswordComplexitySetting
    {
        /// <summary>
        /// Obtém ou define RequireDigit.
        /// </summary>
        public bool RequireDigit { get; set; }

        /// <summary>
        /// Obtém ou define RequiredLength.
        /// </summary>
        public int RequiredLength { get; set; }

        /// <summary>
        /// Obtém ou define RequireLowercase.
        /// </summary>
        public bool RequireLowercase { get; set; }

        /// <summary>
        /// Obtém ou define RequireNonAlphanumeric.
        /// </summary>
        public bool RequireNonAlphanumeric { get; set; }

        /// <summary>
        /// Obtém ou define RequireUppercase.
        /// </summary>
        public bool RequireUppercase { get; set; }

        /// <summary>
        /// Equals.
        /// </summary>
        /// <param name="other">Parâmetro other.</param>
        /// <returns>Resultado da operação.</returns>
        public bool Equals(PasswordComplexitySetting other)
        {
            if (other == null)
            {
                return false;
            }

            return
                RequireDigit == other.RequireDigit &&
                RequireLowercase == other.RequireLowercase &&
                RequireNonAlphanumeric == other.RequireNonAlphanumeric &&
                RequireUppercase == other.RequireUppercase &&
                RequiredLength == other.RequiredLength;
        }
    }
}