using Abp.Domain.Entities;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Eaf.MiddlewareCore.SampleApp.Core.BookStore
{
    public class Author : Entity<Guid>
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public override Guid Id { get; set; }

        public string Name { get; set; }
    }
}