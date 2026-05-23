using Abp.Dependency;
using System.Threading.Tasks;

namespace Eaf.Middleware.UiCustomization
{
    /// <summary>
    /// Representa a interface IUiThemeCustomizerFactory.
    /// </summary>
    public interface IUiThemeCustomizerFactory : ISingletonDependency
    {
        Task<IUiCustomizer> GetCurrentUiCustomizer();

        IUiCustomizer GetUiCustomizer(string theme);
    }
}