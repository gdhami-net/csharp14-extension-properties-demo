using System.Text.Json;
using Shop.Checks;
using Shop.Model;
using Shop.Naive;

namespace Orders.Tests;

public class ExtensionPropertyTests
{
    [Fact]
    public void Plain_code_reads_it_like_a_real_property()
    {
        using var db = ShopDb.CreateSeeded();
        Assert.Equal(
            ["Ada: IsOverdue = True", "Brahim: IsOverdue = False", "Chen: IsOverdue = False"],
            PlainCode.OverdueFlags(db));
    }

    [Fact]
    public void System_Text_Json_does_not_write_it()
    {
        using var db = ShopDb.CreateSeeded();
        var ada = db.Orders.Single(o => o.Customer == "Ada");
        Assert.True(ada.IsOverdue);

        var json = PlainCode.Json(ada);

        Assert.Equal("""{"Id":1,"Customer":"Ada","DueDate":"2020-01-31T00:00:00","PaidOn":null}""", json);
        Assert.DoesNotContain("IsOverdue", json);
        Assert.Equal(["Id", "Customer", "DueDate", "PaidOn"], JsonDocument.Parse(json).RootElement.EnumerateObject().Select(p => p.Name));
    }

    [Fact]
    public void Reflection_on_Order_does_not_list_it()
    {
        Assert.Null(PlainCode.GetPropertyOnOrder());
        Assert.Equal(["Id", "Customer", "DueDate", "PaidOn"], PlainCode.PropertiesOnOrder());
    }

    [Fact]
    public void It_lives_as_a_static_getter_on_the_extension_class()
    {
        Assert.Equal(["Boolean get_IsOverdue(Order)"], PlainCode.StaticMethodsOnExtensionClass());

        using var db = ShopDb.CreateSeeded();
        var ada = db.Orders.Single(o => o.Customer == "Ada");
        Assert.True(PlainCode.CallGetterThroughReflection(ada));
    }

    [Fact]
    public void The_compiler_emits_one_nested_marker_type_that_names_the_property()
    {
        var nested = Assert.Single(PlainCode.NestedTypesOnExtensionClass());
        Assert.StartsWith("<G>$", nested);
        Assert.EndsWith("[properties: IsOverdue]", nested);
    }
}
