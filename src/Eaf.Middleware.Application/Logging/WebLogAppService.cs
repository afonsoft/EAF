using Abp.Authorization;
using Eaf.Middleware.Authorization;
using Eaf.Middleware.Dto;
using Eaf.Middleware.IO;
using Eaf.Middleware.Logging.Dto;
using Eaf.Middleware.Net.MimeTypes;
using Eaf.Middleware.Storage;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;

namespace Eaf.Middleware.Logging
{
    /// <summary>
    /// Serviço de aplicação para gerenciamento de WebLog.
    /// </summary>
    [AbpAuthorize(MiddlewarePermissions.Pages_Administration_Maintenance)]
    public class WebLogAppService : MiddlewareAppServiceBase, IWebLogAppService
    {
        private readonly IAppFolders _appFolders;
        private readonly ITempFileCacheManager _tempFileCacheManager;

        /// <summary>
        /// WebLogAppService.
        /// </summary>
        /// <param name="appFolders">Parâmetro appFolders.</param>
        /// <param name="tempFileCacheManager">Parâmetro tempFileCacheManager.</param>
        /// <returns>Resultado da operação.</returns>
        public WebLogAppService(IAppFolders appFolders, ITempFileCacheManager tempFileCacheManager)
        {
            _appFolders = appFolders;
            _tempFileCacheManager = tempFileCacheManager;
        }

        [Produces("application/json", "application/json-patch+json", "text/json")]
        public FileDto DownloadWebLogs()
        {
            //Create temporary copy of logs
            var logFiles = GetAllLogFiles();

            //Create the zip file
            var zipFileDto = new FileDto("WebSiteLogs.zip", MimeTypeNames.ApplicationZip);

            using (var outputZipFileStream = new MemoryStream())
            {
                using (var zipStream = new ZipArchive(outputZipFileStream, ZipArchiveMode.Create))
                {
                    foreach (var logFile in logFiles)
                    {
                        var entry = zipStream.CreateEntry(logFile.Name);
                        using (var entryStream = entry.Open())
                        {
                            using (var fs = new FileStream(logFile.FullName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite, 0x1000, FileOptions.SequentialScan))
                            {
                                fs.CopyTo(entryStream);
                                entryStream.Flush();
                            }
                        }
                    }
                }

                _tempFileCacheManager.SetFile(zipFileDto.FileToken, outputZipFileStream.ToArray());
            }

            return zipFileDto;
        }

        [Produces("application/json", "application/json-patch+json", "text/json")]
        public GetLatestWebLogsOutput GetLatestWebLogs()
        {
            var directory = new DirectoryInfo(_appFolders.WebLogsFolder);
            if (!directory.Exists)
            {
                return new GetLatestWebLogsOutput { LatestWebLogLines = new List<string>() };
            }

            var lastLogFile = directory.GetFiles("*.txt", SearchOption.AllDirectories)
                                        .OrderByDescending(f => f.LastWriteTime)
                                        .FirstOrDefault();

            if (lastLogFile == null)
            {
                return new GetLatestWebLogsOutput();
            }

            const int maxLinesToRead = 1000;
            const int logLinesToReturn = 100;
            var logLevels = new[] { "[IMF]", "INFO", "[DBG]", "DEBUG", "[WRN]", "WARNING", "[ERR]", "ERROR", "[FAT]", "[FTL]", "FATAL" };

            var recentLines = File.ReadLines(lastLogFile.FullName)
                .Reverse()
                .Take(maxLinesToRead)
                .ToList();

            var logLineCount = 0;
            var lineCount = 0;
            for (int i = 0; i < recentLines.Count; i++)
            {
                if (logLevels.Any(level => recentLines[i].Contains(level)))
                {
                    logLineCount++;
                }

                lineCount++;

                if (logLineCount == logLinesToReturn)
                {
                    break;
                }
            }

            return new GetLatestWebLogsOutput
            {
                LatestWebLogLines = recentLines.Take(lineCount).Reverse().ToList()
            };
        }

        private List<FileInfo> GetAllLogFiles()
        {
            var directory = new DirectoryInfo(_appFolders.WebLogsFolder);
            return directory.GetFiles("*.*", SearchOption.TopDirectoryOnly).ToList();
        }
    }
}