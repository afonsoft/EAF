using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace Eaf.Middleware.Authentication.ApiKeys
{
    /// <summary>
    /// Representa uma API key emitida para um usuário.
    /// Somente o hash SHA-256 da chave é persistido; o valor em claro é exibido uma única vez na criação.
    /// </summary>
    [Table("EafApiKeys")]
    public class ApiKey : FullAuditedEntity<long>, IMayHaveTenant
    {
        public const int MaxNameLength = 128;
        public const int MaxKeyHashLength = 64;
        public const int MaxKeyPrefixLength = 32;
        public const int MaxPermissionsLength = 4000;
        public const int MaxAllowedIpRangesLength = 2000;

        /// <summary>
        /// Tenant do dono da chave (null para usuários host).
        /// </summary>
        public int? TenantId { get; set; }

        /// <summary>
        /// Usuário dono da chave; a autenticação executa em nome desse usuário.
        /// </summary>
        public long UserId { get; set; }

        /// <summary>
        /// Nome amigável da chave (ex.: "ci-pipeline").
        /// </summary>
        [Required]
        [StringLength(MaxNameLength)]
        public string Name { get; set; }

        /// <summary>
        /// Prefixo legível da chave (primeiros caracteres) para identificação na UI.
        /// </summary>
        [Required]
        [StringLength(MaxKeyPrefixLength)]
        public string KeyPrefix { get; set; }

        /// <summary>
        /// Hash SHA-256 hexadecimal da chave completa.
        /// </summary>
        [Required]
        [StringLength(MaxKeyHashLength)]
        public string KeyHash { get; set; }

        /// <summary>
        /// Escopo de permissões da chave em JSON (array de nomes de permissão).
        /// Null/vazio = todas as permissões do usuário (quando escopo não é obrigatório).
        /// </summary>
        [StringLength(MaxPermissionsLength)]
        public string PermissionsJson { get; set; }

        /// <summary>
        /// Lista de IPs ou blocos CIDR permitidos, separados por vírgula.
        /// Null/vazio = qualquer IP.
        /// </summary>
        [StringLength(MaxAllowedIpRangesLength)]
        public string AllowedIpRanges { get; set; }

        /// <summary>
        /// Data/hora UTC de expiração (null = sem expiração).
        /// </summary>
        public DateTime? ExpirationTime { get; set; }

        /// <summary>
        /// Status da chave.
        /// </summary>
        public ApiKeyStatus Status { get; set; }

        /// <summary>
        /// Última vez (UTC) que a chave autenticou uma requisição com sucesso.
        /// </summary>
        public DateTime? LastUsedTime { get; set; }

        /// <summary>
        /// Verifica se a chave está utilizável no instante informado.
        /// </summary>
        public bool IsUsable(DateTime now)
        {
            return !IsDeleted
                && Status == ApiKeyStatus.Active
                && (!ExpirationTime.HasValue || ExpirationTime.Value > now);
        }

        /// <summary>
        /// Retorna os nomes de permissão permitidos pela chave, ou null se irrestrita.
        /// </summary>
        public string[] GetPermissionScopes()
        {
            if (string.IsNullOrWhiteSpace(PermissionsJson))
            {
                return null;
            }

            try
            {
                return System.Text.Json.JsonSerializer.Deserialize<string[]>(PermissionsJson);
            }
            catch (System.Text.Json.JsonException)
            {
                return Array.Empty<string>();
            }
        }

        /// <summary>
        /// Define o escopo de permissões (null = irrestrito).
        /// </summary>
        public void SetPermissionScopes(string[] scopes)
        {
            PermissionsJson = scopes == null || scopes.Length == 0
                ? null
                : System.Text.Json.JsonSerializer.Serialize(scopes.Distinct().OrderBy(s => s).ToArray());
        }
    }
}
