using Eaf.Middleware.Authorization.Users.Dto;
using Eaf.Middleware.Dto;
using System.Collections.Generic;

namespace Eaf.Middleware.Authorization.Users.Exporting
{
    /// <summary>
    /// Representa a interface IUserListExcelExporter.
    /// </summary>
    public interface IUserListExcelExporter
    {
        FileDto ExportToFile(List<UserListDto> userListDtos);
    }
}