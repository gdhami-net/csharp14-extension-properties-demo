# csharp14-extension-properties-demo

Companion demo for the post
[Use C# 14 extension properties only where your own code reads them](https://gdhami.net/blog/use-csharp-14-extension-properties-only-where-your-own-code-reads-them).

One rule, "an order is overdue when it's unpaid and past its due date", written
three ways and tried in four places.

## What it proves

| Where | The C# 14 extension property `order.IsOverdue` | The classic extension method `order.IsOverdue()` |
|---|---|---|
| Plain C# code | works like a property | works |
| `db.Orders.Where(...)` in EF Core 10 | **does not compile**: `error CS9296: An expression tree may not contain an extension property access` | compiles with zero warnings, **throws** `InvalidOperationException: The LINQ expression ... could not be translated` when the query runs |
| `JsonSerializer.Serialize(order)` | not in the output, no warning | (methods are never serialized) |
| `typeof(Order).GetProperty("IsOverdue")` | `null`; `GetProperties()` lists only Id, Customer, DueDate, PaidOn | n/a |

Also:

- The property really lives on the extension class as a static method,
  `Boolean get_IsOverdue(Order)`, which can be called through reflection.
- When a type gets a real `IsOverdue` (and a real `IsDue()`) next to extensions
  with the same names, the build has zero warnings and the real members win.
- The fix the post recommends: keep the rule as an
  `Expression<Func<Order, bool>>` (`Shop.Rules.OrderRules.Overdue`) for queries,
  and build the extension property from the compiled expression. The query and
  the property both return the same orders.

## Run it

```bash
./check.sh                       # every assertion; prints ALL CHECKS PASSED
dotnet run --project src/Orders  # the output the post quotes
dotnet test tests/Orders.Tests
```

`check.sh` needs bash (Git Bash on Windows works). It builds
`probes/ExpressionTree`, which must fail with exactly one CS9296, builds and
runs `probes/Shadowing`, runs the console app and the tests.

## Layout

- `src/Orders/Model` - the `Order` entity and a seeded in-memory SQLite `ShopDb`
  (Ada: overdue since 2020; Brahim: paid; Chen: due in 2099, so the answers
  don't depend on the day you run it).
- `src/Orders/Naive` - the extension property, written the obvious way.
- `src/Orders/Classic` - the same rule as a classic extension method.
- `src/Orders/Rules` - the expression plus an extension property built from it.
- `src/Orders/Checks` - one file per experiment, each importing exactly one of
  the three namespaces above.
- `probes/ExpressionTree` - the query that must not compile.
- `probes/Shadowing` - a real member next to a same-name extension.

## Versions

- .NET SDK 10.0.201 (pinned in `global.json`, `latestPatch` roll-forward);
  C# 14 is the default language version for `net10.0`, and no project sets
  `LangVersion`.
- Microsoft.EntityFrameworkCore.Sqlite 10.0.13
- System.Text.Json from the .NET 10 shared framework
- xUnit 2.9.3, Microsoft.NET.Test.Sdk 18.11.0, xunit.runner.visualstudio 3.1.5
- Run on Windows 11.

## Edge cases the post leaves out

- **The marker type.** The compiler also emits a nested type with a generated
  name (here `<G>$EB2CDBA6E04E1EB04EB4291942EB4D03`) that declares an
  `IsOverdue` property. It describes the extension block for other compilers
  (see "Extension marker types" in the feature spec). Reflection on `Order`
  doesn't look there; listing the nested types of `OrderExtensions` finds it.
- **CS9296 is not about EF Core.** It's a compiler rule for any lambda converted
  to an `Expression<T>`, so a `Select` projection or any other LINQ provider
  hits it too.
- **The EF Core message** for the classic method prints the expression and
  "could not be translated". It doesn't add a "Translation of method ... failed"
  line in this case.
- **The SQL** for the expression version on SQLite compares `DueDate` with
  `rtrim(rtrim(strftime('%Y-%m-%d %H:%M:%f', 'now'), '0'), '.')`, which is
  EF Core's translation of `DateTime.UtcNow`. SQL Server would use its own
  function.
- **A custom `JsonConverter`** can of course write the value; the default
  serializer just doesn't know it exists.
- **Static extension members and extension operators** (also C# 14) are not
  covered. Extension indexers are C# 15 (.NET 11), not C# 14.
- **A computed column** is the other way to make the rule queryable. Not built
  here; the post explains why it isn't the pick.

## License

MIT, see [LICENSE](LICENSE).
