namespace Lab2.Core;

public interface IOrderRepository
{
    Order GetById(int id);
    void Save(Order order);
    IEnumerable<Order> GetByCustomerId(int customerId);
}