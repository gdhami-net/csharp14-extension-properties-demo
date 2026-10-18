using System.Linq.Expressions;
using Shop.Model;

namespace Shop.Rules;

public static class OrderRules
{
    // Queries use this.
    public static readonly Expression<Func<Order, bool>> Overdue =
        o => o.PaidOn == null && o.DueDate < DateTime.UtcNow;

    private static readonly Func<Order, bool> overdue = Overdue.Compile();

    // Plain code uses this. Same rule, one place.
    extension(Order o)
    {
        public bool IsOverdue => overdue(o);
    }
}
