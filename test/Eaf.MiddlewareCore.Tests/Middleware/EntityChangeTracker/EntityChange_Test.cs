using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.EntityFrameworkCore.Extensions;
using Abp.EntityFrameworkCore.Repositories;
using Eaf.MiddlewareCore.SampleApp.Core.EntityHistory;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Shouldly;
using Xunit;

namespace Eaf.Middleware.EntityChangeTracker
{
    public class EntityChangeTracker_Test : EafMiddlewareTestBase
    {
        private readonly IRepository<Blog> _blogRepository;

        public EntityChangeTracker_Test()
        {
            _blogRepository = Resolve<IRepository<Blog>>();
        }

        [Fact]
        public void Entity_Change_Should_Check_OwnedEntity()
        {
            using (var uow = Resolve<IUnitOfWorkManager>().Begin())
            {
                var blog1 = _blogRepository.Single(b => b.Name == "test-blog-1");

                //blog1.More is Owned Entity
                blog1.More.BloggerName = "test-blog-2";

                _blogRepository.GetDbContext().Entry(blog1).State.ShouldBe(EntityState.Unchanged);
                _blogRepository.GetDbContext().Entry(blog1.More).State.ShouldBe(EntityState.Modified);
                _blogRepository.GetDbContext().Entry(blog1).CheckOwnedEntityChange().ShouldBeTrue();

                uow.Complete();
            }
        }
    }
}