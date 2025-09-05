using FluentValidation.TestHelper;
using OmniSuite.Application.DigitalProduct.Commands;
using OmniSuite.Application.DigitalProduct.Validations;
using OmniSuite.Domain.Enums;
using Xunit;

namespace OmniSuite.Tests.Application.DigitalProduct.Validations
{
    public class CreateDigitalProductCommandValidationTests
    {
        private readonly CreateDigitalProductCommandValidation _validator;

        public CreateDigitalProductCommandValidationTests()
        {
            _validator = new CreateDigitalProductCommandValidation();
        }

        [Fact]
        public void Should_Have_Error_When_Name_Is_Empty()
        {
            // Arrange
            var command = new CreateDigitalProductCommand
            {
                Name = "",
                Description = "Valid description",
                Price = 19.99m,
                Type = DigitalProductTypeEnum.Ebook
            };

            // Act & Assert
            _validator.TestValidate(command)
                .ShouldHaveValidationErrorFor(x => x.Name)
                .WithErrorMessage("Nome é obrigatório");
        }

        [Fact]
        public void Should_Have_Error_When_Name_Exceeds_MaxLength()
        {
            // Arrange
            var command = new CreateDigitalProductCommand
            {
                Name = new string('A', 201), // 201 characters
                Description = "Valid description",
                Price = 19.99m,
                Type = DigitalProductTypeEnum.Ebook
            };

            // Act & Assert
            _validator.TestValidate(command)
                .ShouldHaveValidationErrorFor(x => x.Name)
                .WithErrorMessage("Nome deve ter no máximo 200 caracteres");
        }

        [Fact]
        public void Should_Have_Error_When_Description_Is_Empty()
        {
            // Arrange
            var command = new CreateDigitalProductCommand
            {
                Name = "Valid name",
                Description = "",
                Price = 19.99m,
                Type = DigitalProductTypeEnum.Ebook
            };

            // Act & Assert
            _validator.TestValidate(command)
                .ShouldHaveValidationErrorFor(x => x.Description)
                .WithErrorMessage("Descrição é obrigatória");
        }

        [Fact]
        public void Should_Have_Error_When_Description_Exceeds_MaxLength()
        {
            // Arrange
            var command = new CreateDigitalProductCommand
            {
                Name = "Valid name",
                Description = new string('A', 2001), // 2001 characters
                Price = 19.99m,
                Type = DigitalProductTypeEnum.Ebook
            };

            // Act & Assert
            _validator.TestValidate(command)
                .ShouldHaveValidationErrorFor(x => x.Description)
                .WithErrorMessage("Descrição deve ter no máximo 2000 caracteres");
        }

        [Fact]
        public void Should_Have_Error_When_Price_Is_Zero()
        {
            // Arrange
            var command = new CreateDigitalProductCommand
            {
                Name = "Valid name",
                Description = "Valid description",
                Price = 0m,
                Type = DigitalProductTypeEnum.Ebook
            };

            // Act & Assert
            _validator.TestValidate(command)
                .ShouldHaveValidationErrorFor(x => x.Price)
                .WithErrorMessage("Preço deve ser maior que zero");
        }

        [Fact]
        public void Should_Have_Error_When_Price_Is_Negative()
        {
            // Arrange
            var command = new CreateDigitalProductCommand
            {
                Name = "Valid name",
                Description = "Valid description",
                Price = -10m,
                Type = DigitalProductTypeEnum.Ebook
            };

            // Act & Assert
            _validator.TestValidate(command)
                .ShouldHaveValidationErrorFor(x => x.Price)
                .WithErrorMessage("Preço deve ser maior que zero");
        }

        [Fact]
        public void Should_Have_Error_When_Type_Is_Invalid()
        {
            // Arrange
            var command = new CreateDigitalProductCommand
            {
                Name = "Valid name",
                Description = "Valid description",
                Price = 19.99m,
                Type = (DigitalProductTypeEnum)999 // Invalid enum value
            };

            // Act & Assert
            _validator.TestValidate(command)
                .ShouldHaveValidationErrorFor(x => x.Type)
                .WithErrorMessage("Tipo de produto inválido");
        }

        [Fact]
        public void Should_Have_Error_When_DownloadLimit_Is_Zero()
        {
            // Arrange
            var command = new CreateDigitalProductCommand
            {
                Name = "Valid name",
                Description = "Valid description",
                Price = 19.99m,
                Type = DigitalProductTypeEnum.Ebook,
                DownloadLimit = 0
            };

            // Act & Assert
            _validator.TestValidate(command)
                .ShouldHaveValidationErrorFor(x => x.DownloadLimit)
                .WithErrorMessage("Limite de downloads deve ser maior que zero");
        }

        [Fact]
        public void Should_Have_Error_When_ExpirationDate_Is_In_Past()
        {
            // Arrange
            var command = new CreateDigitalProductCommand
            {
                Name = "Valid name",
                Description = "Valid description",
                Price = 19.99m,
                Type = DigitalProductTypeEnum.Ebook,
                ExpirationDate = DateTime.UtcNow.AddDays(-1)
            };

            // Act & Assert
            _validator.TestValidate(command)
                .ShouldHaveValidationErrorFor(x => x.ExpirationDate)
                .WithErrorMessage("Data de expiração deve ser futura");
        }

        [Fact]
        public void Should_Have_Error_When_Category_Exceeds_MaxLength()
        {
            // Arrange
            var command = new CreateDigitalProductCommand
            {
                Name = "Valid name",
                Description = "Valid description",
                Price = 19.99m,
                Type = DigitalProductTypeEnum.Ebook,
                Category = new string('A', 101) // 101 characters
            };

            // Act & Assert
            _validator.TestValidate(command)
                .ShouldHaveValidationErrorFor(x => x.Category)
                .WithErrorMessage("Categoria deve ter no máximo 100 caracteres");
        }

        [Fact]
        public void Should_Have_Error_When_Tags_Exceed_MaxLength()
        {
            // Arrange
            var command = new CreateDigitalProductCommand
            {
                Name = "Valid name",
                Description = "Valid description",
                Price = 19.99m,
                Type = DigitalProductTypeEnum.Ebook,
                Tags = new string('A', 501) // 501 characters
            };

            // Act & Assert
            _validator.TestValidate(command)
                .ShouldHaveValidationErrorFor(x => x.Tags)
                .WithErrorMessage("Tags devem ter no máximo 500 caracteres");
        }

        [Fact]
        public void Should_Not_Have_Error_When_Command_Is_Valid()
        {
            // Arrange
            var command = new CreateDigitalProductCommand
            {
                Name = "Valid Product Name",
                Description = "Valid product description",
                Price = 29.99m,
                Type = DigitalProductTypeEnum.Ebook,
                Category = "Education",
                Tags = "ebook, education, learning",
                DownloadLimit = 5,
                ExpirationDate = DateTime.UtcNow.AddDays(30)
            };

            // Act & Assert
            _validator.TestValidate(command)
                .ShouldNotHaveAnyValidationErrors();
        }

        [Fact]
        public void Should_Not_Have_Error_When_Optional_Fields_Are_Null()
        {
            // Arrange
            var command = new CreateDigitalProductCommand
            {
                Name = "Valid Product Name",
                Description = "Valid product description",
                Price = 29.99m,
                Type = DigitalProductTypeEnum.Ebook,
                ShortDescription = null,
                Category = null,
                Tags = null,
                DownloadLimit = null,
                ExpirationDate = null
            };

            // Act & Assert
            _validator.TestValidate(command)
                .ShouldNotHaveAnyValidationErrors();
        }
    }
}
