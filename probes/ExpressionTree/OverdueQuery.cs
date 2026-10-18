using Shop.Model;
using Shop.Naive;

namespace Probes;

public static class OverdueQuery
{
    public static List<Order> Run(ShopDb db) =>
        db.Orders.Where(o => o.IsOverdue).ToList();
}
