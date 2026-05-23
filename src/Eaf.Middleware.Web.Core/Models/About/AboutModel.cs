using System.Collections.Generic;
using System.ComponentModel;

namespace Eaf.Models.About
{
    /// <summary>
    /// Representa a classe AboutModel.
    /// </summary>
    public class AboutModel
    {
        [Description("Version")]
        public string Version { get; set; }

        [Description("OS Version")]
        public string OSVersion { get; set; }

        [Description("OS")]
        public string OS { get; set; }

        [Description("Number of Processors")]
        public string NumberOfProcessors { get; set; }

        [Description("Machine Name")]
        public string MachineName { get; set; }

        [Description("Architecture")]
        public string Architecture { get; set; }

        [Description("Runtime Identifier")]
        public string RuntimeIdentifier { get; set; }

        [Description("Framework Description")]
        public string FrameworkDescription { get; set; }

        [Description("Total Available Memory")]
        public string TotalAvailableMemory { get; set; }

        [Description("Current Culture")]
        public string CurrentCulture { get; set; }

        [Description("Current TimeZone Local")]
        public string CurrentTimeZoneLocal { get; set; }

        [Description("Current Enviromment")]
        public string CurrentEnviromment { get; set; }

        [Description("Current Directory")]
        public string CurrentDirectory { get; set; }

        [Description("Process Name")]
        public string ProcessName { get; set; }

        [Description("Paged Memory Size")]
        public string PagedMemorySize { get; set; }

        [Description("Private Memory Size")]
        public string PrivateMemorySize { get; set; }

        [Description("Virtual Memory Size")]
        public string VirtualMemorySize { get; set; }

        [Description("Working Memory Used")]
        public string WorkingMemoryUsed { get; set; }

        [Description("Modules")]
        public string[] Modules { get; set; }

        [Description("Environments")]
        public Dictionary<string, string> Environments { get; set; }
    }
}