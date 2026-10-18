using System.Reflection;
using System.Text.Json;
using Shop.Model;
using Shop.Naive;

namespace Shop.Checks;

// Everything here reads the extension property from Shop.Naive.
public static class PlainCode
{
    public static IReadOnlyList<string> OverdueFlags(ShopDb db) =>
        db.Orders.OrderBy(o => o.Id).ToList()
            .Select(o => $"{o.Customer}: IsOverdue = {o.IsOverdue}")
            .ToList();

    public static string Json(Order order) => JsonSerializer.Serialize(order);

    public static PropertyInfo? GetPropertyOnOrder() => typeof(Order).GetProperty("IsOverdue");

    public static IReadOnlyList<string> PropertiesOnOrder() =>
        typeof(Order).GetProperties().Select(p => p.Name).ToList();

    // Where the compiler really put the property: a static getter method on
    // the class that holds the extension block, taking the Order as a parameter.
    public static IReadOnlyList<string> StaticMethodsOnExtensionClass() =>
        typeof(OrderExtensions)
            .GetMethods(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly)
            .Select(m => $"{m.ReturnType.Name} {m.Name}({string.Join(", ", m.GetParameters().Select(p => p.ParameterType.Name))})")
            .ToList();

    public static bool CallGetterThroughReflection(Order order) =>
        (bool)typeof(OrderExtensions).GetMethod("get_IsOverdue")!.Invoke(null, [order])!;

    // The compiler also emits nested marker types that describe the extension
    // block for other compilers. Listed here so the README can say what they are.
    public static IReadOnlyList<string> NestedTypesOnExtensionClass() =>
        typeof(OrderExtensions)
            .GetNestedTypes(BindingFlags.Public | BindingFlags.NonPublic)
            .Select(t => $"{t.Name} [properties: {string.Join(", ", t.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly).Select(p => p.Name))}]")
            .ToList();
}
