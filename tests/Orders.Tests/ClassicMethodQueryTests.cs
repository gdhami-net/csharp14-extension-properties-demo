using Shop.Checks;
using Shop.Model;

namespace Orders.Tests;

public class ClassicMethodQueryTests
{
    [Fact]
    public void The_classic_method_compiles_in_a_query_and_throws_when_it_runs()
    {
        using var db = ShopDb.CreateSeeded();

        var ex = Assert.Throws<InvalidOperationException>(() => ClassicQuery.OverdueOrders(db));

        Assert.StartsWith("The LINQ expression 'DbSet<Order>()", ex.Message);
        Assert.Contains(".IsOverdue())' could not be translated.", ex.Message);
        Assert.Contains("Either rewrite the query in a form that can be translated, or switch to client evaluation explicitly", ex.Message);
    }
}
