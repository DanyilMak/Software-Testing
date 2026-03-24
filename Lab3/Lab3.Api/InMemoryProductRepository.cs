public class InMemoryProductRepository : IProductRepository
{
    private readonly List<Product> _products = new();

    public IEnumerable<Product> GetAll() => _products;
    public Product? GetById(int id) => _products.FirstOrDefault(p => p.Id == id);
    public void Add(Product product)
    {
        if (product.Id == 0)
        {
            product.Id = _products.Count == 0 ? 1 : _products.Max(p => p.Id) + 1;
        }
        _products.Add(product);
    }
    public void Update(Product product)
    {
        var existing = GetById(product.Id);
        if (existing != null)
        {
            existing.Name = product.Name;
            existing.Price = product.Price;
        }
    }
    public void Delete(int id) => _products.RemoveAll(p => p.Id == id);
}
