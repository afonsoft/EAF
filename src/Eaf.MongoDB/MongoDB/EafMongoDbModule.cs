using Abp;
using Abp.Modules;
using Abp.Reflection.Extensions;
using Castle.MicroKernel.Registration;
using MongoDB.Driver;

namespace Eaf.MongoDB
{
    /// <summary>
    /// Módulo Eaf.MongoDB — contexto MongoDB com isolamento por tenant.
    /// </summary>
    [DependsOn(typeof(Abp.AbpKernelModule))]
    public class EafMongoDbModule : AbpModule
    {
        /// <inheritdoc />
        public override void PreInitialize()
        {
            IocManager.IocContainer.Register(
                Component.For<EafMongoDbOptions>().ImplementedBy<EafMongoDbOptions>().LifestyleSingleton());
        }

        /// <inheritdoc />
        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(EafMongoDbModule).GetAssembly());
        }

        /// <inheritdoc />
        public override void PostInitialize()
        {
            var options = IocManager.Resolve<EafMongoDbOptions>();

            IocManager.IocContainer.Register(
                Component.For<IMongoClient>()
                    .UsingFactoryMethod(() => new MongoClient(options.ConnectionString))
                    .LifestyleSingleton()
                    .Named("EafMongoClient"),
                Component.For<IMongoDatabase>()
                    .UsingFactoryMethod(k => k.Resolve<IMongoClient>().GetDatabase(options.DatabaseName))
                    .LifestyleSingleton()
                    .Named("EafMongoDatabase"));
        }
    }
}
