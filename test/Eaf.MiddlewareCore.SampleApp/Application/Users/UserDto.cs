using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using Eaf.MiddlewareCore.SampleApp.Core;

namespace Eaf.MiddlewareCore.SampleApp.Application.Users
{
    [AutoMap(typeof(User))]
    public class UserDto : EntityDto<long>
    {
        public string UserName { get; set; }
    }
}