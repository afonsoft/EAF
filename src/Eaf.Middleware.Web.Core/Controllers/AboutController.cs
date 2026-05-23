using Abp.Application.Services;
using Abp.Auditing;
using Abp.Authorization;
using Abp.Modules;
using Abp.Runtime;
using Abp.Runtime.System;
using Eaf.Middleware;
using Eaf.Middleware.StringExtensions;
using Eaf.Middleware.Web.Controllers;
using Eaf.Models.About;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Eaf.Controllers
{
    /// <summary>
    /// Representa a classe AboutController.
    /// </summary>
    public class AboutController : MiddlewareControllerBase, IApplicationService
    {
        private readonly IAbpModuleManager _AbpModuleManager;
        private readonly IOSPlatformProvider _iOSPlatformProvider;
        private readonly string[] envAccept = { "ASPNETCORE", "ASP", "DOTNET", "USERNAME", "TEMP", "EAF", "SYSTEM", "HTTP", "IIS" };

        /// <summary>
        /// AboutController.
        /// </summary>
        /// <param name="AbpModuleManager">Parâmetro AbpModuleManager.</param>
        /// <param name="iOSPlatformProvider">Parâmetro iOSPlatformProvider.</param>
        /// <returns>Resultado da operação.</returns>
        public AboutController(IAbpModuleManager AbpModuleManager, IOSPlatformProvider iOSPlatformProvider)
        {
            _AbpModuleManager = AbpModuleManager;
            _iOSPlatformProvider = iOSPlatformProvider;
        }

        private Dictionary<string, string> GetEnvironmentVariables()
        {
            var envs = new Dictionary<string, string>();
            foreach (DictionaryEntry env in Environment.GetEnvironmentVariables())
            {
                if (env.Key != null && env.Value != null && env.Key.ToString().IsContains(envAccept))
                {
                    envs.Add(env.Key.ToString(), env.Value.ToString());
                }
            }
            return envs;
        }

        [AbpAllowAnonymous]
        [DisableAuditing]
        [HttpGet]
        public AboutModel GetAbout()
        {
            return new AboutModel
            {
                Version = typeof(MiddlewareCoreModule).Assembly.ToString(),
                Architecture = System.Runtime.InteropServices.RuntimeInformation.ProcessArchitecture.ToString(),
                RuntimeIdentifier = System.Runtime.InteropServices.RuntimeInformation.RuntimeIdentifier,
                FrameworkDescription = System.Runtime.InteropServices.RuntimeInformation.FrameworkDescription,
                TotalAvailableMemory = GC.GetGCMemoryInfo().TotalAvailableMemoryBytes.FormatSize(),
                CurrentCulture = CultureInfo.CurrentCulture.Name + " - " + CultureInfo.CurrentCulture.DisplayName,
                CurrentTimeZoneLocal = TimeZoneInfo.Local.Id + " - " + TimeZoneInfo.Local.DisplayName,
                CurrentEnviromment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT"),
                CurrentDirectory = AppDomain.CurrentDomain.BaseDirectory,
                MachineName = Environment.MachineName,
                OSVersion = Environment.OSVersion.VersionString,
                OS = _iOSPlatformProvider.GetCurrentOSPlatform().ToString(),
                NumberOfProcessors = Environment.ProcessorCount.ToString(),
                ProcessName = Process.GetCurrentProcess().ProcessName,
                PagedMemorySize = Process.GetCurrentProcess().PagedMemorySize64.FormatSize(),
                PrivateMemorySize = Process.GetCurrentProcess().PrivateMemorySize64.FormatSize(),
                VirtualMemorySize = Process.GetCurrentProcess().VirtualMemorySize64.FormatSize(),
                WorkingMemoryUsed = Process.GetCurrentProcess().WorkingSet64.FormatSize(),
                Modules = _AbpModuleManager.Modules.Select(x => x.Assembly.FullName).ToArray(),
                Environments = GetEnvironmentVariables()
            };
        }
    }
}