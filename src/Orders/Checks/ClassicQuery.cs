using Microsoft.EntityFrameworkCore;
using Shop.Classic;
using Shop.Model;

namespace Shop.Checks;

// The old-style extension method inside a query. This compiles.
public static class ClassicQuery
{
    public static List<Order> OverdueOrders(ShopDb db) =>
        db.Orders.Where(o => o.IsOverdue()).ToList();

    public static Exception? Run(ShopDb db)
    {
        try
        {
            OverdueOrders(db);
            return null;
        }
        catch (Exception ex)
        {
            return ex;
        }
    }
}
