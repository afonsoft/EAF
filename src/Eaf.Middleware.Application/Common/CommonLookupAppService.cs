using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Collections.Extensions;
using Abp.Extensions;
using Abp.Linq.Extensions;
using Eaf.Middleware.Common.Dto;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Eaf.Middleware.Common
{
    /// <summary>
    /// Serviço de aplicação para gerenciamento de CommonLookup.
    /// </summary>
    [AbpAuthorize]
    public class CommonLookupAppService : MiddlewareAppServiceBase, ICommonLookupAppService
    {
        /// <summary>
        /// CommonLookupAppService.
        /// </summary>
        /// <returns>Resultado da operação.</returns>
        public CommonLookupAppService()
        {
        }

        [Produces("application/json", "application/json-patch+json", "text/json")]
        public async Task<PagedResultDto<NameValueDto>> FindUsers(FindUsersInput input)
        {
            if (AbpSession.TenantId != null)
            {
                //Prevent tenants to get other tenant's users.
                input.TenantId = AbpSession.TenantId;
            }

            using (CurrentUnitOfWork.SetTenantId(input.TenantId))
            {
                var query = UserManager.Users.WhereIf(
                  !input.Filter.IsNullOrWhiteSpace(),
                  u =>
                      u.Name.Contains(input.Filter, StringComparison.OrdinalIgnoreCase) ||
                      u.UserName.Contains(input.Filter, StringComparison.OrdinalIgnoreCase) ||
                      u.Surname.Contains(input.Filter, StringComparison.OrdinalIgnoreCase) ||
                      u.EmailAddress.Contains(input.Filter, StringComparison.OrdinalIgnoreCase)
              ).AsNoTracking().AsQueryable();

                var userCount = await query.CountAsync();
                var users = await query
                    .OrderBy(u => u.Name)
                    .ThenBy(u => u.Surname)
                    .PageBy(input)
                    .ToListAsync();

                return new PagedResultDto<NameValueDto>(
                    userCount,
                    users.Select(u =>
                        new NameValueDto(
                            u.FullName + " (" + u.EmailAddress + ")",
                            u.Id.ToString()
                            )
                        ).ToList()
                    );
            }
        }
    }
}