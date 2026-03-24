public class ThrowingRepository : IProductRepository
{
    public IEnumerable<Product> GetAll() => throw new Exception("Test exception from repository");
    public Product? GetById(int id) => throw new Exception("Test exception from repository");
    public void Add(Product product) => throw new Exception("Test exception from repository");
    public void Update(Product product) => throw new Exception("Test exception from repository");
    public void Delete(int id) => throw new Exception("Test exception from repository");
}
