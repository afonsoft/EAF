using System.IO;
using Castle.Facilities.Logging;
using Castle.Windsor;
using Castle.Core.Logging;
using Abp.IO;
using Shouldly;
using Xunit;
using Eaf.Castle.Logging.SerilogIntegration;
using Serilog;

namespace Abp.Castle.Serilog.Tests
{
    public class Castle_Serilog_Tests
    {
        [Fact]
        public void Should_Write_Logs_To_Text_File()
        {
            //Arrange
            var logFilePath = Path.Combine(Directory.GetCurrentDirectory(), "AbpCastleSerilogTests-Logs.txt");
            FileHelper.DeleteIfExists(logFilePath); //Clean old file

            Log.Logger = new LoggerConfiguration()
                .WriteTo.File(logFilePath)
                .CreateLogger();

            //Act
            var container = new WindsorContainer();
            container.AddFacility<LoggingFacility>(facility =>
            {
                facility.UseEafSerilog();
            });

            var logger = container.Resolve<ILoggerFactory>().Create(typeof(Castle_Serilog_Tests));
            logger.Info("Should_Write_Logs_To_Text_File works!");

            //Assert
            File.Exists(logFilePath).ShouldBeTrue();
        }
    }
}