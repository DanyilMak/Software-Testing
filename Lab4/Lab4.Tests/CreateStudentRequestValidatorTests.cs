using Lab4.Api;
using FluentValidation.TestHelper;
using Xunit;

public class CreateStudentRequestValidatorTests
{
    private readonly CreateStudentRequestValidator _validator = new();

    [Fact]
    public void FullName_ShouldHaveError_WhenEmpty()
    {
        // Arrange
        var model = new CreateStudentRequest { FullName = "" };

        // Act
        var result = _validator.TestValidate(model);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.FullName);
    }

    [Fact]
    public void Email_ShouldHaveError_WhenInvalid()
    {
        // Arrange
        var model = new CreateStudentRequest { Email = "not-an-email" };

        // Act
        var result = _validator.TestValidate(model);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Email);
    }

    [Fact]
    public void EnrollmentDate_ShouldHaveError_WhenFuture()
    {
        // Arrange
        var model = new CreateStudentRequest { EnrollmentDate = DateTime.UtcNow.AddDays(1) };

        // Act
        var result = _validator.TestValidate(model);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.EnrollmentDate);
    }

    [Fact]
    public void Model_ShouldBeValid_WhenCorrect()
    {
        // Arrange
        var model = new CreateStudentRequest
        {
            FullName = "Valid Name",
            Email = "valid@example.com",
            EnrollmentDate = DateTime.UtcNow.AddSeconds(-1)
        };

        // Act
        var result = _validator.TestValidate(model);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }
}
