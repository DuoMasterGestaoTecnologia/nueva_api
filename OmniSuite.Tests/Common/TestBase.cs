using AutoFixture;
using AutoFixture.AutoMoq;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;

namespace OmniSuite.Tests.Common
{
    public abstract class TestBase
    {
        protected readonly IFixture Fixture;
        protected readonly Mock<ILogger> MockLogger;

        protected TestBase()
        {
            Fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            MockLogger = new Mock<ILogger>();
        }

        protected IServiceCollection CreateServiceCollection()
        {
            var services = new ServiceCollection();
            
            // Add common test services
            services.AddLogging(builder =>
            {
                builder.AddProvider(new Mock<ILoggerProvider>().Object);
            });

            return services;
        }

        protected T CreateMock<T>() where T : class
        {
            return new Mock<T>().Object;
        }
    }
}
