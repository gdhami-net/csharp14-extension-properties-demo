using Shop.Model;

namespace Shop.Classic;

// The same rule as an old-style extension method, for comparison.
public static class OrderExtensions
{
    public static bool IsOverdue(this Order o) =>
        o.PaidOn is null && o.DueDate < DateTime.UtcNow;
}
