using Lab1.Core;
using Shouldly;
using Xunit;

namespace Lab1.Tests;

public class StringUtilsTests
{

    [Fact]
    public void Capitalize_FirstLetterUppercase()
    {
        var result = StringUtils.Capitalize("hello");
        result.ShouldBe("Hello");
    }

    [Fact]
    public void Capitalize_AlreadyCapitalized()
    {
        var result = StringUtils.Capitalize("Hello");
        result.ShouldBe("Hello");
    }

    [Fact]
    public void Capitalize_EmptyString()
    {
        var result = StringUtils.Capitalize("");
        result.ShouldBe("");
    }

    [Fact]
    public void Reverse_String()
    {
        var result = StringUtils.Reverse("hello");
        result.ShouldBe("olleh");
    }

    [Fact]
    public void Reverse_SingleCharacter()
    {
        var result = StringUtils.Reverse("a");
        result.ShouldBe("a");
    }

    [Fact]
    public void Reverse_Throws_WhenNull()
    {
        Should.Throw<ArgumentNullException>(() =>
            StringUtils.Reverse(null)
        );
    }

    [Fact]
    public void Palindrome_True()
    {
        var result = StringUtils.IsPalindrome("level");
        result.ShouldBeTrue();
    }

    [Fact]
    public void Palindrome_False()
    {
        var result = StringUtils.IsPalindrome("hello");
        result.ShouldBeFalse();
    }

    [Fact]
    public void Palindrome_CaseInsensitive()
    {
        var result = StringUtils.IsPalindrome("Level");
        result.ShouldBeTrue();
    }

    [Fact]
    public void Truncate_String()
    {
        var result = StringUtils.Truncate("HelloWorld", 5);
        result.ShouldBe("Hello");
    }

    [Fact]
    public void Truncate_ShorterString()
    {
        var result = StringUtils.Truncate("Hi", 5);
        result.ShouldBe("Hi");
    }

    [Fact]
    public void Truncate_Null()
    {
        var result = StringUtils.Truncate(null, 5);
        result.ShouldBeNull();
    }
}