using System.Threading.Tasks;

namespace Eaf.Middleware.Security.Recaptcha
{
    /// <summary>
    /// Representa a classe NullRecaptchaValidator.
    /// </summary>
    public class NullRecaptchaValidator : IRecaptchaValidator
    {
        /// <summary>
        /// Obtém ou define Instance.
        /// </summary>
        public static NullRecaptchaValidator Instance { get; } = new NullRecaptchaValidator();

        /// <summary>
        /// ValidateAsync.
        /// </summary>
        /// <param name="captchaResponse">Parâmetro captchaResponse.</param>
        public Task ValidateAsync(string captchaResponse)
        {
            return Task.CompletedTask;
        }
    }
}