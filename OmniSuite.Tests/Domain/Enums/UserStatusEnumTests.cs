using FluentAssertions;
using OmniSuite.Domain.Enums;
using Xunit;

namespace OmniSuite.Tests.Domain.Enums
{
    public class UserStatusEnumTests
    {
        [Fact]
        public void UserStatusEnum_ShouldHaveExpectedValues()
        {
            // Arrange & Act
            var values = Enum.GetValues<UserStatusEnum>();

            // Assert
            values.Should().HaveCount(4);
            values.Should().Contain(UserStatusEnum.registered);
            values.Should().Contain(UserStatusEnum.active);
            values.Should().Contain(UserStatusEnum.inactive);
            values.Should().Contain(UserStatusEnum.suspended);
        }

        [Fact]
        public void UserStatusEnum_ShouldHaveCorrectDefaultValue()
        {
            // Arrange & Act
            var defaultValue = default(UserStatusEnum);

            // Assert
            defaultValue.Should().Be(UserStatusEnum.registered);
        }

        [Theory]
        [InlineData(UserStatusEnum.registered, 0)]
        [InlineData(UserStatusEnum.active, 1)]
        [InlineData(UserStatusEnum.inactive, 2)]
        [InlineData(UserStatusEnum.suspended, 3)]
        public void UserStatusEnum_ShouldHaveCorrectNumericValues(UserStatusEnum status, int expectedValue)
        {
            // Arrange & Act
            var numericValue = (int)status;

            // Assert
            numericValue.Should().Be(expectedValue);
        }

        [Fact]
        public void UserStatusEnum_ShouldBeComparable()
        {
            // Arrange
            var registered = UserStatusEnum.registered;
            var active = UserStatusEnum.active;
            var inactive = UserStatusEnum.inactive;
            var suspended = UserStatusEnum.suspended;

            // Act & Assert
            registered.Should().BeLessThan(active);
            active.Should().BeLessThan(inactive);
            inactive.Should().BeLessThan(suspended);
        }

        [Fact]
        public void UserStatusEnum_ShouldSupportBitwiseOperations()
        {
            // Arrange
            var status1 = UserStatusEnum.active;
            var status2 = UserStatusEnum.inactive;

            // Act
            var combined = status1 | status2;

            // Assert
            combined.Should().Be(UserStatusEnum.inactive); // Assuming bitwise OR behavior
        }

        [Fact]
        public void UserStatusEnum_ShouldBeSerializable()
        {
            // Arrange
            var status = UserStatusEnum.active;

            // Act
            var serialized = status.ToString();

            // Assert
            serialized.Should().Be("active");
        }

        [Fact]
        public void UserStatusEnum_ShouldBeParseable()
        {
            // Arrange
            var statusString = "active";

            // Act
            var parsed = Enum.Parse<UserStatusEnum>(statusString);

            // Assert
            parsed.Should().Be(UserStatusEnum.active);
        }

        [Fact]
        public void UserStatusEnum_ShouldSupportTryParse()
        {
            // Arrange
            var validStatusString = "active";
            var invalidStatusString = "invalid_status";

            // Act
            var validResult = Enum.TryParse<UserStatusEnum>(validStatusString, out var validStatus);
            var invalidResult = Enum.TryParse<UserStatusEnum>(invalidStatusString, out var invalidStatus);

            // Assert
            validResult.Should().BeTrue();
            validStatus.Should().Be(UserStatusEnum.active);
            invalidResult.Should().BeFalse();
            invalidStatus.Should().Be(default(UserStatusEnum));
        }

        [Fact]
        public void UserStatusEnum_ShouldHaveDistinctValues()
        {
            // Arrange
            var values = Enum.GetValues<UserStatusEnum>();

            // Act
            var distinctValues = values.Distinct();

            // Assert
            distinctValues.Should().HaveCount(values.Length);
        }

        [Fact]
        public void UserStatusEnum_ShouldBeUsedInSwitchStatements()
        {
            // Arrange
            var status = UserStatusEnum.active;
            var result = string.Empty;

            // Act
            switch (status)
            {
                case UserStatusEnum.registered:
                    result = "registered";
                    break;
                case UserStatusEnum.active:
                    result = "active";
                    break;
                case UserStatusEnum.inactive:
                    result = "inactive";
                    break;
                case UserStatusEnum.suspended:
                    result = "suspended";
                    break;
                default:
                    result = "unknown";
                    break;
            }

            // Assert
            result.Should().Be("active");
        }
    }
}
