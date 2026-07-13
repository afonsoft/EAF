using Abp;
using Abp.Collections.Extensions;
using Abp.Dependency;
using Eaf.Middleware.Dto;
using Eaf.Middleware.Localization;
using Eaf.Middleware.Net.MimeTypes;
using Eaf.Middleware.Storage;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace Eaf.Middleware.DataExporting.Excel.EpPlus
{
    /// <summary>
    /// Representa a classe EpPlusExcelExporterBase.
    /// </summary>
    public abstract class EpPlusExcelExporterBase : AbpServiceBase, ITransientDependency
    {
        private readonly ITempFileCacheManager _tempFileCacheManager;

        protected EpPlusExcelExporterBase(
            ITempFileCacheManager tempFileCacheManager
        )
        {
            _tempFileCacheManager = tempFileCacheManager;

            LocalizationSourceName = MiddlewareAppConsts.LocalizationSourceName;
        }

        /// <summary>
        /// Obtém a string localizada com fallback para múltiplos sources.
        /// </summary>
        protected override string L(string name)
        {
            return MiddlewareLocalizationHelper.Localize(LocalizationManager, name);
        }

        /// <summary>
        /// Obtém a string localizada com fallback para múltiplos sources com formatação.
        /// </summary>
        protected override string L(string name, params object[] args)
        {
            return MiddlewareLocalizationHelper.Localize(LocalizationManager, name, args);
        }

        /// <summary>
        /// Obtém a string localizada com fallback para múltiplos sources para uma cultura específica.
        /// </summary>
        protected override string L(string name, CultureInfo culture)
        {
            return MiddlewareLocalizationHelper.Localize(LocalizationManager, name, culture);
        }

        /// <summary>
        /// Add Header to Excel
        /// </summary>
        /// <param name="sheet">Sheet <see cref="ExcelWorksheet"/></param>
        /// <param name="headerTexts">Header Texts</param>
        protected static void AddHeader(ExcelWorksheet sheet, params string[] headerTexts)
        {
            if (headerTexts.IsNullOrEmpty())
            {
                return;
            }

            for (var i = 0; i < headerTexts.Length; i++)
            {
                AddHeader(sheet, i + 1, headerTexts[i]);
            }
        }

        /// <summary>
        /// Add Header to Excel
        /// </summary>
        /// <param name="sheet">Sheet <see cref="ExcelWorksheet"/></param>
        /// <param name="columnIndex">Column Index</param>
        /// <param name="headerText">Header Text</param>
        protected static void AddHeader(ExcelWorksheet sheet, int columnIndex, string headerText)
        {
            sheet.Cells[1, columnIndex].Value = headerText;
            sheet.Cells[1, columnIndex].Style.Font.Bold = true;
        }

        /// <summary>
        /// Add Objects to Excel
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sheet">Sheet <see cref="ExcelWorksheet"/></param>
        /// <param name="startRowIndex">Start Row Index</param>
        /// <param name="items">items <see cref="List{T}"/></param>
        /// <param name="propertySelectors">Property Selectors</param>
        protected static void AddObjects<T>(ExcelWorksheet sheet, int startRowIndex, IList<T> items, params Func<T, object>[] propertySelectors)
        {
            if (items.IsNullOrEmpty() || propertySelectors.IsNullOrEmpty())
            {
                return;
            }

            for (var i = 0; i < items.Count; i++)
            {
                for (var j = 0; j < propertySelectors.Length; j++)
                {
                    sheet.Cells[i + startRowIndex, j + 1].Value = propertySelectors[j](items[i]);
                }
            }
        }

        /// <summary>
        /// CreateExcelPackage Create a Excel with EpPlus
        /// </summary>
        /// <param name="fileName">Name of file</param>
        /// <param name="creator">Action ExcelPackage <see cref="ExcelPackage"/></param>
        /// <returns></returns>
        protected FileDto CreateExcelPackage(string fileName, Action<ExcelPackage> creator)
        {
            var file = new FileDto(fileName, MimeTypeNames.ApplicationVndOpenxmlformatsOfficedocumentSpreadsheetmlSheet);

            ExcelPackage.License.SetNonCommercialOrganization("EAF");
            using (var excelPackage = new ExcelPackage())
            {
                creator(excelPackage);
                Save(excelPackage, file);
            }

            return file;
        }

        protected void Save(ExcelPackage excelPackage, FileDto file)
        {
            _tempFileCacheManager.SetFile(file.FileToken, excelPackage.GetAsByteArray());
        }
    }
}
