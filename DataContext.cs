using Dapper;
using Microsoft.Data.Sqlite;

namespace MapleRoyalsPlayerCount;

public static class DataContext
{
    public static string DbFile => "./mapleroyals.db";

    public static SqliteConnection GetSqliteConnection()
    {
        return new SqliteConnection("Data Source=" + DbFile);
    }

    public static async Task InitializeDatabase()
    {
        await using var cnn = GetSqliteConnection();
        await cnn.OpenAsync();
        await cnn.ExecuteAsync(@"CREATE TABLE IF NOT EXISTS Players (
        id         integer primary key AUTOINCREMENT,
        players    int not null,
        server_online     bool not null,
        created_at  datetime DEFAULT CURRENT_TIMESTAMP
    )");
        cnn.Close();
    }
}