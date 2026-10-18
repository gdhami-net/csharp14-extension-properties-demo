// Version 2 of a type you don't own adds a real IsOverdue and a real
// IsDue(), and your extensions with the same names are still in scope.
var invoice = new Invoice { DueDate = new DateTime(2020, 1, 31) };
Console.WriteLine($"invoice.IsOverdue = {invoice.IsOverdue}");
Console.WriteLine($"invoice.IsDue() = {invoice.IsDue()}");

public class Invoice
{
    public DateTime DueDate { get; set; }

    // The library's own rules: never overdue, never due (it waits for a reminder run).
    public bool IsOverdue => false;
    public bool IsDue() => false;
}

public static class InvoiceExtensions
{
    extension(Invoice i)
    {
        // Your rule from before the library added its property.
        public bool IsOverdue => i.DueDate < DateTime.UtcNow;
    }

    // The same situation with an old-style extension method.
    public static bool IsDue(this Invoice i) => i.DueDate < DateTime.UtcNow;
}
