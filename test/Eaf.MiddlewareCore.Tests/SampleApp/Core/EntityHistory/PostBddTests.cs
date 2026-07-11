using Eaf.MiddlewareCore.SampleApp.Core.EntityHistory;
using Shouldly;
using System;
using System.Collections.Generic;
using Xunit;

namespace Eaf.MiddlewareCore.Tests.SampleApp.Core.EntityHistory
{
    public class PostBddTests
    {
        [Fact]
        public void Dado_BlogETitulo_Quando_ConstruirPost_Entao_DevePreencherPropriedades()
        {
            var blog = new Blog("Meu Blog", "https://meublog.com", "Autor");
            var post = new Post(blog, "Titulo", "Corpo");

            post.Blog.ShouldBe(blog);
            post.BlogId.ShouldBe(0);
            post.Title.ShouldBe("Titulo");
            post.Body.ShouldBe("Corpo");
            post.IsDeleted.ShouldBeFalse();
            post.TenantId.ShouldBeNull();
            post.Id.ShouldNotBe(Guid.Empty);
        }

        [Fact]
        public void Dado_ConstrutorPadrao_Quando_CriarPost_Entao_DeveGerarNovoId()
        {
            var post = new Post();

            post.Id.ShouldNotBe(Guid.Empty);
            post.Blog.ShouldBeNull();
            post.Title.ShouldBeNull();
        }

        [Fact]
        public void Dado_Post_Quando_ConfigurarTenantId_Entao_DeveManterValor()
        {
            var post = new Post();

            post.TenantId = 42;

            post.TenantId.ShouldBe(42);
        }

        [Fact]
        public void Dado_Post_Quando_MarcarComoDeletado_Entao_DeveDefinirIsDeleted()
        {
            var post = new Post();

            post.IsDeleted = true;

            post.IsDeleted.ShouldBeTrue();
        }

        [Fact]
        public void Dado_Post_Quando_ConfigurarBody_Entao_DeveManterValor()
        {
            var post = new Post { Body = "Novo corpo" };

            post.Body.ShouldBe("Novo corpo");
        }

        [Fact]
        public void Dado_Post_Quando_ConfigurarTitulo_Entao_DeveManterValor()
        {
            var post = new Post { Title = "Novo titulo" };

            post.Title.ShouldBe("Novo titulo");
        }

        [Fact]
        public void Dado_Post_Quando_ConfigurarBlogId_Entao_DeveManterValor()
        {
            var post = new Post { BlogId = 99 };

            post.BlogId.ShouldBe(99);
        }

        [Fact]
        public void Dado_ColecaoDePosts_Quando_AdicionarPost_Entao_DeveManterReferencias()
        {
            var blog = new Blog("Meu Blog", "https://meublog.com", "Autor");
            var posts = new List<Post>
            {
                new Post(blog, "Post 1", "Corpo 1"),
                new Post(blog, "Post 2", "Corpo 2")
            };

            blog.Posts = posts;

            blog.Posts.Count.ShouldBe(2);
        }
    }
}
