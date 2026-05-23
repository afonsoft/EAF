using Abp.Collections.Extensions;
using Abp.Runtime.Session;
using Abp.Timing.Timezone;
using Eaf.Middleware.Authorization.Users.Dto;
using Eaf.Middleware.DataExporting.Excel.EpPlus;
using Eaf.Middleware.Dto;
using Eaf.Middleware.Storage;
using System.Collections.Generic;
using System.Linq;

namespace Eaf.Middleware.Authorization.Users.Exporting
{
    /// <summary>
    /// Representa a classe UserListExcelExporter.
    /// </summary>
    public class UserListExcelExporter : EpPlusExcelExporterBase, IUserListExcelExporter
    {
        private readonly IAbpSession _AbpSession;
        private readonly ITimeZoneConverter _timeZoneConverter;

        /// <summary>
        /// UserListExcelExporter.
        /// </summary>
        /// <param name="timeZoneConverter">Parâmetro timeZoneConverter.</param>
        /// <param name="eafSession">Parâmetro eafSession.</param>
        /// <param name="tempFileCacheManager">Parâmetro tempFileCacheManager.</param>
        /// <returns>Resultado da operação.</returns>
        public UserListExcelExporter(
            ITimeZoneConverter timeZoneConverter,
            IAbpSession eafSession,
            ITempFileCacheManager tempFileCacheManager)
            : base(tempFileCacheManager)
        {
            _timeZoneConverter = timeZoneConverter;
            _AbpSession = eafSession;
        }

        /// <summary>
        /// ExportToFile.
        /// </summary>
        /// <param name="userListDtos">Parâmetro userListDtos.</param>
        /// <returns>Resultado da operação.</returns>
        public FileDto ExportToFile(List<UserListDto> userListDtos)
        {
            return CreateExcelPackage(
                "UserList.xlsx",
                excelPackage =>
                {
                    var sheet = excelPackage.Workbook.Worksheets.Add(L("Users"));
                    sheet.OutLineApplyStyle = true;

                    AddHeader(
                        sheet,
                        L("Name"),
                        L("Surname"),
                        L("UserName"),
                        L("EmailAddress"),
                        L("EmailConfirm"),
                        L("Roles"),
                        L("LastLoginTime"),
                        L("Active"),
                        L("CreationTime")
                        );

                    AddObjects(
                        sheet, 2, userListDtos,
                        _ => _.Name,
                        _ => _.Surname,
                        _ => _.UserName,
                        _ => _.EmailAddress,
                        _ => _.IsEmailConfirmed,
                        _ => _.Roles.Select(r => r.RoleName).JoinAsString(", "),
                        _ => _timeZoneConverter.Convert(_.LastLoginTime, _AbpSession.TenantId, _AbpSession.GetUserId()),
                        _ => _.IsActive,
                        _ => _timeZoneConverter.Convert(_.CreationTime, _AbpSession.TenantId, _AbpSession.GetUserId())
                        );

                    //Formatting cells

                    var lastLoginTimeColumn = sheet.Column(8);
                    lastLoginTimeColumn.Style.Numberformat.Format = "yyyy-mm-dd";

                    var creationTimeColumn = sheet.Column(10);
                    creationTimeColumn.Style.Numberformat.Format = "yyyy-mm-dd";

                    for (var i = 1; i <= 10; i++)
                    {
                        sheet.Column(i).AutoFit();
                    }
                });
        }
    }
}