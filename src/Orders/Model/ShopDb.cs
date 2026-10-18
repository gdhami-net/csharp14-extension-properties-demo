using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace Shop.Model;

public class ShopDb(DbContextOptions<ShopDb> options) : DbContext(options)
{
    public DbSet<Order> Orders => Set<Order>();

    // An in-memory SQLite database with three orders: one overdue (Ada), one
    // paid (Brahim) and one due in 2099 (Chen). The dates are far enough from
    // today that the answers don't depend on when the demo runs.
    public static ShopDb CreateSeeded()
    {
        var connection = new SqliteConnection("Data Source=:memory:");
        connection.Open();
        var db = new ShopDb(new DbContextOptionsBuilder<ShopDb>().UseSqlite(connection).Options);
        db.Database.EnsureCreated();
        db.Orders.AddRange(
            new Order { Id = 1, Customer = "Ada", DueDate = new DateTime(2020, 1, 31) },
            new Order { Id = 2, Customer = "Brahim", DueDate = new DateTime(2020, 1, 31), PaidOn = new DateTime(2020, 1, 15) },
            new Order { Id = 3, Customer = "Chen", DueDate = new DateTime(2099, 12, 31) });
        db.SaveChanges();
        db.ChangeTracker.Clear();
        return db;
    }
}
