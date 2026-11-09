using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Eaf.Middleware.Application.Net.Emailing.Dto
{
    /// <summary>Linha da listagem: definição × cultura.</summary>
    public class EmailTemplateListItemDto
    {
        /// <summary>Nome do template.</summary>
        public string Name { get; set; }

        /// <summary>Chave de localização do nome de exibição.</summary>
        public string DisplayNameLocalizationKey { get; set; }

        /// <summary>Cultura do override ("" = neutra/padrão).</summary>
        public string Culture { get; set; }

        /// <summary>Indica se existe override salvo para a cultura.</summary>
        public bool IsOverride { get; set; }

        /// <summary>Última modificação do override, quando houver.</summary>
        public DateTime? LastModificationTime { get; set; }
    }

    /// <summary>Dados para edição de um template (override ou padrão).</summary>
    public class EmailTemplateForEditDto
    {
        /// <summary>Nome do template.</summary>
        public string Name { get; set; }

        /// <summary>Cultura alvo da edição.</summary>
        public string Culture { get; set; }

        /// <summary>Corpo atual (override ou padrão).</summary>
        public string Body { get; set; }

        /// <summary>Placeholders permitidos.</summary>
        public List<string> Placeholders { get; set; } = new List<string>();

        /// <summary>Placeholders obrigatórios.</summary>
        public List<string> RequiredPlaceholders { get; set; } = new List<string>();

        /// <summary>Indica se o corpo retornado veio de um override existente.</summary>
        public bool IsOverride { get; set; }
    }

    /// <summary>Input para criar/atualizar um override.</summary>
    public class CreateOrUpdateEmailTemplateInput
    {
        /// <summary>Nome do template.</summary>
        [Required]
        [MaxLength(128)]
        public string Name { get; set; }

        /// <summary>Cultura alvo ("" = neutra).</summary>
        [MaxLength(10)]
        public string Culture { get; set; } = string.Empty;

        /// <summary>Corpo HTML com placeholders.</summary>
        [Required]
        public string Body { get; set; }
    }

    /// <summary>Preview renderizado com dados de exemplo.</summary>
    public class EmailTemplatePreviewDto
    {
        /// <summary>HTML renderizado.</summary>
        public string Html { get; set; }
    }

    /// <summary>Input de preview: renderiza o corpo informado (ou o salvo).</summary>
    public class EmailTemplatePreviewInput : EntityDto
    {
        /// <summary>Nome do template.</summary>
        [Required]
        [MaxLength(128)]
        public string Name { get; set; }

        /// <summary>Cultura.</summary>
        [MaxLength(10)]
        public string Culture { get; set; } = string.Empty;

        /// <summary>Corpo a pré-visualizar; nulo usa o corpo atual.</summary>
        public string Body { get; set; }
    }

    /// <summary>Identifica template × cultura.</summary>
    public class EmailTemplateIdInput
    {
        /// <summary>Nome do template.</summary>
        [Required]
        [MaxLength(128)]
        public string Name { get; set; }

        /// <summary>Cultura ("" = neutra).</summary>
        [MaxLength(10)]
        public string Culture { get; set; } = string.Empty;
    }
}
