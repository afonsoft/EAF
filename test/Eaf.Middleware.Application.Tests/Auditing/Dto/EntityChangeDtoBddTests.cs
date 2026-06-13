using Eaf.Middleware.Auditing.Dto;
using Shouldly;
using System;
using Xunit;

namespace Eaf.Middleware.Application.Tests.Auditing.Dto
{
    /// <summary>
    /// Testes BDD para GetEntityChangeInput e GetEntityTypeChangeInput seguindo o padrão Dado/Quando/Então
    /// </summary>
    public class EntityChangeDtoBddTests
    {
        #region GetEntityChangeInput.Normalize

        [Fact]
        public void Dado_GetEntityChangeInput_SemSorting_Quando_Normalize_Entao_DeveDefinirPadrao()
        {
            var input = new GetEntityChangeInput
            {
                StartDate = DateTime.UtcNow.AddDays(-7),
                EndDate = DateTime.UtcNow
            };

            input.Normalize();
            input.Sorting.ShouldBe("EntityChange.ChangeTime DESC");
        }

        [Fact]
        public void Dado_GetEntityChangeInput_ComSortingUserName_Quando_Normalize_Entao_DevePrefixarUser()
        {
            var input = new GetEntityChangeInput
            {
                Sorting = "UserName ASC",
                StartDate = DateTime.UtcNow.AddDays(-7),
                EndDate = DateTime.UtcNow
            };

            input.Normalize();
            input.Sorting.ShouldBe("User.UserName ASC");
        }

        [Fact]
        public void Dado_GetEntityChangeInput_ComSortingOutro_Quando_Normalize_Entao_DevePrefixarEntityChange()
        {
            var input = new GetEntityChangeInput
            {
                Sorting = "ChangeTime DESC",
                StartDate = DateTime.UtcNow.AddDays(-7),
                EndDate = DateTime.UtcNow
            };

            input.Normalize();
            input.Sorting.ShouldBe("EntityChange.ChangeTime DESC");
        }

        [Fact]
        public void Dado_GetEntityChangeInput_Quando_DefinirPropriedades_Entao_DeveArmazenar()
        {
            var input = new GetEntityChangeInput
            {
                EntityTypeFullName = "Eaf.Middleware.Authorization.Users.User",
                UserName = "admin"
            };

            input.EntityTypeFullName.ShouldBe("Eaf.Middleware.Authorization.Users.User");
            input.UserName.ShouldBe("admin");
        }

        #endregion

        #region GetEntityTypeChangeInput.Normalize

        [Fact]
        public void Dado_GetEntityTypeChangeInput_SemSorting_Quando_Normalize_Entao_DeveDefinirPadrao()
        {
            var input = new GetEntityTypeChangeInput
            {
                EntityId = "42",
                EntityTypeFullName = "Eaf.Middleware.Authorization.Users.User"
            };

            input.Normalize();
            input.Sorting.ShouldBe("EntityChange.ChangeTime DESC");
        }

        [Fact]
        public void Dado_GetEntityTypeChangeInput_ComSortingUserName_Quando_Normalize_Entao_DevePrefixarUser()
        {
            var input = new GetEntityTypeChangeInput
            {
                EntityId = "42",
                EntityTypeFullName = "Eaf.Middleware.Authorization.Users.User",
                Sorting = "UserName DESC"
            };

            input.Normalize();
            input.Sorting.ShouldBe("User.UserName DESC");
        }

        [Fact]
        public void Dado_GetEntityTypeChangeInput_ComSortingOutro_Quando_Normalize_Entao_DevePrefixarEntityChange()
        {
            var input = new GetEntityTypeChangeInput
            {
                EntityId = "42",
                EntityTypeFullName = "Eaf.Middleware.Authorization.Users.User",
                Sorting = "ChangeTime ASC"
            };

            input.Normalize();
            input.Sorting.ShouldBe("EntityChange.ChangeTime ASC");
        }

        #endregion
    }
}
