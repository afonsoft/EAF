using System;
using System.IO;
using Abp.Modules;
using Abp.Reflection.Extensions;
using Abp.TestBase;

namespace Eaf.BlobStoring.Tests
{
    /// <summary>
    /// Módulo de testes para o Eaf.BlobStoring.
    /// </summary>
    [DependsOn(typeof(EafBlobStoringModule), typeof(AbpTestBaseModule))]
    public class BlobStoringTestModule : AbpModule
    {
        /// <summary>
        /// Configura o caminho base temporário para os testes de FileSystem.
        /// </summary>
        public override void PreInitialize()
        {
            Configuration.Modules.EafBlobStoring().FileSystemBasePath = Path.Combine(
                Path.GetTempPath(),
                "eaf-blob-tests",
                Guid.NewGuid().ToString("N"));
        }

        /// <summary>
        /// Registra os tipos deste assembly por convenção.
        /// </summary>
        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(BlobStoringTestModule).GetAssembly());
        }
    }
}
