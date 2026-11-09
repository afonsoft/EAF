using Abp.Domain.Entities.Auditing;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Eaf.Middleware.Authentication.Passkeys
{
    /// <summary>
    /// Representa um passkey (credencial WebAuthn) registrado por um usuário.
    /// Contém apenas dados públicos da credencial — o desafio nunca é persistido após a cerimônia.
    /// </summary>
    [Table("EafUserPasskeys")]
    public class UserPasskey : FullAuditedEntity<long>, IMayHaveTenant
    {
        public const int MaxNameLength = 128;
        public const int MaxAttestationDataLength = 4000;
        public const int MaxTransportsLength = 256;

        /// <summary>
        /// Tenant do dono (null para usuários host).
        /// </summary>
        public int? TenantId { get; set; }

        /// <summary>
        /// Usuário dono do passkey.
        /// </summary>
        public long UserId { get; set; }

        /// <summary>
        /// Nome amigável escolhido pelo usuário (ex.: "MacBook Touch ID").
        /// </summary>
        [Required]
        [StringLength(MaxNameLength)]
        public string Name { get; set; }

        /// <summary>
        /// Credential ID emitido pelo autenticador (base64url).
        /// </summary>
        [Required]
        [StringLength(1024)]
        public string CredentialId { get; set; }

        /// <summary>
        /// Chave pública COSE serializada (base64url) — usada para verificar assinaturas.
        /// </summary>
        [Required]
        public byte[] PublicKey { get; set; }

        /// <summary>
        /// Contador de assinaturas reportado pelo autenticador (detecta clones).
        /// </summary>
        public uint SignCount { get; set; }

        /// <summary>
        /// Transports reportados no registro (usb, nfc, ble, internal, hybrid...), separados por vírgula.
        /// </summary>
        [StringLength(MaxTransportsLength)]
        public string Transports { get; set; }

        /// <summary>
        /// GUID AAGUID do autenticador (base64url ou hex; opcional, para exibição).
        /// </summary>
        [StringLength(64)]
        public string AaGuid { get; set; }

        /// <summary>
        /// Attestation Object / dados de attestation armazenados para auditoria (base64url, JSON).
        /// </summary>
        [StringLength(MaxAttestationDataLength)]
        public string AttestationData { get; set; }

        /// <summary>
        /// Se a credencial é de roaming/syncable (backup elegível) ou device-bound.
        /// </summary>
        public bool IsBackupEligible { get; set; }

        /// <summary>
        /// Última vez (UTC) que o passkey autenticou um login com sucesso.
        /// </summary>
        public DateTime? LastUsedTime { get; set; }
    }
}
