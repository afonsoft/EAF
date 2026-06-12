using Eaf.Middleware.Net.MimeTypes;
using Shouldly;
using Xunit;

namespace Eaf.Middleware.Tests.Net.MimeTypes
{
    /// <summary>
    /// Testes para MimeTypeNames — valida constantes de MIME types
    /// </summary>
    public class MimeTypeNamesTests
    {
        [Fact]
        public void Dado_ApplicationJson_Quando_Verificar_Entao_DeveSerApplicationJson()
        {
            MimeTypeNames.ApplicationJson.ShouldBe("application/json");
        }

        [Fact]
        public void Dado_ApplicationXml_Quando_Verificar_Entao_DeveSerApplicationXml()
        {
            MimeTypeNames.ApplicationXml.ShouldBe("application/xml");
        }

        [Fact]
        public void Dado_ApplicationOctetStream_Quando_Verificar_Entao_DeveSerCorreto()
        {
            MimeTypeNames.ApplicationOctetStream.ShouldBe("application/octet-stream");
        }

        [Fact]
        public void Dado_ApplicationPdf_Quando_Verificar_Entao_DeveSerCorreto()
        {
            MimeTypeNames.ApplicationPdf.ShouldBe("application/pdf");
        }

        [Fact]
        public void Dado_ApplicationZip_Quando_Verificar_Entao_DeveSerCorreto()
        {
            MimeTypeNames.ApplicationZip.ShouldBe("application/zip");
        }

        [Fact]
        public void Dado_TextHtml_Quando_Verificar_Entao_DeveSerTextHtml()
        {
            MimeTypeNames.TextHtml.ShouldBe("text/html");
        }

        [Fact]
        public void Dado_TextPlain_Quando_Verificar_Entao_DeveSerTextPlain()
        {
            MimeTypeNames.TextPlain.ShouldBe("text/plain");
        }

        [Fact]
        public void Dado_TextCsv_Quando_Verificar_Entao_DeveSerTextCsv()
        {
            MimeTypeNames.TextCsv.ShouldBe("text/csv");
        }

        [Fact]
        public void Dado_TextXml_Quando_Verificar_Entao_DeveSerTextXml()
        {
            MimeTypeNames.TextXml.ShouldBe("text/xml");
        }

        [Fact]
        public void Dado_ImagePng_Quando_Verificar_Entao_DeveSerImagePng()
        {
            MimeTypeNames.ImagePng.ShouldBe("image/png");
        }

        [Fact]
        public void Dado_ImageJpeg_Quando_Verificar_Entao_DeveSerImageJpeg()
        {
            MimeTypeNames.ImageJpeg.ShouldBe("image/jpeg");
        }

        [Fact]
        public void Dado_ImageGif_Quando_Verificar_Entao_DeveSerImageGif()
        {
            MimeTypeNames.ImageGif.ShouldBe("image/gif");
        }

        [Fact]
        public void Dado_ApplicationJavascript_Quando_Verificar_Entao_DeveSerCorreto()
        {
            MimeTypeNames.ApplicationJavascript.ShouldBe("application/javascript");
        }

        [Fact]
        public void Dado_ApplicationFontWoff_Quando_Verificar_Entao_DeveSerCorreto()
        {
            MimeTypeNames.ApplicationFontWoff.ShouldBe("application/font-woff");
        }

        [Fact]
        public void Dado_ApplicationAtomXml_Quando_Verificar_Entao_DeveSerCorreto()
        {
            MimeTypeNames.ApplicationAtomXml.ShouldBe("application/atom+xml");
        }

        [Fact]
        public void Dado_ApplicationEcmascript_Quando_Verificar_Entao_DeveSerCorreto()
        {
            MimeTypeNames.ApplicationEcmascript.ShouldBe("application/ecmascript");
        }
    }
}
