using Eaf.Middleware.Dto;
using Shouldly;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Xunit;

namespace Eaf.Middleware.Application.Tests.Dto
{
    public class FileDtoTests
    {
        [Fact]
        public void Dado_FileDto_Quando_CriadoComParametros_Entao_PropriedadesDevemSerAtribuidas()
        {
            var dto = new FileDto("report.xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");

            dto.FileName.ShouldBe("report.xlsx");
            dto.FileType.ShouldBe("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
            dto.FileToken.ShouldNotBeNullOrEmpty();
            dto.FileToken.Length.ShouldBe(32);
        }

        [Fact]
        public void Dado_FileDto_Quando_CriadoSemParametros_Entao_PropriedadesDevemSerNulas()
        {
            var dto = new FileDto();

            dto.FileName.ShouldBeNull();
            dto.FileType.ShouldBeNull();
            dto.FileToken.ShouldBeNull();
        }

        [Fact]
        public void Dado_FileDto_Quando_CriadoMultiplasVezes_Entao_FileTokensDevemSerDiferentes()
        {
            var dto1 = new FileDto("a.txt", "text/plain");
            var dto2 = new FileDto("b.txt", "text/plain");

            dto1.FileToken.ShouldNotBe(dto2.FileToken);
        }

        [Fact]
        public void Dado_FileDto_Quando_Verificado_Entao_FileNameDeveConterRequiredAttribute()
        {
            var prop = typeof(FileDto).GetProperty(nameof(FileDto.FileName));
            var attr = prop!.GetCustomAttributes(typeof(RequiredAttribute), false).FirstOrDefault();
            attr.ShouldNotBeNull();
        }

        [Fact]
        public void Dado_FileDto_Quando_Verificado_Entao_FileTokenDeveConterRequiredAttribute()
        {
            var prop = typeof(FileDto).GetProperty(nameof(FileDto.FileToken));
            var attr = prop!.GetCustomAttributes(typeof(RequiredAttribute), false).FirstOrDefault();
            attr.ShouldNotBeNull();
        }
    }
}
