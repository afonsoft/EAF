using Microsoft.Extensions.Options;
using System;

namespace Eaf.Hangfire
{
    /// <summary>
    /// Representa a classe EafHangFireOptions.
    /// </summary>
    public class EafHangFireOptions : IOptions<EafHangFireOptions>
    {
        /// <summary>
        /// /hangfire
        /// </summary>
        public string PathMatch { get; set; } = "/hangfire";

        /// <summary>
        /// The path for the Back To Site link. Set to null in order to hide the Back To Site link.
        /// </summary>
        public string AppPath { get; set; }

        /// <summary>
        /// Summary: The path for the first url prefix link, eg. set "/admin", then url is "{domain}/{PrefixPath}/{hangfire}"
        /// </summary>
        public string PrefixPath { get; set; }

        /// <summary>
        /// The Title displayed on the dashboard, optionally modify to describe this dashboards purpose.
        /// </summary>
        public string DashboardTitle { get; set; }

        /// <summary>
        /// Hangfire Is Enabled?
        /// </summary>
        public bool IsEnabled { get; set; } = true;

        /// <summary>
        /// Tipo de armazenamento utilizado pelo Hangfire.
        /// Determinado automaticamente com base no provider do banco e configurações de Redis.
        /// </summary>
        public HangfireStorageType StorageType { get; set; } = HangfireStorageType.SqlServer;

        /// <summary>
        /// Default RequiredPermission
        /// </summary>
        public string[] RequiredPermissionName { get; set; } = new[] { "Pages.Administration", "Pages.Administration.HangfireDashboard" };

        /// <summary>
        /// Default Queues "default"
        /// </summary>
        public string[] Queues { get; set; } = new[] { "default" };

        /// <summary>
        /// Default Environment.ProcessorCount * 4, max 16 work default
        /// </summary>
        public int WorkerCount { get; set; } = Environment.ProcessorCount * 4 > 16 ? 16 : Environment.ProcessorCount * 4;

        public EafHangFireOptions Value => this;
    }
}
