using Abp.Application.Features;
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

        [Fact]
        public void Dado_FeatureProvider_Quando_SetFeatures_Entao_DeveCriarChatFeature()
        {
            // Dado
            var provider = new MiddlewareFeatureProvider();
            var context = Substitute.For<IFeatureDefinitionContext>();
            var chatFeature = new Feature(AppFeatures.ChatFeature, "true");
            context.Create(
                AppFeatures.ChatFeature,
                Arg.Any<string>(),
                Arg.Any<Abp.Localization.ILocalizableString>(),
                Arg.Any<Abp.Localization.ILocalizableString>(),
                Arg.Any<FeatureScopes>(),
                Arg.Any<Abp.UI.Inputs.IInputType>()
            ).Returns(chatFeature);

            // Quando
            provider.SetFeatures(context);

            // Entao
            context.Received(1).Create(
                AppFeatures.ChatFeature,
                Arg.Any<string>(),
                Arg.Any<Abp.Localization.ILocalizableString>(),
                Arg.Any<Abp.Localization.ILocalizableString>(),
                Arg.Any<FeatureScopes>(),
                Arg.Any<Abp.UI.Inputs.IInputType>()
            );
        }

        [Fact]
        public void Dado_FeatureProvider_Quando_SetFeatures_Entao_DeveCriar3ChildFeatures()
        {
            // Dado
            var provider = new MiddlewareFeatureProvider();
            var context = Substitute.For<IFeatureDefinitionContext>();
            var chatFeature = new Feature(AppFeatures.ChatFeature, "true");
            context.Create(
                Arg.Any<string>(),
                Arg.Any<string>(),
                Arg.Any<Abp.Localization.ILocalizableString>(),
                Arg.Any<Abp.Localization.ILocalizableString>(),
                Arg.Any<FeatureScopes>(),
                Arg.Any<Abp.UI.Inputs.IInputType>()
            ).Returns(chatFeature);

            // Quando
            provider.SetFeatures(context);

            // Entao — 3 child features created on chatFeature
            chatFeature.Children.Count.ShouldBe(3);
        }

        [Fact]
        public void Dado_FeatureProvider_Quando_SetFeatures_Entao_ChildFeaturesDevemIncluirTenantToHost()
        {
            // Dado
            var provider = new MiddlewareFeatureProvider();
            var context = Substitute.For<IFeatureDefinitionContext>();
            var chatFeature = new Feature(AppFeatures.ChatFeature, "true");
            context.Create(
                Arg.Any<string>(),
                Arg.Any<string>(),
                Arg.Any<Abp.Localization.ILocalizableString>(),
                Arg.Any<Abp.Localization.ILocalizableString>(),
                Arg.Any<FeatureScopes>(),
                Arg.Any<Abp.UI.Inputs.IInputType>()
            ).Returns(chatFeature);

            // Quando
            provider.SetFeatures(context);

            // Entao
            chatFeature.Children.ShouldContain(f => f.Name == AppFeatures.TenantToHostChatFeature);
            chatFeature.Children.ShouldContain(f => f.Name == AppFeatures.GroupChatFeature);
            chatFeature.Children.ShouldContain(f => f.Name == AppFeatures.TenantToTenantChatFeature);
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
