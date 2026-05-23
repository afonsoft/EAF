using System.Threading.Tasks;

namespace Eaf.Middleware.Security.Recaptcha
{
    /// <summary>
    /// Representa a interface IRecaptchaValidator.
    /// </summary>
    public interface IRecaptchaValidator
    {
        Task ValidateAsync(string captchaResponse);
    }
}