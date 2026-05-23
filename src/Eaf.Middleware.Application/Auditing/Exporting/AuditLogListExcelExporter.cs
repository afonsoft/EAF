using Abp.Extensions;
using Abp.Runtime.Session;
using Abp.Timing.Timezone;
using Eaf.Middleware.Auditing.Dto;
using Eaf.Middleware.DataExporting.Excel.EpPlus;
using Eaf.Middleware.Dto;
using Eaf.Middleware.Storage;
using System.Collections.Generic;

namespace Eaf.Middleware.Auditing.Exporting
{
    /// <summary>
    /// Representa a classe AuditLogListExcelExporter.
    /// </summary>
    public class AuditLogListExcelExporter : EpPlusExcelExporterBase, IAuditLogListExcelExporter
    {
        private readonly IAbpSession _AbpSession;
        private readonly ITimeZoneConverter _timeZoneConverter;

        /// <summary>
        /// AuditLogListExcelExporter.
        /// </summary>
        /// <param name="timeZoneConverter">Parâmetro timeZoneConverter.</param>
        /// <param name="eafSession">Parâmetro eafSession.</param>
        /// <param name="tempFileCacheManager">Parâmetro tempFileCacheManager.</param>
        /// <returns>Resultado da operação.</returns>
        public AuditLogListExcelExporter(
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
        /// <param name="auditLogListDtos">Parâmetro auditLogListDtos.</param>
        /// <returns>Resultado da operação.</returns>
        public FileDto ExportToFile(List<AuditLogListDto> auditLogListDtos)
        {
            return CreateExcelPackage(
                "AuditLogs.xlsx",
                excelPackage =>
                {
                    var sheet = excelPackage.Workbook.Worksheets.Add(L("AuditLogs"));
                    sheet.OutLineApplyStyle = true;

                    AddHeader(
                        sheet,
                        L("Time"),
                        L("UserName"),
                        L("Service"),
                        L("Action"),
                        L("Parameters"),
                        L("Duration"),
                        L("IpAddress"),
                        L("Client"),
                        L("Browser"),
                        L("ErrorState")
                    );

                    AddObjects(
                        sheet, 2, auditLogListDtos,
                        _ => _timeZoneConverter.Convert(_.ExecutionTime, _AbpSession.TenantId, _AbpSession.GetUserId()),
                        _ => _.UserName,
                        _ => _.ServiceName,
                        _ => _.MethodName,
                        _ => _.Parameters,
                        _ => _.ExecutionDuration,
                        _ => _.ClientIpAddress,
                        _ => _.ClientName,
                        _ => _.BrowserInfo,
                        _ => _.Exception.IsNullOrEmpty() ? L("Success") : _.Exception
                        );

                    //Formatting cells

                    var timeColumn = sheet.Column(1);
                    timeColumn.Style.Numberformat.Format = "yyyy-mm-dd hh:mm:ss";

                    for (var i = 1; i <= 10; i++)
                    {
                        if (i.IsIn(5, 10)) //Don't AutoFit Parameters and Exception
                        {
                            continue;
                        }

                        sheet.Column(i).AutoFit();
                    }
                });
        }

        /// <summary>
        /// ExportToFile.
        /// </summary>
        /// <param name="entityChangeListDtos">Parâmetro entityChangeListDtos.</param>
        /// <returns>Resultado da operação.</returns>
        public FileDto ExportToFile(List<EntityChangeListDto> entityChangeListDtos)
        {
            return CreateExcelPackage(
                "DetailedLogs.xlsx",
                excelPackage =>
                {
                    var sheet = excelPackage.Workbook.Worksheets.Add(L("DetailedLogs"));
                    sheet.OutLineApplyStyle = true;

                    AddHeader(
                        sheet,
                        L("Action"),
                        L("Object"),
                        L("UserName"),
                        L("Time")
                    );

                    AddObjects(
                        sheet, 2, entityChangeListDtos,
                        _ => _.ChangeType.ToString(),
                        _ => _.EntityTypeFullName,
                        _ => _.UserName,
                        _ => _timeZoneConverter.Convert(_.ChangeTime, _AbpSession.TenantId, _AbpSession.GetUserId())
                    );

                    //Formatting cells

                    var timeColumn = sheet.Column(1);
                    timeColumn.Style.Numberformat.Format = "yyyy-mm-dd hh:mm:ss";

                    for (var i = 1; i <= 10; i++)
                    {
                        if (i.IsIn(5, 10)) //Don't AutoFit Parameters and Exception
                        {
                            continue;
                        }

                        sheet.Column(i).AutoFit();
                    }
                });
        }
    }
}