using Abp.DynamicEntityProperties;
using Abp.UI.Inputs;
using Eaf.Middleware.Authorization.Users;
using Eaf.Middleware.Core.CustomInputTypes;

namespace Eaf.Middleware.Core.DynamicEntityProperties
{
    /// <summary>
    /// Representa a classe AppDynamicEntityPropertyDefinitionProvider.
    /// </summary>
    public class AppDynamicEntityPropertyDefinitionProvider : DynamicEntityPropertyDefinitionProvider
    {
        /// <summary>
        /// SetDynamicEntityProperties.
        /// </summary>
        /// <param name="context">Parâmetro context.</param>
        public override void SetDynamicEntityProperties(IDynamicEntityPropertyDefinitionContext context)
        {
            context.Manager.AddAllowedInputType<SingleLineStringInputType>();
            context.Manager.AddAllowedInputType<ComboboxInputType>();
            context.Manager.AddAllowedInputType<CheckboxInputType>();
            context.Manager.AddAllowedInputType<MultiSelectComboboxInputType>();

            //Add entities here
            context.Manager.AddEntity<User, long>();
        }
    }
}