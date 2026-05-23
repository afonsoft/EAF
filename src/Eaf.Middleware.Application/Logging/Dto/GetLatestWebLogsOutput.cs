using System.Collections.Generic;

namespace Eaf.Middleware.Logging.Dto
{
    /// <summary>
    /// Representa a classe GetLatestWebLogsOutput.
    /// </summary>
    public class GetLatestWebLogsOutput
    {
        public List<string> LatestWebLogLines { get; set; }
    }
}