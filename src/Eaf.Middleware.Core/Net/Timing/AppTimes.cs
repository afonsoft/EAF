using Abp.Dependency;
using System;

namespace Eaf.Middleware.Timing
{
    /// <summary>
    /// Representa a classe AppTimes.
    /// </summary>
    public class AppTimes : ISingletonDependency
    {
        /// <summary>
        /// Gets the startup time of the application.
        /// </summary>
        public DateTime StartupTime { get; set; }
    }
}