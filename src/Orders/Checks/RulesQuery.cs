using Microsoft.EntityFrameworkCore;
using Shop.Model;
using Shop.Rules;

namespace Shop.Checks;

// The version I'd ship: the query takes the expression, plain code reads the
// extension property built from it.
public static class RulesQuery
{
    public static IQueryable<Order> OverdueQuery(ShopDb db) =>
        db.Orders.Where(OrderRules.Overdue);

    public static List<string> OverdueFromDatabase(ShopDb db) =>
        OverdueQuery(db).OrderBy(o => o.Id).Select(o => o.Customer).ToList();

    public static string Sql(ShopDb db) => OverdueQuery(db).ToQueryString();

    public static List<string> OverdueInMemory(ShopDb db) =>
        db.Orders.OrderBy(o => o.Id).ToList()
            .Where(o => o.IsOverdue)
            .Select(o => o.Customer)
            .ToList();
}
