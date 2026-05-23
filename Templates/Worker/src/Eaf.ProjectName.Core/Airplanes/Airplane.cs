using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Eaf.Domain.Entities.Auditing;
using Eaf.Domain.Entities;
using Eaf.Auditing;

namespace Eaf.ProjectName.Airplanes
{
	[Table("EafAirplanes")]
    [Audited]
    public class Airplane : FullAuditedEntity, IMayHaveTenant
    {
        public const int MaxModelLength = 256;

        public int? TenantId { get; set; }
			
		[Required]
		public virtual string Number { get; set; }
		
		[Required]
		[StringLength(MaxModelLength)]
		public virtual string Model { get; set; }
    }
}