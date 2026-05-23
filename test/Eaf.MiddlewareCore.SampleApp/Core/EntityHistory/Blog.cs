using Abp.Auditing;
using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;

namespace Eaf.MiddlewareCore.SampleApp.Core.EntityHistory
{
    [Audited]
    public class Blog : AggregateRoot, IHasCreationTime
    {
        public Blog()
        {
        }

        public Blog(string name, string url, string bloggerName)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentNullException(nameof(name));
            }

            if (string.IsNullOrWhiteSpace(url))
            {
                throw new ArgumentNullException(nameof(url));
            }

            Name = name;
            Url = url;
            More = new BlogEx { BloggerName = bloggerName };
        }

        public DateTime CreationTime { get; set; }
        public BlogEx More { get; set; }
        [DisableAuditing] public string Name { get; set; }

        public ICollection<Post> Posts { get; set; }
        public ICollection<BlogPromotion> Promotions { get; set; }
        public string Url { get; protected set; }

        public void ChangeUrl(string url)
        {
            if (string.IsNullOrWhiteSpace(url))
            {
                throw new ArgumentNullException(nameof(url));
            }

            var oldUrl = Url;
            Url = url;
        }
    }

    public class BlogEx
    {
        public string BloggerName { get; set; }
    }

    public class BlogPromotion
    {
        public int AdvertisementId { get; set; }
        public int BlogId { get; set; }
        public string Title { get; set; }
    }
}