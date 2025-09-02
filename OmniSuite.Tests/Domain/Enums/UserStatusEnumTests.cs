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
                         values.Should().HaveCount(3);
             values.Should().Contain(UserStatusEnum.registered);
             values.Should().Contain(UserStatusEnum.approved);
             values.Should().Contain(UserStatusEnum.inactive);
        }

        [Fact]
        public void UserStatusEnum_ShouldHaveCorrectDefaultValue()
        {
            // Arrange & Act
            var defaultValue = default(UserStatusEnum);

            // Assert
            defaultValue.Should().Be(UserStatusEnum.inactive);
        }

                 [Theory]
         [InlineData(UserStatusEnum.inactive, 0)]
         [InlineData(UserStatusEnum.registered, 1)]
         [InlineData(UserStatusEnum.approved, 2)]
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
                         var inactive = UserStatusEnum.inactive;
             var registered = UserStatusEnum.registered;
             var approved = UserStatusEnum.approved;

                         // Act & Assert
             ((int)inactive).Should().BeLessThan((int)registered);
             ((int)registered).Should().BeLessThan((int)approved);
        }

        [Fact]
        public void UserStatusEnum_ShouldSupportBitwiseOperations()
        {
            // Arrange
            var status1 = UserStatusEnum.approved;
            var status2 = UserStatusEnum.inactive;

            // Act
            var combined = status1 | status2;

            // Assert
            // For enums without [Flags] attribute, bitwise OR returns the higher value
            combined.Should().Be(UserStatusEnum.approved);
        }

        [Fact]
        public void UserStatusEnum_ShouldBeSerializable()
        {
            // Arrange
                         var status = UserStatusEnum.approved;

            // Act
            var serialized = status.ToString();

            // Assert
            serialized.Should().Be("approved");
        }

        [Fact]
        public void UserStatusEnum_ShouldBeParseable()
        {
            // Arrange
                         var statusString = "approved";

            // Act
            var parsed = Enum.Parse<UserStatusEnum>(statusString);

            // Assert
            parsed.Should().Be(UserStatusEnum.approved);
        }

        [Fact]
        public void UserStatusEnum_ShouldSupportTryParse()
        {
            // Arrange
                         var validStatusString = "approved";
            var invalidStatusString = "invalid_status";

            // Act
            var validResult = Enum.TryParse<UserStatusEnum>(validStatusString, out var validStatus);
            var invalidResult = Enum.TryParse<UserStatusEnum>(invalidStatusString, out var invalidStatus);

            // Assert
            validResult.Should().BeTrue();
                         validStatus.Should().Be(UserStatusEnum.approved);
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
                         var status = UserStatusEnum.approved;
            var result = string.Empty;

            // Act
            switch (status)
            {
                                 case UserStatusEnum.inactive:
                     result = "inactive";
                     break;
                 case UserStatusEnum.registered:
                     result = "registered";
                     break;
                 case UserStatusEnum.approved:
                     result = "approved";
                     break;
                default:
                    result = "unknown";
                    break;
            }

            // Assert
                         result.Should().Be("approved");
        }
    }
}
