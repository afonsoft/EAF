using Abp.Application.Services.Dto;
using System;

namespace Eaf.Middleware.Auditing.Dto
{
    //### This class is mapped in CustomDtoMapper ###
    /// <summary>
    /// Representa a classe AuditLogListDto.
    /// </summary>
    public class AuditLogListDto : EntityDto<long>
    {
        /// <summary>
        /// Obtém ou define BrowserInfo.
        /// </summary>
        public string BrowserInfo { get; set; }
        /// <summary>
        /// Obtém ou define ClientIpAddress.
        /// </summary>
        public string ClientIpAddress { get; set; }
        /// <summary>
        /// Obtém ou define ClientName.
        /// </summary>
        public string ClientName { get; set; }
        /// <summary>
        /// Obtém ou define CustomData.
        /// </summary>
        public string CustomData { get; set; }
        /// <summary>
        /// Obtém ou define Exception.
        /// </summary>
        public string Exception { get; set; }
        /// <summary>
        /// Obtém ou define ExecutionDuration.
        /// </summary>
        public int ExecutionDuration { get; set; }
        /// <summary>
        /// Obtém ou define ExecutionTime.
        /// </summary>
        public DateTime ExecutionTime { get; set; }
        public int? ImpersonatorTenantId { get; set; }
        public long? ImpersonatorUserId { get; set; }
        /// <summary>
        /// Obtém ou define MethodName.
        /// </summary>
        public string MethodName { get; set; }
        /// <summary>
        /// Obtém ou define Parameters.
        /// </summary>
        public string Parameters { get; set; }
        /// <summary>
        /// Obtém ou define ServiceName.
        /// </summary>
        public string ServiceName { get; set; }
        public long? UserId { get; set; }

        /// <summary>
        /// Obtém ou define UserName.
        /// </summary>
        public string UserName { get; set; }
    }
}