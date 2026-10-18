using Shop.Checks;
using Shop.Model;

namespace Orders.Tests;

public class RulesQueryTests
{
    [Fact]
    public void The_expression_runs_in_the_database()
    {
        using var db = ShopDb.CreateSeeded();
        Assert.Equal(["Ada"], RulesQuery.OverdueFromDatabase(db));
    }

    [Fact]
    public void The_extension_property_built_from_it_gives_the_same_answer_in_memory()
    {
        using var db = ShopDb.CreateSeeded();
        Assert.Equal(RulesQuery.OverdueFromDatabase(db), RulesQuery.OverdueInMemory(db));
    }

    [Fact]
    public void The_rule_becomes_a_where_clause_in_the_sql()
    {
        using var db = ShopDb.CreateSeeded();
        var sql = RulesQuery.Sql(db);
        Assert.Contains("""WHERE "o"."PaidOn" IS NULL AND "o"."DueDate" < """, sql);
        Assert.Contains("'now'", sql);
    }
}
