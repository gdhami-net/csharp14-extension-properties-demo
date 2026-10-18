using Shop.Checks;
using Shop.Model;

// Prints what each check sees. The tests assert the same things; this is the
// output the blog post quotes.
using var db = ShopDb.CreateSeeded();

Console.WriteLine("== 1. Plain code: order.IsOverdue");
foreach (var line in PlainCode.OverdueFlags(db)) Console.WriteLine(line);

Console.WriteLine();
Console.WriteLine("== 2. Classic extension method in a query: db.Orders.Where(o => o.IsOverdue())");
var ex = ClassicQuery.Run(db);
Console.WriteLine(ex is null ? "no exception" : $"{ex.GetType().FullName}: {ex.Message}");

Console.WriteLine();
Console.WriteLine("== 3. System.Text.Json: JsonSerializer.Serialize(ada)");
var ada = db.Orders.Single(o => o.Customer == "Ada");
Console.WriteLine(PlainCode.Json(ada));

Console.WriteLine();
Console.WriteLine("== 4. Reflection");
Console.WriteLine($"typeof(Order).GetProperty(\"IsOverdue\") is null: {PlainCode.GetPropertyOnOrder() is null}");
Console.WriteLine($"typeof(Order).GetProperties(): {string.Join(", ", PlainCode.PropertiesOnOrder())}");
Console.WriteLine("public static methods on Shop.Naive.OrderExtensions:");
foreach (var m in PlainCode.StaticMethodsOnExtensionClass()) Console.WriteLine($"  {m}");
Console.WriteLine($"get_IsOverdue(ada) through reflection: {PlainCode.CallGetterThroughReflection(ada)}");
Console.WriteLine("nested types on Shop.Naive.OrderExtensions:");
foreach (var t in PlainCode.NestedTypesOnExtensionClass()) Console.WriteLine($"  {t}");

Console.WriteLine();
Console.WriteLine("== 5. The rule as an expression: db.Orders.Where(OrderRules.Overdue)");
Console.WriteLine($"from the database: {string.Join(", ", RulesQuery.OverdueFromDatabase(db))}");
Console.WriteLine($"in memory, order.IsOverdue: {string.Join(", ", RulesQuery.OverdueInMemory(db))}");
Console.WriteLine(RulesQuery.Sql(db));
