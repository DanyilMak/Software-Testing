using Lab1.Core;
using Shouldly;
using Xunit;

namespace Lab1.Tests;

public class CollectionUtilsTests
{

    [Fact]
    public void Average_NormalNumbers()
    {
        var result = CollectionUtils.Average(new[] { 2, 4, 6 });
        result.ShouldBe(4);
    }

    [Fact]
    public void Average_SingleNumber()
    {
        var result = CollectionUtils.Average(new[] { 5 });
        result.ShouldBe(5);
    }

    [Fact]
    public void Average_EmptyCollection_Throws()
    {
        Should.Throw<InvalidOperationException>(() =>
            CollectionUtils.Average(Array.Empty<int>())
        );
    }

    [Fact]
    public void Max_ReturnsCorrectValue()
    {
        var result = CollectionUtils.Max(new[] { 1, 5, 3 });
        result.ShouldBe(5);
    }

    [Fact]
    public void Max_SingleValue()
    {
        var result = CollectionUtils.Max(new[] { 7 });
        result.ShouldBe(7);
    }

    [Fact]
    public void Max_EmptyCollection_Throws()
    {
        Should.Throw<InvalidOperationException>(() =>
            CollectionUtils.Max(Array.Empty<int>())
        );
    }

    [Fact]
    public void Distinct_RemovesDuplicates()
    {
        var result = CollectionUtils.Distinct(new[] { 1, 1, 2, 3, 3 });

        result.ShouldBe(new[] { 1, 2, 3 });
    }

    [Fact]
    public void Distinct_NoDuplicates()
    {
        var result = CollectionUtils.Distinct(new[] { 1, 2, 3 });

        result.ShouldBe(new[] { 1, 2, 3 });
    }

    [Fact]
    public void Chunk_CreatesChunks()
    {
        var result = CollectionUtils.Chunk(new[] { 1, 2, 3, 4 }, 2).ToList();

        result.Count.ShouldBe(2);
        result[0].ShouldBe(new List<int> { 1, 2 });
        result[1].ShouldBe(new List<int> { 3, 4 });
    }

    [Fact]
    public void Chunk_LastChunkSmaller()
    {
        var result = CollectionUtils.Chunk(new[] { 1, 2, 3 }, 2).ToList();

        result.Count.ShouldBe(2);
        result[1].ShouldBe(new List<int> { 3 });
    }

    [Fact]
    public void Chunk_SizeZero_Throws()
    {
        Should.Throw<ArgumentException>(() =>
            CollectionUtils.Chunk(new[] { 1, 2, 3 }, 0).ToList()
        );
    }

}