using Shouldly;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Xunit;

namespace Eaf.MiddlewareCore.Tests.Extensions
{
    public class CollectionExtensionsTests
    {
        [Fact]
        public void Dado_ListaComItens_Quando_RemoveAllComPredicado_Entao_DeveRemoverItensFiltrados()
        {
            // Dado
            var list = new List<int> { 1, 2, 3, 4, 5, 6 };

            // Quando
            Eaf.Middleware.CollectionExtensions.CollectionExtensions.RemoveAll(list, x => x % 2 == 0);

            // Então
            list.Count.ShouldBe(3);
            list.ShouldContain(1);
            list.ShouldContain(3);
            list.ShouldContain(5);
        }

        [Fact]
        public void Dado_ListaVazia_Quando_RemoveAll_Entao_DeveManterVazia()
        {
            var list = new List<int>();
            Eaf.Middleware.CollectionExtensions.CollectionExtensions.RemoveAll(list, x => x > 0);
            list.Count.ShouldBe(0);
        }

        [Fact]
        public void Dado_ListaSemMatchNoPredicado_Quando_RemoveAll_Entao_DeveManterTodosItens()
        {
            var list = new List<int> { 1, 3, 5, 7 };
            Eaf.Middleware.CollectionExtensions.CollectionExtensions.RemoveAll(list, x => x % 2 == 0);
            list.Count.ShouldBe(4);
        }

        [Fact]
        public void Dado_CollectionNaoList_Quando_RemoveAll_Entao_DeveRemoverItensFiltrados()
        {
            // Dado - Collection que não é List<T>
            ICollection<int> collection = new Collection<int> { 1, 2, 3, 4, 5, 6 };

            // Quando
            Eaf.Middleware.CollectionExtensions.CollectionExtensions.RemoveAll(collection, x => x % 2 == 0);

            // Então
            collection.Count.ShouldBe(3);
            collection.ShouldContain(1);
            collection.ShouldContain(3);
            collection.ShouldContain(5);
        }

        [Fact]
        public void Dado_CollectionNaoList_Quando_RemoveAllSemMatch_Entao_DeveManterTodos()
        {
            ICollection<string> collection = new Collection<string> { "a", "b", "c" };
            Eaf.Middleware.CollectionExtensions.CollectionExtensions.RemoveAll(collection, x => x == "z");
            collection.Count.ShouldBe(3);
        }

        [Fact]
        public void Dado_ListaComTodosMatchados_Quando_RemoveAll_Entao_DeveRemoverTodos()
        {
            var list = new List<int> { 2, 4, 6, 8 };
            Eaf.Middleware.CollectionExtensions.CollectionExtensions.RemoveAll(list, x => x % 2 == 0);
            list.Count.ShouldBe(0);
        }
    }
}
