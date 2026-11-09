using Abp.Application.Services.Dto;
using Eaf.Middleware.Authentication.ApiKeys;
using System;
using System.ComponentModel.DataAnnotations;

namespace Eaf.Middleware.Application.Authentication.ApiKeys.Dto
{
    /// <summary>
    /// DTO de listagem de API keys — nunca carrega material secreto.
    /// </summary>
    public class ApiKeyDto : EntityDto<long>
    {
        public string Name { get; set; }
        public string KeyPrefix { get; set; }
        public string[] PermissionScopes { get; set; }
        public string[] AllowedIpRanges { get; set; }
        public DateTime? ExpirationTime { get; set; }
        public ApiKeyStatus Status { get; set; }
        public DateTime? LastUsedTime { get; set; }
        public DateTime CreationTime { get; set; }
        public long UserId { get; set; }
        public string UserName { get; set; }
    }

    /// <summary>
    /// Input de criação de API key.
    /// </summary>
    public class CreateApiKeyInput
    {
        [Required]
        [StringLength(ApiKey.MaxNameLength)]
        public string Name { get; set; }

        /// <summary>
        /// Escopo de permissões (nomes). Vazio/null = irrestrito (quando não obrigatório por setting).
        /// </summary>
        public string[] PermissionScopes { get; set; }

        /// <summary>
        /// IPs ou blocos CIDR permitidos. Vazio/null = qualquer IP.
        /// </summary>
        public string[] AllowedIpRanges { get; set; }

        /// <summary>
        /// Expiração UTC. Null = respeita MaxLifetimeDays; para desativar, configure o limite máximo.
        /// </summary>
        public DateTime? ExpirationTime { get; set; }
    }

    /// <summary>
    /// Resultado de criação/rotação — contém a chave em claro exibida uma única vez.
    /// </summary>
    public class CreatedApiKeyDto
    {
        public ApiKeyDto ApiKey { get; set; }

        /// <summary>
        /// Chave completa em claro. Exibida uma única vez; não é persistida nem recuperável.
        /// </summary>
        public string PlainKey { get; set; }
    }
}
