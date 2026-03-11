namespace Lab1.Core;

public static class CollectionUtils
{
    public static double Average(IEnumerable<int> numbers)
    {
        if (numbers == null)
            throw new ArgumentNullException(nameof(numbers));

        if (!numbers.Any())
            throw new InvalidOperationException("Collection is empty");

        return numbers.Average();
    }

    public static int Max(IEnumerable<int> numbers)
    {
        if (numbers == null)
            throw new ArgumentNullException(nameof(numbers));

        if (!numbers.Any())
            throw new InvalidOperationException("Collection is empty");

        return numbers.Max();
    }

    public static IEnumerable<int> Distinct(IEnumerable<int> numbers)
    {
        if (numbers == null)
            throw new ArgumentNullException(nameof(numbers));

        return numbers.Distinct();
    }

    public static IEnumerable<List<int>> Chunk(IEnumerable<int> numbers, int size)
    {
        if (numbers == null)
            throw new ArgumentNullException(nameof(numbers));

        if (size <= 0)
            throw new ArgumentException("Size must be greater than zero");

        var list = numbers.ToList();

        for (int i = 0; i < list.Count; i += size)
        {
            yield return list.Skip(i).Take(size).ToList();
        }
    }
}