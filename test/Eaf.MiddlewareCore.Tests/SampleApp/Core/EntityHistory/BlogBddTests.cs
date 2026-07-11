using Eaf.MiddlewareCore.SampleApp.Core.EntityHistory;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Eaf.MiddlewareCore.Tests.SampleApp.Core.EntityHistory
{
    public class BlogBddTests
    {
        [Fact]
        public void Dado_ParametrosValidos_Quando_ConstruirBlog_Entao_DevePreencherPropriedades()
        {
            var blog = new Blog("Meu Blog", "https://meublog.com", "Autor");

            blog.Name.ShouldBe("Meu Blog");
            blog.Url.ShouldBe("https://meublog.com");
            blog.More.ShouldNotBeNull();
            blog.More.BloggerName.ShouldBe("Autor");
            blog.CreationTime.ShouldBe(default);
        }

        [Theory]
        [InlineData(null, "https://url.com", "autor")]
        [InlineData("", "https://url.com", "autor")]
        [InlineData("  ", "https://url.com", "autor")]
        public void Dado_NomeNuloOuVazio_Quando_ConstruirBlog_Entao_DeveLancarArgumentNullException(string name, string url, string bloggerName)
        {
            Should.Throw<ArgumentNullException>(() => new Blog(name, url, bloggerName));
        }

        [Theory]
        [InlineData("Nome", null, "autor")]
        [InlineData("Nome", "", "autor")]
        [InlineData("Nome", "  ", "autor")]
        public void Dado_UrlNuloOuVazio_Quando_ConstruirBlog_Entao_DeveLancarArgumentNullException(string name, string url, string bloggerName)
        {
            Should.Throw<ArgumentNullException>(() => new Blog(name, url, bloggerName));
        }

        [Fact]
        public void Dado_BlogComUrl_Quando_AlterarUrl_Entao_DeveAtualizarValor()
        {
            var blog = new Blog("Meu Blog", "https://antiga.com", "Autor");

            blog.ChangeUrl("https://nova.com");

            blog.Url.ShouldBe("https://nova.com");
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("  ")]
        public void Dado_UrlNuloOuVazio_Quando_AlterarUrl_Entao_DeveLancarArgumentNullException(string newUrl)
        {
            var blog = new Blog("Meu Blog", "https://meublog.com", "Autor");
            Should.Throw<ArgumentNullException>(() => blog.ChangeUrl(newUrl));
        }

        [Fact]
        public void Dado_Blog_Quando_ConfigurarPosts_Entao_DeveManterColecao()
        {
            var blog = new Blog("Meu Blog", "https://meublog.com", "Autor");
            var posts = new List<Post> { new Post(blog, "Titulo", "Corpo") };

            blog.Posts = posts;

            blog.Posts.Count.ShouldBe(1);
            blog.Posts.First().Blog.ShouldBe(blog);
        }

        [Fact]
        public void Dado_Blog_Quando_ConfigurarPromocoes_Entao_DeveManterColecao()
        {
            var blog = new Blog("Meu Blog", "https://meublog.com", "Autor");
            var promotions = new List<BlogPromotion> { new BlogPromotion { BlogId = 1, AdvertisementId = 2, Title = "Promo" } };

            blog.Promotions = promotions;

            blog.Promotions.Count.ShouldBe(1);
            blog.Promotions.First().Title.ShouldBe("Promo");
        }

        [Fact]
        public void Dado_BlogEx_Quando_AtribuirBloggerName_Entao_DeveManterValor()
        {
            var ex = new BlogEx { BloggerName = "Novo Autor" };
            ex.BloggerName.ShouldBe("Novo Autor");
        }
    }
}
