using System.Collections.Generic;
using Eaf.MiddlewareCore.SampleApp.Core;
using Eaf.MiddlewareCore.SampleApp.Core.EntityHistory;
using Eaf.MiddlewareCore.SampleApp.Core.Shop;
using Shouldly;
using Xunit;

namespace Eaf.MiddlewareCore.Tests.SampleApp.Core
{
    public class SampleAppEntitiesBddTests
    {
        [Fact]
        public void Dado_UserQuando_CreateTenantAdminUser_Entao_DevePreencherDadosAdmin()
        {
            var user = User.CreateTenantAdminUser(1, "admin@example.com");

            user.ShouldNotBeNull();
            user.TenantId.ShouldBe(1);
            user.UserName.ShouldBe(User.AdminUserName);
            user.Name.ShouldBe(User.AdminUserName);
            user.Surname.ShouldBe(User.AdminUserName);
            user.EmailAddress.ShouldBe("admin@example.com");
        }

        [Fact]
        public void Dado_User_Quando_ToString_Entao_DeveRetornarFormatoCorreto()
        {
            var user = new User { Id = 5, UserName = "john" };

            var result = user.ToString();

            result.ShouldBe("[User 5] john");
        }

        [Fact]
        public void Dado_Advertisement_Quando_PreencherPropriedades_Entao_DeveManterValores()
        {
            var advertisement = new Advertisement
            {
                Banner = "banner.png",
                Feedbacks = new List<AdvertisementFeedback>
                {
                    new AdvertisementFeedback { AdvertisementId = 1, CommentId = 10 }
                }
            };

            advertisement.Banner.ShouldBe("banner.png");
            advertisement.Feedbacks.ShouldNotBeNull();
            advertisement.Feedbacks.Count.ShouldBe(1);
            advertisement.Feedbacks.ShouldContain(f => f.AdvertisementId == 1 && f.CommentId == 10);
        }

        [Fact]
        public void Dado_AdvertisementFeedback_Quando_PreencherPropriedades_Entao_DeveManterValores()
        {
            var feedback = new AdvertisementFeedback { AdvertisementId = 2, CommentId = 20 };

            feedback.AdvertisementId.ShouldBe(2);
            feedback.CommentId.ShouldBe(20);
        }

        [Fact]
        public void Dado_Country_Quando_PreencherPropriedades_Entao_DeveManterValores()
        {
            var country = new Country { CountryCode = "BR" };

            country.CountryCode.ShouldBe("BR");
        }

        [Fact]
        public void Dado_Foo_Quando_PreencherPropriedades_Entao_DeveManterValores()
        {
            var foo = new Foo { Audited = "audited-value", NonAudited = "non-audited-value" };

            foo.Audited.ShouldBe("audited-value");
            foo.NonAudited.ShouldBe("non-audited-value");
        }

        [Fact]
        public void Dado_ProductTranslation_Quando_PreencherPropriedades_Entao_DeveManterValores()
        {
            var translation = new ProductTranslation
            {
                Core = new Product { Price = 9.99m, Stock = 100 },
                CoreId = 1,
                Language = "pt-BR",
                Name = "Produto A"
            };

            translation.Core.ShouldNotBeNull();
            translation.CoreId.ShouldBe(1);
            translation.Language.ShouldBe("pt-BR");
            translation.Name.ShouldBe("Produto A");
        }

        [Fact]
        public void Dado_OrderTranslation_Quando_PreencherPropriedades_Entao_DeveManterValores()
        {
            var translation = new OrderTranslation
            {
                Core = new Order { Price = 19.99m },
                CoreId = 2,
                Language = "en-US",
                Name = "Order B"
            };

            translation.Core.ShouldNotBeNull();
            translation.CoreId.ShouldBe(2);
            translation.Language.ShouldBe("en-US");
            translation.Name.ShouldBe("Order B");
        }
    }
}
