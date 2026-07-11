using Eaf.Hangfire;
using Hangfire;
using Hangfire.Common;
using Shouldly;
using System.ComponentModel;
using System.Reflection;
using Xunit;

namespace Eaf.Hangfire.Tests
{
    public class EafDisplayNameExtensionsBddTests
    {
        [Fact]
        public void Dado_JobComDisplayNameAttribute_Quando_Format_Entao_DeveRetornarFormatadoComArgs()
        {
            var job = new Job(typeof(StaticJobs), StaticJobs.GetMethod(nameof(StaticJobs.JobWithDisplayName)), new object[] { "Alice", 42 });

            var result = EafDisplayNameExtensions.Format(null, job);

            result.ShouldBe("Hello Alice, 42");
        }

        [Fact]
        public void Dado_JobComJobDisplayNameAttribute_Quando_Format_Entao_DeveRetornarFormatadoComArgs()
        {
            var job = new Job(typeof(StaticJobs), StaticJobs.GetMethod(nameof(StaticJobs.JobWithJobDisplayName)), new object[] { "Bob" });

            var result = EafDisplayNameExtensions.Format(null, job);

            result.ShouldBe("Job display for Bob");
        }

        [Fact]
        public void Dado_JobSemDisplayAttribute_Quando_Format_Entao_DeveRetornarNomeDoMetodo()
        {
            var job = new Job(typeof(StaticJobs), StaticJobs.GetMethod(nameof(StaticJobs.JobWithoutDisplayName)), new object[] { });

            var result = EafDisplayNameExtensions.Format(null, job);

            result.ShouldBe("StaticJobs.JobWithoutDisplayName");
        }

        [Fact]
        public void Dado_JobComDisplayNameInvalido_Quando_Format_Entao_DeveRetornarNomeDoMetodo()
        {
            var job = new Job(typeof(StaticJobs), StaticJobs.GetMethod(nameof(StaticJobs.JobWithBadDisplayName)), new object[] { });

            var result = EafDisplayNameExtensions.Format(null, job);

            result.ShouldBe("StaticJobs.JobWithBadDisplayName");
        }

        private static class StaticJobs
        {
            public static MethodInfo GetMethod(string name) => typeof(StaticJobs).GetMethod(name, BindingFlags.Public | BindingFlags.Static);

            [DisplayName("Hello {0}, {1}")]
            public static void JobWithDisplayName(string name, int id) { }

            [JobDisplayName("Job display for {0}")]
            public static void JobWithJobDisplayName(string name) { }

            public static void JobWithoutDisplayName() { }

            [DisplayName("Bad {0}")]
            public static void JobWithBadDisplayName() { }
        }
    }
}
