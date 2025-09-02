using AutoFixture;
using AutoFixture.AutoMoq;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using Moq;
using System.Security.Claims;
using OmniSuite.Domain.Utils;

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
                builder.ClearProviders();
                builder.SetMinimumLevel(LogLevel.Warning);
            });

            // Add HttpContextAccessor for UserClaimsHelper
            services.AddHttpContextAccessor();

            return services;
        }

        protected void SetupAuthenticatedUser(Guid userId, string email = "test@example.com")
        {
            var httpContext = new DefaultHttpContext();
            var claims = new List<Claim>
            {
                new Claim("userId", userId.ToString()),
                new Claim(ClaimTypes.Email, email)
            };
            var identity = new ClaimsIdentity(claims, "Test");
            httpContext.User = new ClaimsPrincipal(identity);
            
            var mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
            mockHttpContextAccessor.Setup(x => x.HttpContext).Returns(httpContext);
            
            UserClaimsHelper.Configure(mockHttpContextAccessor.Object);
        }

        protected T CreateMock<T>() where T : class
        {
            return new Mock<T>().Object;
        }
    }
}
