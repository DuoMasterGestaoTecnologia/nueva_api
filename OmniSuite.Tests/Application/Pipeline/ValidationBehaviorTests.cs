using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.Logging;
using Moq;
using OmniSuite.Application.Pipeline;
using OmniSuite.Tests.Common;
using Xunit;

namespace OmniSuite.Tests.Application.Pipeline
{
    public class ValidationBehaviorTests : TestBase
    {
        private ValidationBehavior<TestRequest, TestResponse> _behavior;

        public ValidationBehaviorTests()
        {
        }

        [Fact]
        public async Task Handle_WhenNoValidators_ShouldCallNextHandler()
        {
            // Arrange
            var request = new TestRequest { Name = "Test" };
            var expectedResponse = new TestResponse { Result = "Success" };
            var validators = Enumerable.Empty<IValidator<TestRequest>>();
            _behavior = new ValidationBehavior<TestRequest, TestResponse>(validators);

            // Act
            var result = await _behavior.Handle(request, () => Task.FromResult(expectedResponse), CancellationToken.None);

            // Assert
            result.Should().BeEquivalentTo(expectedResponse);
        }

        [Fact]
        public async Task Handle_WhenValidatorsPass_ShouldCallNextHandler()
        {
            // Arrange
            var request = new TestRequest { Name = "Test" };
            var expectedResponse = new TestResponse { Result = "Success" };
            
            var mockValidator = new Mock<IValidator<TestRequest>>();
            mockValidator.Setup(x => x.ValidateAsync(It.IsAny<ValidationContext<TestRequest>>(), It.IsAny<CancellationToken>()))
                       .ReturnsAsync(new ValidationResult());
            
            var validators = new[] { mockValidator.Object };
            _behavior = new ValidationBehavior<TestRequest, TestResponse>(validators);

            // Act
            var result = await _behavior.Handle(request, () => Task.FromResult(expectedResponse), CancellationToken.None);

            // Assert
            result.Should().BeEquivalentTo(expectedResponse);
            mockValidator.Verify(x => x.ValidateAsync(It.IsAny<ValidationContext<TestRequest>>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_WhenValidatorsFail_ShouldThrowValidationException()
        {
            // Arrange
            var request = new TestRequest { Name = "Test" };
            var validationFailures = new List<ValidationFailure>
            {
                new ValidationFailure("Name", "Name is required")
            };
            
            var mockValidator = new Mock<IValidator<TestRequest>>();
            mockValidator.Setup(x => x.ValidateAsync(It.IsAny<ValidationContext<TestRequest>>(), It.IsAny<CancellationToken>()))
                       .ReturnsAsync(new ValidationResult(validationFailures));
            
            var validators = new[] { mockValidator.Object };
            _behavior = new ValidationBehavior<TestRequest, TestResponse>(validators);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ValidationException>(() =>
                _behavior.Handle(request, () => Task.FromResult(new TestResponse()), CancellationToken.None));

            exception.Errors.Should().HaveCount(1);
            exception.Errors.First().PropertyName.Should().Be("Name");
            exception.Errors.First().ErrorMessage.Should().Be("Name is required");
        }

        [Fact]
        public async Task Handle_WhenMultipleValidatorsFail_ShouldThrowValidationExceptionWithAllFailures()
        {
            // Arrange
            var request = new TestRequest { Name = "Test" };
            var validationFailures1 = new List<ValidationFailure>
            {
                new ValidationFailure("Name", "Name is required")
            };
            var validationFailures2 = new List<ValidationFailure>
            {
                new ValidationFailure("Email", "Email is invalid")
            };
            
            var mockValidator1 = new Mock<IValidator<TestRequest>>();
            mockValidator1.Setup(x => x.ValidateAsync(It.IsAny<ValidationContext<TestRequest>>(), It.IsAny<CancellationToken>()))
                         .ReturnsAsync(new ValidationResult(validationFailures1));
            
            var mockValidator2 = new Mock<IValidator<TestRequest>>();
            mockValidator2.Setup(x => x.ValidateAsync(It.IsAny<ValidationContext<TestRequest>>(), It.IsAny<CancellationToken>()))
                         .ReturnsAsync(new ValidationResult(validationFailures2));
            
            var validators = new[] { mockValidator1.Object, mockValidator2.Object };
            _behavior = new ValidationBehavior<TestRequest, TestResponse>(validators);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ValidationException>(() =>
                _behavior.Handle(request, () => Task.FromResult(new TestResponse()), CancellationToken.None));

            exception.Errors.Should().HaveCount(2);
            exception.Errors.Should().Contain(x => x.PropertyName == "Name" && x.ErrorMessage == "Name is required");
            exception.Errors.Should().Contain(x => x.PropertyName == "Email" && x.ErrorMessage == "Email is invalid");
        }

        // Test classes for the tests
        public class TestRequest
        {
            public string Name { get; set; } = string.Empty;
        }

        public class TestResponse
        {
            public string Result { get; set; } = string.Empty;
        }
    }
}
