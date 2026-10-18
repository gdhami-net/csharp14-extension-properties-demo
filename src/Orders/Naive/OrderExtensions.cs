using Shop.Model;

namespace Shop.Naive;

// The C# 14 extension property, written the obvious way.
public static class OrderExtensions
{
    extension(Order o)
    {
        public bool IsOverdue => o.PaidOn is null && o.DueDate < DateTime.UtcNow;
    }
}
