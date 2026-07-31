using Abp.Application.Features;
using Abp.Localization;
using Abp.UI.Inputs;
using Eaf.Middleware.Web.Features;
using NSubstitute;
using Shouldly;
using Xunit;

namespace Eaf.Middleware.Web.Core.Tests.Features
{
    /// <summary>
    /// Testes BDD para MiddlewareFeatureProvider seguindo o padrao Dado/Quando/Entao.
    /// </summary>
    public class MiddlewareFeatureProviderBddTests
    {
        #region SetFeatures

        private static IFeatureDefinitionContext CreateContextWithChatFeature(out Feature chatFeature)
        {
            chatFeature = new Feature(AppFeatures.ChatFeature, "true");
            var context = Substitute.For<IFeatureDefinitionContext>();
            context.Create(
                Arg.Any<string>(),
                Arg.Any<string>(),
                Arg.Any<ILocalizableString>(),
                Arg.Any<ILocalizableString>(),
                Arg.Any<FeatureScopes>(),
                Arg.Any<IInputType>()
            ).Returns(chatFeature);
            return context;
        }

        [Fact]
        public void Dado_FeatureProvider_Quando_SetFeatures_Entao_DeveCriarChatFeature()
        {
            // Dado
            var provider = new MiddlewareFeatureProvider();
            var context = CreateContextWithChatFeature(out _);

            // Quando
            provider.SetFeatures(context);

            // Entao
            context.Received(1).Create(
                AppFeatures.ChatFeature,
                Arg.Any<string>(),
                Arg.Any<ILocalizableString>(),
                Arg.Any<ILocalizableString>(),
                Arg.Any<FeatureScopes>(),
                Arg.Any<IInputType>()
            );
        }

        [Fact]
        public void Dado_FeatureProvider_Quando_SetFeatures_Entao_DeveCriarChatPlanFeatures()
        {
            // Dado
            var provider = new MiddlewareFeatureProvider();
            var context = CreateContextWithChatFeature(out var chatFeature);

            // Quando
            provider.SetFeatures(context);

            // Entao — 3 chat + 4 plan child features
            chatFeature.Children.Count.ShouldBe(7);
            chatFeature.Children.ShouldContain(f => f.Name == AppFeatures.TenantToHostChatFeature);
            chatFeature.Children.ShouldContain(f => f.Name == AppFeatures.GroupChatFeature);
            chatFeature.Children.ShouldContain(f => f.Name == AppFeatures.TenantToTenantChatFeature);
            chatFeature.Children.ShouldContain(f => f.Name == AppFeatures.PlanFeaturesMaxUserCount);
            chatFeature.Children.ShouldContain(f => f.Name == AppFeatures.PlanFeaturesMaxOrganizationUnitCount);
            chatFeature.Children.ShouldContain(f => f.Name == AppFeatures.PlanFeaturesApiCallLimit);
            chatFeature.Children.ShouldContain(f => f.Name == AppFeatures.PlanFeaturesStorageLimitGb);
        }

        #endregion

        #region Instanciacao

        [Fact]
        public void Dado_MiddlewareFeatureProvider_Quando_CriarInstancia_Entao_DeveSerFeatureProvider()
        {
            var provider = new MiddlewareFeatureProvider();
            provider.ShouldNotBeNull();
            provider.ShouldBeAssignableTo<FeatureProvider>();
        }

        #endregion
    }
}
