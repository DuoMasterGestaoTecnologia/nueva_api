using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using OmniSuite.API.Controllers;
using OmniSuite.Application.User.Commands;
using OmniSuite.Application.User.Queries;
using OmniSuite.Application.User.Responses;
using OmniSuite.Domain.Entities;
using OmniSuite.Domain.Utils;
using OmniSuite.Tests.Common;
using OmniSuite.Tests.Common.Factories;
using Xunit;

namespace OmniSuite.Tests.API.Controllers
{
    public class UserControllerTests : TestBase
    {
        private readonly UserController _controller;
        private readonly Mock<ILogger<UserController>> _mockLogger;

        public UserControllerTests()
        {
            _mockLogger = new Mock<ILogger<UserController>>();
            _controller = new UserController(_mockLogger.Object);
        }

        [Fact]
        public void Constructor_ShouldCreateInstance()
        {
            // Arrange & Act
            var controller = new UserController(_mockLogger.Object);

            // Assert
            controller.Should().NotBeNull();
            controller.Should().BeOfType<UserController>();
        }

        [Fact]
        public void GetUser_WhenCalled_ShouldReturnOkResult()
        {
            // Arrange
            var query = new GetUserQuery();

            // Act
            var result = _controller.GetUser(query);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public void GetUser_WhenCalled_ShouldReturnCorrectResponseType()
        {
            // Arrange
            var query = new GetUserQuery();

            // Act
            var result = _controller.GetUser(query) as OkObjectResult;

            // Assert
            result.Should().NotBeNull();
            result!.Value.Should().BeOfType<GetUserResponse>();
        }

        [Fact]
        public void UpdateUser_WhenCalled_ShouldReturnOkResult()
        {
            // Arrange
            var command = CommandFactory.CreateValidUpdateUserCommand();

            // Act
            var result = _controller.UpdateUser(command);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public void UpdateUser_WhenCalled_ShouldReturnCorrectResponseType()
        {
            // Arrange
            var command = CommandFactory.CreateValidUpdateUserCommand();

            // Act
            var result = _controller.UpdateUser(command) as OkObjectResult;

            // Assert
            result.Should().NotBeNull();
            result!.Value.Should().BeOfType<bool>();
        }

        [Fact]
        public void UpdatePhoto_WhenCalled_ShouldReturnOkResult()
        {
            // Arrange
            var command = CommandFactory.CreateValidUpdatePhotoCommand();

            // Act
            var result = _controller.UpdatePhoto(command);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public void UpdatePhoto_WhenCalled_ShouldReturnCorrectResponseType()
        {
            // Arrange
            var command = CommandFactory.CreateValidUpdatePhotoCommand();

            // Act
            var result = _controller.UpdatePhoto(command) as OkObjectResult;

            // Assert
            result.Should().NotBeNull();
            result!.Value.Should().BeOfType<bool>();
        }

        [Fact]
        public void SetupMFA_WhenCalled_ShouldReturnOkResult()
        {
            // Arrange
            var query = new SetupMFAQuery();

            // Act
            var result = _controller.SetupMFA(query);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public void SetupMFA_WhenCalled_ShouldReturnCorrectResponseType()
        {
            // Arrange
            var query = new SetupMFAQuery();

            // Act
            var result = _controller.SetupMFA(query) as OkObjectResult;

            // Assert
            result.Should().NotBeNull();
            result!.Value.Should().BeOfType<SetupMFAResponse>();
        }

        [Fact]
        public void CreateMFA_WhenCalled_ShouldReturnOkResult()
        {
            // Arrange
            var command = CommandFactory.CreateValidMFACommand();

            // Act
            var result = _controller.CreateMFA(command);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public void CreateMFA_WhenCalled_ShouldReturnCorrectResponseType()
        {
            // Arrange
            var command = CommandFactory.CreateValidMFACommand();

            // Act
            var result = _controller.CreateMFA(command) as OkObjectResult;

            // Assert
            result.Should().NotBeNull();
            result!.Value.Should().BeOfType<CreateMFAUserResponse>();
        }

        [Fact]
        public void GetUserLogged_WhenCalled_ShouldReturnOkResult()
        {
            // Arrange
            var query = new UserLoggedQuery();

            // Act
            var result = _controller.GetUserLogged(query);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public void GetUserLogged_WhenCalled_ShouldReturnCorrectResponseType()
        {
            // Arrange
            var query = new UserLoggedQuery();

            // Act
            var result = _controller.GetUserLogged(query) as OkObjectResult;

            // Assert
            result.Should().NotBeNull();
            result!.Value.Should().BeOfType<UserLoggedResponse>();
        }

        [Fact]
        public void GetUserByEmail_WhenCalled_ShouldReturnOkResult()
        {
            // Arrange
            var query = new UserByEmailQuery("test@example.com");

            // Act
            var result = _controller.GetUserByEmail(query);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public void GetUserByEmail_WhenCalled_ShouldReturnCorrectResponseType()
        {
            // Arrange
            var query = new UserByEmailQuery("test@example.com");

            // Act
            var result = _controller.GetUserByEmail(query) as OkObjectResult;

            // Assert
            result.Should().NotBeNull();
            result!.Value.Should().BeOfType<UserByEmailResponse>();
        }

        [Fact]
        public void Controller_ShouldInheritFromBaseController()
        {
            // Arrange & Act
            var controller = new UserController(_mockLogger.Object);

            // Assert
            controller.Should().BeAssignableTo<BaseController>();
        }

        [Fact]
        public void Controller_ShouldHaveCorrectRouteAttribute()
        {
            // Arrange & Act
            var controller = new UserController(_mockLogger.Object);
            var routeAttribute = controller.GetType().GetCustomAttributes(typeof(Microsoft.AspNetCore.Mvc.RouteAttribute), true)
                .FirstOrDefault() as Microsoft.AspNetCore.Mvc.RouteAttribute;

            // Assert
            routeAttribute.Should().NotBeNull();
            routeAttribute!.Template.Should().Be("api/[controller]");
        }

        [Fact]
        public void Controller_ShouldHaveAuthorizeAttribute()
        {
            // Arrange & Act
            var controller = new UserController(_mockLogger.Object);
            var authorizeAttribute = controller.GetType().GetCustomAttributes(typeof(Microsoft.AspNetCore.Authorization.AuthorizeAttribute), true)
                .FirstOrDefault();

            // Assert
            authorizeAttribute.Should().NotBeNull();
        }

        [Theory]
        [InlineData("GetUser")]
        [InlineData("UpdateUser")]
        [InlineData("UpdatePhoto")]
        [InlineData("SetupMFA")]
        [InlineData("CreateMFA")]
        [InlineData("GetUserLogged")]
        [InlineData("GetUserByEmail")]
        public void Controller_ShouldHaveRequiredMethods(string methodName)
        {
            // Arrange & Act
            var controller = new UserController(_mockLogger.Object);
            var method = controller.GetType().GetMethod(methodName);

            // Assert
            method.Should().NotBeNull();
            method!.IsPublic.Should().BeTrue();
        }

        [Fact]
        public void Controller_ShouldHaveCorrectNamespace()
        {
            // Arrange & Act
            var controller = new UserController(_mockLogger.Object);

            // Assert
            controller.GetType().Namespace.Should().Be("OmniSuite.API.Controllers");
        }
    }
}
