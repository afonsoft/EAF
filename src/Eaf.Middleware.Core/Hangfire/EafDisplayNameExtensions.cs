using Hangfire;
using Hangfire.Common;
using Hangfire.Dashboard;
using System;
using System.ComponentModel;
using System.Linq;

namespace Eaf.Hangfire
{
    /// <summary>
    /// Representa a classe EafDisplayNameExtensions.
    /// </summary>
    public static class EafDisplayNameExtensions
    {
        /// <summary>
        /// Format.
        /// </summary>
        /// <param name="context">Parâmetro context.</param>
        /// <param name="job">Parâmetro job.</param>
        /// <returns>Resultado da operação.</returns>
        public static string Format(DashboardContext context, Job job)
        {
            try
            {
                if (Attribute.GetCustomAttribute(job.Method, typeof(DisplayNameAttribute), true) is DisplayNameAttribute displayNameAttribute && displayNameAttribute.DisplayName != null)
                    return String.Format(displayNameAttribute.DisplayName, job.Args.ToArray());
                else if (Attribute.GetCustomAttribute(job.Method, typeof(JobDisplayNameAttribute), true) is JobDisplayNameAttribute jobdisplayNameAttribute && jobdisplayNameAttribute.DisplayName != null)
                    return String.Format(jobdisplayNameAttribute.DisplayName, job.Args.ToArray());
                else
                    return String.Format("{0}.{1}", job.Type.Name, job.Method.Name);
            }
            catch
            {
                return String.Format("{0}.{1}", job.Type.Name, job.Method.Name);
            }
        }
    }
}