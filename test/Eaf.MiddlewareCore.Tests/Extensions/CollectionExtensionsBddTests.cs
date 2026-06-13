using Eaf.Middleware.CollectionExtensions;
using Shouldly;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Eaf.Middleware.Tests.Extensions
{
    /// <summary>
    /// Testes BDD para CollectionExtensions seguindo o padrão Dado/Quando/Então
    /// </summary>
    public class CollectionExtensionsBddTests
    {
        [Fact]
        public void Dado_ListaComElementos_Quando_RemoveAll_Entao_DeveRemoverElementosFiltrados()
        {
            // Dado
            var list = new List<int> { 1, 2, 3, 4, 5, 6 };

            // Quando
            CollectionExtensions.CollectionExtensions.RemoveAll(list, x => x % 2 == 0);

            // Então
            list.Count.ShouldBe(3);
            list.ShouldContain(1);
            list.ShouldContain(3);
            list.ShouldContain(5);
        }

        [Fact]
        public void Dado_ListaVazia_Quando_RemoveAll_Entao_DevePermanecerVazia()
        {
            // Dado
            var list = new List<string>();

            // Quando
            CollectionExtensions.CollectionExtensions.RemoveAll(list, x => x == "test");

            // Então
            list.Count.ShouldBe(0);
        }

        [Fact]
        public void Dado_ColecaoNaoList_Quando_RemoveAll_Entao_DeveRemoverElementosFiltrados()
        {
            // Dado - usando HashSet que implementa ICollection<T> mas não é List<T>
            ICollection<int> collection = new HashSet<int> { 1, 2, 3, 4, 5 };

            // Quando
            CollectionExtensions.CollectionExtensions.RemoveAll(collection, x => x > 3);

            // Então
            collection.Count.ShouldBe(3);
            collection.ShouldContain(1);
            collection.ShouldContain(2);
            collection.ShouldContain(3);
        }

        [Fact]
        public void Dado_ListaComTodos_Quando_RemoveAllTrue_Entao_DeveLimpar()
        {
            // Dado
            var list = new List<int> { 10, 20, 30 };

            // Quando
            CollectionExtensions.CollectionExtensions.RemoveAll(list, x => true);

            // Então
            list.ShouldBeEmpty();
        }

        [Fact]
        public void Dado_ListaComTodos_Quando_RemoveAllFalse_Entao_DeveManter()
        {
            // Dado
            var list = new List<string> { "a", "b", "c" };

            // Quando
            CollectionExtensions.CollectionExtensions.RemoveAll(list, x => false);

            // Então
            list.Count.ShouldBe(3);
        }
    }
}
