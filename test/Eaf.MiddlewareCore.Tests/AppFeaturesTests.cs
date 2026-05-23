using Eaf.Middleware;
using Shouldly;
using Xunit;

namespace Eaf.Middleware.Tests
{
    public class AppFeaturesTests
    {
        [Fact]
        public void ChatFeature_ShouldNotBeEmpty()
        {
            // Act
            var chatFeature = AppFeatures.ChatFeature;

            // Assert
            chatFeature.ShouldNotBeNull();
            chatFeature.ShouldNotBeEmpty();
        }

        [Fact]
        public void ChatFeature_ShouldBeAppChatFeature()
        {
            // Act
            var chatFeature = AppFeatures.ChatFeature;

            // Assert
            chatFeature.ShouldBe("App.ChatFeature");
        }

        [Fact]
        public void TenantToHostChatFeature_ShouldNotBeEmpty()
        {
            // Act
            var tenantToHostChatFeature = AppFeatures.TenantToHostChatFeature;

            // Assert
            tenantToHostChatFeature.ShouldNotBeNull();
            tenantToHostChatFeature.ShouldNotBeEmpty();
        }

        [Fact]
        public void TenantToHostChatFeature_ShouldBeAppChatFeatureTenantToHost()
        {
            // Act
            var tenantToHostChatFeature = AppFeatures.TenantToHostChatFeature;

            // Assert
            tenantToHostChatFeature.ShouldBe("App.ChatFeature.TenantToHost");
        }

        [Fact]
        public void TenantToTenantChatFeature_ShouldNotBeEmpty()
        {
            // Act
            var tenantToTenantChatFeature = AppFeatures.TenantToTenantChatFeature;

            // Assert
            tenantToTenantChatFeature.ShouldNotBeNull();
            tenantToTenantChatFeature.ShouldNotBeEmpty();
        }

        [Fact]
        public void TenantToTenantChatFeature_ShouldBeAppChatFeatureTenantToTenant()
        {
            // Act
            var tenantToTenantChatFeature = AppFeatures.TenantToTenantChatFeature;

            // Assert
            tenantToTenantChatFeature.ShouldBe("App.ChatFeature.TenantToTenant");
        }

        [Fact]
        public void GroupChatFeature_ShouldNotBeEmpty()
        {
            // Act
            var groupChatFeature = AppFeatures.GroupChatFeature;

            // Assert
            groupChatFeature.ShouldNotBeNull();
            groupChatFeature.ShouldNotBeEmpty();
        }

        [Fact]
        public void GroupChatFeature_ShouldBeAppChatFeatureGroupChat()
        {
            // Act
            var groupChatFeature = AppFeatures.GroupChatFeature;

            // Assert
            groupChatFeature.ShouldBe("App.ChatFeature.GroupChat");
        }

        [Fact]
        public void AllFeatures_ShouldStartWithAppChatFeature()
        {
            // Act
            var chatFeature = AppFeatures.ChatFeature;
            var tenantToHost = AppFeatures.TenantToHostChatFeature;
            var tenantToTenant = AppFeatures.TenantToTenantChatFeature;
            var groupChat = AppFeatures.GroupChatFeature;

            // Assert
            tenantToHost.ShouldStartWith(chatFeature);
            tenantToTenant.ShouldStartWith(chatFeature);
            groupChat.ShouldStartWith(chatFeature);
        }

        [Fact]
        public void AppFeatures_ShouldBeStaticClass()
        {
            // Arrange & Act
            var type = typeof(AppFeatures);

            // Assert
            type.ShouldNotBeNull();
            type.IsAbstract.ShouldBeTrue();
            type.IsSealed.ShouldBeTrue();
        }
    }
}
