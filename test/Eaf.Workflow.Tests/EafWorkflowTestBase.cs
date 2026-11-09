using Abp.AutoMapper;
using Abp.Domain.Entities;
using Abp.Domain.Repositories;
using Abp.Modules;
using Abp.Reflection.Extensions;
using Abp.TestBase;
using Eaf.Workflow;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace Eaf.Workflow.Tests
{
    /// <summary>
    /// Base para testes do módulo Eaf.Workflow — repositórios em memória.
    /// </summary>
    public abstract class EafWorkflowTestBase : AbpIntegratedTestBase<EafWorkflowTestModule>
    {
        /// <summary>Gerenciador de IoC local.</summary>
        protected IIocManagerAlias IocManager => new(LocalIocManager);

        /// <summary>Cria um repositório fake com os membros usados pelo motor.</summary>
        protected static IRepository<TEntity, Guid> FakeRepo<TEntity>(List<TEntity> store)
            where TEntity : class, IEntity<Guid>
        {
            var repo = Substitute.For<IRepository<TEntity, Guid>>();
            repo.InsertAsync(Arg.Any<TEntity>())
                .Returns(ci =>
                {
                    var e = ci.Arg<TEntity>();
                    if (e.Id == Guid.Empty)
                    {
                        e.Id = Guid.NewGuid();
                    }
                    store.Add(e);
                    return Task.FromResult(e);
                });
            repo.InsertOrUpdateAndGetIdAsync(Arg.Any<TEntity>())
                .Returns(ci =>
                {
                    var e = ci.Arg<TEntity>();
                    if (e.Id == Guid.Empty)
                    {
                        e.Id = Guid.NewGuid();
                    }
                    if (!store.Contains(e))
                    {
                        store.Add(e);
                    }
                    return Task.FromResult(e.Id);
                });
            repo.GetAsync(Arg.Any<Guid>())
                .Returns(ci => Task.FromResult(store.FirstOrDefault(e => e.Id == ci.Arg<Guid>())));
            repo.FirstOrDefaultAsync(Arg.Any<Guid>())
                .Returns(ci => Task.FromResult(store.FirstOrDefault(e => e.Id == ci.Arg<Guid>())));
            repo.FirstOrDefaultAsync(Arg.Any<Expression<Func<TEntity, bool>>>())
                .Returns(ci => Task.FromResult(store.AsQueryable().FirstOrDefault(ci.Arg<Expression<Func<TEntity, bool>>>())));
            repo.GetAllListAsync()
                .Returns(Task.FromResult(store.ToList()));
            repo.GetAllListAsync(Arg.Any<Expression<Func<TEntity, bool>>>())
                .Returns(ci => Task.FromResult(store.AsQueryable().Where(ci.Arg<Expression<Func<TEntity, bool>>>()).ToList()));
            repo.UpdateAsync(Arg.Any<TEntity>())
                .Returns(ci => Task.FromResult(ci.Arg<TEntity>()));
            repo.DeleteAsync(Arg.Any<TEntity>())
                .Returns(ci =>
                {
                    store.Remove(ci.Arg<TEntity>());
                    return Task.CompletedTask;
                });
            return repo;
        }
    }

    /// <summary>Alias para expor LocalIocManager de forma curta.</summary>
    public class IIocManagerAlias
    {
        private readonly Abp.Dependency.IIocManager _inner;

        /// <summary>IIocManagerAlias.</summary>
        public IIocManagerAlias(Abp.Dependency.IIocManager inner) => _inner = inner;

        /// <summary>Resolve.</summary>
        public T Resolve<T>() => _inner.Resolve<T>();
    }

    /// <summary>
    /// Módulo de testes para Eaf.Workflow.
    /// </summary>
    [DependsOn(typeof(EafWorkflowModule), typeof(AbpTestBaseModule))]
    public class EafWorkflowTestModule : AbpModule
    {
        /// <summary>Configura AutoMapper e desabilita autorização.</summary>
        public override void PreInitialize()
        {
#pragma warning disable CS0618
            Configuration.Modules.AbpAutoMapper().UseStaticMapper = false;
#pragma warning restore CS0618
            Configuration.Authorization.IsEnabled = false;
        }

        /// <inheritdoc />
        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(EafWorkflowTestModule).GetAssembly());
        }
    }
}
