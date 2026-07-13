using Abp.Application.Services.Dto;
using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;

namespace Eaf.Middleware.Authorization.Users.Dto
{
    /// <summary>
    /// Representa a classe UserListDto.
    /// </summary>
    public class UserListDto : FullAuditedEntityDto<long>, IPassivable
    {
        /// <summary>
        /// Obtém ou define AuthenticationSource.
        /// </summary>
        public string AuthenticationSource { get; set; }
        /// <summary>
        /// Obtém ou define EmailAddress.
        /// </summary>
        public string EmailAddress { get; set; }
        /// <summary>
        /// Obtém ou define IsActive.
        /// </summary>
        public bool IsActive { get; set; }
        /// <summary>
        /// Obtém ou define IsEmailConfirmed.
        /// </summary>
        public bool IsEmailConfirmed { get; set; }
        public DateTime? LastLoginTime { get; set; }

        public DateTime LastModificationDate
        {
            get
            {
                return LastModificationTime == null ? CreationTime : LastModificationTime.Value;
            }
        }

        /// <summary>
        /// Obtém ou define Name.
        /// </summary>
        public string Name { get; set; }

        public Guid? ProfilePictureId { get; set; }
        public List<UserListRoleDto> Roles { get; set; }
        /// <summary>
        /// Obtém ou define Surname.
        /// </summary>
        public string Surname { get; set; }

        /// <summary>
        /// Obtém ou define UserName.
        /// </summary>
        public string UserName { get; set; }
    }
}