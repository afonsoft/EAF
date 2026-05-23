using Abp.Auditing;
using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System;

namespace Eaf.MiddlewareCore.SampleApp.Core.EntityHistory
{
    [Audited]
    public class UserTestEntity : AggregateRoot, IHasCreationTime
    {
        public int Age { get; set; }
        public DateTime CreationTime { get; set; }

        public string Name { get; set; }

        public string Surname { get; set; }
    }
}