using Lab1.Core;
using Shouldly;
using Xunit;

namespace Lab1.Tests;

public class CalculatorTests
{
    private readonly Calculator _sut = new();

    [Fact]
    public void Add_TwoNumbers_ReturnsSum()
    {
        // Arrange
        double a = 2;
        double b = 3;

        // Act
        var result = _sut.Add(a, b);

        // Assert
        result.ShouldBe(5);
    }

    [Fact]
    public void Subtract_TwoNumbers_ReturnsDifference()
    {
        var result = _sut.Subtract(10, 3);
        result.ShouldBe(7);
    }

    [Fact]
    public void Multiply_TwoNumbers_ReturnsProduct()
    {
        var result = _sut.Multiply(4, 5);
        result.ShouldBe(20);
    }

    [Fact]
    public void Divide_TwoNumbers_ReturnsQuotient()
    {
        var result = _sut.Divide(10, 2);
        result.ShouldBe(5);
    }

    [Fact]
    public void Divide_ByZero_ThrowsException()
    {
        Should.Throw<DivideByZeroException>(() =>
            _sut.Divide(10, 0)
        );
    }

    [Fact]
    public void Add_NegativeNumbers_WorksCorrectly()
    {
        var result = _sut.Add(-5, -5);
        result.ShouldBe(-10);
    }

    [Fact]
    public void Multiply_ByZero_ReturnsZero()
    {
        var result = _sut.Multiply(10, 0);
        result.ShouldBe(0);
    }

    [Fact]
    public void FloatingPoint_Addition_IsCorrect()
    {
        var result = _sut.Add(0.1, 0.2);
        result.ShouldBe(0.3, 0.0001);
    }
}