using Eaf.Middleware.Net.MimeTypes;
using Shouldly;
using Xunit;

namespace Eaf.MiddlewareCore.Tests.Net
{
    public class MimeTypeNamesTests
    {
        [Fact]
        public void Dado_ApplicationJson_Quando_Verificar_Entao_DeveSerCorreto()
        {
            MimeTypeNames.ApplicationJson.ShouldBe("application/json");
        }

        [Fact]
        public void Dado_ApplicationPdf_Quando_Verificar_Entao_DeveSerCorreto()
        {
            MimeTypeNames.ApplicationPdf.ShouldBe("application/pdf");
        }

        [Fact]
        public void Dado_ApplicationOctetStream_Quando_Verificar_Entao_DeveSerCorreto()
        {
            MimeTypeNames.ApplicationOctetStream.ShouldBe("application/octet-stream");
        }

        [Fact]
        public void Dado_ApplicationXmlDtd_Quando_Verificar_Entao_DeveSerCorreto()
        {
            MimeTypeNames.ApplicationXmlDtd.ShouldBe("application/xml-dtd");
        }

        [Fact]
        public void Dado_TextHtml_Quando_Verificar_Entao_DeveSerCorreto()
        {
            MimeTypeNames.TextHtml.ShouldBe("text/html");
        }

        [Fact]
        public void Dado_TextPlain_Quando_Verificar_Entao_DeveSerCorreto()
        {
            MimeTypeNames.TextPlain.ShouldBe("text/plain");
        }

        [Fact]
        public void Dado_TextCss_Quando_Verificar_Entao_DeveSerCorreto()
        {
            MimeTypeNames.TextCss.ShouldBe("text/css");
        }

        [Fact]
        public void Dado_ImageJpeg_Quando_Verificar_Entao_DeveSerCorreto()
        {
            MimeTypeNames.ImageJpeg.ShouldBe("image/jpeg");
        }

        [Fact]
        public void Dado_ImagePng_Quando_Verificar_Entao_DeveSerCorreto()
        {
            MimeTypeNames.ImagePng.ShouldBe("image/png");
        }

        [Fact]
        public void Dado_ImageGif_Quando_Verificar_Entao_DeveSerCorreto()
        {
            MimeTypeNames.ImageGif.ShouldBe("image/gif");
        }

        [Fact]
        public void Dado_ApplicationJavascript_Quando_Verificar_Entao_DeveSerCorreto()
        {
            MimeTypeNames.ApplicationJavascript.ShouldBe("application/javascript");
        }

        [Fact]
        public void Dado_ApplicationZip_Quando_Verificar_Entao_DeveSerCorreto()
        {
            MimeTypeNames.ApplicationZip.ShouldBe("application/zip");
        }

        [Fact]
        public void Dado_ApplicationMsword_Quando_Verificar_Entao_DeveSerCorreto()
        {
            MimeTypeNames.ApplicationMsword.ShouldBe("application/msword");
        }

        [Fact]
        public void Dado_ApplicationVndMsExcel_Quando_Verificar_Entao_DeveSerCorreto()
        {
            MimeTypeNames.ApplicationVndMsExcel.ShouldBe("application/vnd.ms-excel");
        }

        [Fact]
        public void Dado_MultipartFormData_Quando_Verificar_Entao_DeveSerCorreto()
        {
            MimeTypeNames.MultipartFormData.ShouldBe("multipart/form-data");
        }
    }
}
