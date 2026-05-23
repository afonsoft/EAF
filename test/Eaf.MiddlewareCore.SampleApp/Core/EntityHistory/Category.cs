using Abp.Auditing;
using System.ComponentModel.DataAnnotations;

namespace Eaf.MiddlewareCore.SampleApp.Core.EntityHistory
{
    public class Category
    {
        [Audited]
        public string DisplayName { get; set; }

        [Key]
        public int Id { get; set; }
    }
}