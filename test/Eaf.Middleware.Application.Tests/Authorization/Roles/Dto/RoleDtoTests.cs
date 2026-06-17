using Eaf.Middleware.Authorization.Roles.Dto;
using Shouldly;
using System;
using System.Collections.Generic;
using Xunit;

namespace Eaf.Middleware.Tests.Authorization.Roles.Dto
{
    /// <summary>
    /// Testes BDD para DTOs de Role seguindo o padrão Dado/Quando/Então
    /// </summary>
    public class RoleDtoTests
    {
        #region RoleListDto

        [Fact]
        public void Dado_RoleListDto_Quando_DefinirPropriedades_Entao_DeveArmazenar()
        {
            // Dado & Quando
            var dto = new RoleListDto
            {
                DisplayName = "Administrador",
                IsDefault = false,
                IsStatic = true,
                Name = "Admin"
            };

            // Então
            dto.DisplayName.ShouldBe("Administrador");
            dto.IsDefault.ShouldBeFalse();
            dto.IsStatic.ShouldBeTrue();
            dto.Name.ShouldBe("Admin");
        }

        [Fact]
        public void Dado_RoleListDto_Quando_LastModificationTimeNull_Entao_DeveRetornarCreationTime()
        {
            // Dado
            var creationTime = new DateTime(2026, 1, 1);
            var dto = new RoleListDto
            {
                CreationTime = creationTime,
                LastModificationTime = null
            };

            // Quando & Então
            dto.LastModificationDate.ShouldBe(creationTime);
        }

        [Fact]
        public void Dado_RoleListDto_Quando_LastModificationTimePreenchido_Entao_DeveRetornarLastModificationTime()
        {
            // Dado
            var modTime = new DateTime(2026, 6, 15);
            var dto = new RoleListDto
            {
                CreationTime = new DateTime(2026, 1, 1),
                LastModificationTime = modTime
            };

            // Quando & Então
            dto.LastModificationDate.ShouldBe(modTime);
        }

        #endregion

        #region RoleEditDto

        [Fact]
        public void Dado_RoleEditDto_Quando_DefinirPropriedades_Entao_DeveArmazenar()
        {
            // Dado & Quando
            var dto = new RoleEditDto
            {
                Id = 1,
                DisplayName = "Gerente",
                IsDefault = true
            };

            // Então
            dto.Id.ShouldBe(1);
            dto.DisplayName.ShouldBe("Gerente");
            dto.IsDefault.ShouldBeTrue();
        }

        [Fact]
        public void Dado_RoleEditDto_Quando_IdNull_Entao_DeveAceitar()
        {
            // Dado & Quando
            var dto = new RoleEditDto { DisplayName = "Novo" };

            // Então
            dto.Id.ShouldBeNull();
        }

        #endregion

        #region GetRolesInput

        [Fact]
        public void Dado_GetRolesInput_Quando_Criar_Entao_FilterDeveSerVazio()
        {
            // Dado & Quando
            var input = new GetRolesInput();

            // Então
            input.Filter.ShouldBe("");
        }

        [Fact]
        public void Dado_GetRolesInput_Quando_NormalizeSemSorting_Entao_DeveDefinirComoName()
        {
            // Dado
            var input = new GetRolesInput();

            // Quando
            input.Normalize();

            // Então
            input.Sorting.ShouldBe("Name");
        }

        [Fact]
        public void Dado_GetRolesInput_Quando_NormalizeComSorting_Entao_NaoDeveAlterar()
        {
            // Dado
            var input = new GetRolesInput { Sorting = "DisplayName" };

            // Quando
            input.Normalize();

            // Então
            input.Sorting.ShouldBe("DisplayName");
        }

        #endregion

        #region CreateOrUpdateRoleInput

        [Fact]
        public void Dado_CreateOrUpdateRoleInput_Quando_DefinirPropriedades_Entao_DeveArmazenar()
        {
            // Dado & Quando
            var input = new CreateOrUpdateRoleInput
            {
                Role = new RoleEditDto { DisplayName = "Editor" },
                GrantedPermissionNames = new List<string> { "Pages", "Pages.Dashboard" }
            };

            // Então
            input.Role.DisplayName.ShouldBe("Editor");
            input.GrantedPermissionNames.Count.ShouldBe(2);
        }

        #endregion

        #region GetRoleForEditOutput

        [Fact]
        public void Dado_GetRoleForEditOutput_Quando_DefinirPropriedades_Entao_DeveArmazenar()
        {
            // Dado & Quando
            var output = new GetRoleForEditOutput
            {
                Role = new RoleEditDto { Id = 1, DisplayName = "Admin" },
                GrantedPermissionNames = new List<string> { "Pages" },
                Permissions = new List<Eaf.Middleware.Authorization.Permissions.Dto.FlatPermissionDto>()
            };

            // Então
            output.Role.DisplayName.ShouldBe("Admin");
            output.GrantedPermissionNames.Count.ShouldBe(1);
            output.Permissions.ShouldNotBeNull();
        }

        #endregion
    }
}
