using Abp.Domain.Entities;
using System;

namespace Eaf.MiddlewareCore.SampleApp.Core.BookStore
{
    public class Book : Entity<Guid>
    {
        public string Name { get; set; }
    }
}