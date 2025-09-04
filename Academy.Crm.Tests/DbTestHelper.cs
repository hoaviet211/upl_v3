using Academy.Crm.Infra.Db;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace Academy.Crm.Tests;

public static class DbTestHelper
{
    public static AppDbContext CreateSqliteInMemory()
    {
        var connection = new SqliteConnection("DataSource=:memory:");
        connection.Open();
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlite(connection)
            .Options;
        var ctx = new AppDbContext(options);
        ctx.Database.EnsureCreated();
        return ctx;
    }
}

