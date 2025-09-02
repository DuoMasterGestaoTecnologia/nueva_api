using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using OmniSuite.API.Controllers;
using OmniSuite.Application.Generic.Responses;
using MediatR;
using Xunit;

namespace OmniSuite.Tests.API.Controllers
{
    public class BaseControllerTests
    {
        private readonly Mock<IMediator> _mockMediator;
        private readonly TestController _controller;
        private readonly Mock<HttpContext> _mockHttpContext;
        private readonly Mock<IServiceProvider> _mockServiceProvider;

        public BaseControllerTests()
        {
            _mockMediator = new Mock<IMediator>();
            _controller = new TestController();
            _mockHttpContext = new Mock<HttpContext>();
            _mockServiceProvider = new Mock<IServiceProvider>();

            _mockHttpContext.Setup(x => x.RequestServices).Returns(_mockServiceProvider.Object);
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = _mockHttpContext.Object
            };
        }

        [Fact]
        public void SetMediator_ShouldSetMediatorForTesting()
        {
            // Act
            _controller.SetMediator(_mockMediator.Object);

            // Assert
            var mediator = _controller.GetMediator();
            Assert.Equal(_mockMediator.Object, mediator);
        }

        [Fact]
        public void Mediator_WhenNotSet_ShouldGetFromServiceProvider()
        {
            // Arrange
            _mockServiceProvider.Setup(x => x.GetService(typeof(IMediator)))
                .Returns(_mockMediator.Object);

            // Act
            var mediator = _controller.Mediator;

            // Assert
            Assert.Equal(_mockMediator.Object, mediator);
        }

        [Fact]
        public void Mediator_WhenServiceProviderNotAvailable_ShouldThrowException()
        {
            // Arrange
            _mockHttpContext.Setup(x => x.RequestServices).Returns((IServiceProvider)null!);

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => _controller.Mediator);
        }

        [Fact]
        public async Task SendCommand_WithSuccessfulResponse_ShouldReturnOk()
        {
            // Arrange
            var command = new Mock<IRequest<Response<string>>>();
            var response = Response<string>.Ok("Success");
            
            _mockMediator.Setup(x => x.Send(command.Object, It.IsAny<CancellationToken>()))
                .ReturnsAsync(response);
            
            _controller.SetMediator(_mockMediator.Object);

            // Act
            var result = await _controller.SendCommand(command.Object);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(response, okResult.Value);
        }

        [Fact]
        public async Task SendCommand_WithFailedResponse_ShouldReturnBadRequest()
        {
            // Arrange
            var command = new Mock<IRequest<Response<string>>>();
            var response = Response<string>.Fail("Error");
            
            _mockMediator.Setup(x => x.Send(command.Object, It.IsAny<CancellationToken>()))
                .ReturnsAsync(response);
            
            _controller.SetMediator(_mockMediator.Object);

            // Act
            var result = await _controller.SendCommand(command.Object);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(response, badRequestResult.Value);
        }

        [Fact]
        public async Task SendQuery_WithSuccessfulResponse_ShouldReturnOk()
        {
            // Arrange
            var query = new Mock<IRequest<Response<string>>>();
            var response = Response<string>.Ok("Success");
            
            _mockMediator.Setup(x => x.Send(query.Object, It.IsAny<CancellationToken>()))
                .ReturnsAsync(response);
            
            _controller.SetMediator(_mockMediator.Object);

            // Act
            var result = await _controller.SendQuery(query.Object);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(response, okResult.Value);
        }

        [Fact]
        public async Task SendQuery_WithFailedResponse_ShouldReturnBadRequest()
        {
            // Arrange
            var query = new Mock<IRequest<Response<string>>>();
            var response = Response<string>.Fail("Error");
            
            _mockMediator.Setup(x => x.Send(query.Object, It.IsAny<CancellationToken>()))
                .ReturnsAsync(response);
            
            _controller.SetMediator(_mockMediator.Object);

            // Act
            var result = await _controller.SendQuery(query.Object);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(response, badRequestResult.Value);
        }

        [Fact]
        public async Task SendQueryRaw_ShouldReturnOk()
        {
            // Arrange
            var query = new Mock<IRequest<string>>();
            var response = "Raw Result";
            
            _mockMediator.Setup(x => x.Send(query.Object, It.IsAny<CancellationToken>()))
                .ReturnsAsync(response);
            
            _controller.SetMediator(_mockMediator.Object);

            // Act
            var result = await _controller.SendQueryRaw(query.Object);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(response, okResult.Value);
        }

        // Test controller to access protected methods
        private class TestController : BaseController
        {
            public new IMediator Mediator => base.Mediator;
            public new Task<IActionResult> SendCommand<T>(IRequest<Response<T>> command) => base.SendCommand(command);
            public new Task<IActionResult> SendQuery<T>(IRequest<Response<T>> query) => base.SendQuery(query);
            public new Task<IActionResult> SendQueryRaw<T>(IRequest<T> query) => base.SendQueryRaw(query);
        }
    }
}
