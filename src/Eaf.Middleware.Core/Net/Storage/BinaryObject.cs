using Abp;
using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Eaf.Middleware.Storage
{
    [Table("EafBinaryObjects")]
    public class BinaryObject : CreationAuditedEntity<Guid>, IMayHaveTenant
    {
        /// <summary>
        /// BinaryObject.
        /// </summary>
        /// <returns>Resultado da operação.</returns>
        public BinaryObject()
        {
            Id = SequentialGuidGenerator.Instance.Create();
        }

        /// <summary>
        /// BinaryObject.
        /// </summary>
        /// <param name="tenantId">Parâmetro tenantId.</param>
        /// <param name="bytes">Parâmetro bytes.</param>
        /// <param name="fileType">Parâmetro fileType.</param>
        /// <param name="fileName">Parâmetro fileName.</param>
        /// <returns>Resultado da operação.</returns>
        public BinaryObject(int? tenantId, byte[] bytes, string fileType, string fileName)
            : this()
        {
            TenantId = tenantId;
            Bytes = bytes;
            FileType = fileType;
            FileName = string.Format("{0}_{1}", Id.ToString().Replace("-", ""), fileName);
        }

        [Required]
        public byte[] Bytes { get; set; }

        [Required]
        public string FileName { get; set; }

        [Required]
        public string FileType { get; set; }

        public int? TenantId { get; set; }
    }
}