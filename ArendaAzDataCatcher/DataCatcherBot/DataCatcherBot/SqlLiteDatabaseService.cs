using Dapper;
using System.Data.SQLite;
using System.Security.Cryptography.X509Certificates;

namespace DataCatcherBot
{
    public class ArendaRecord
    {
        public string Id { get; set; }
        public DateTime InsertDate { get; set; }
    }
    public class DatabaseService
    {
        private readonly string _connectionString;
        public DatabaseService(string dbPath)
        {
            if (!File.Exists(dbPath))
            {
                SQLiteConnection.CreateFile(dbPath);
            }

            _connectionString = $"Data Source={dbPath};Version=3;";

            using (var connection = new SQLiteConnection(_connectionString))
            {
                connection.Execute(@"
                CREATE TABLE IF NOT EXISTS Records (
                    Id TEXT PRIMARY KEY,
                    InsertDate DATETIME DEFAULT CURRENT_TIMESTAMP
                );");
            }
        }

        public void InsertRecord(string id)
        {
            using var connection = new SQLiteConnection(_connectionString);
            string insertQuery = "INSERT INTO Records (Id) VALUES (@Id);";
            connection.Execute(insertQuery, new { Id = id });
        }

        public ArendaRecord? FindById(string id)
        {
            using var connection = new SQLiteConnection(_connectionString);
            string selectQuery = "SELECT Id, InsertDate FROM Records WHERE Id = @Id;";
            return connection.QuerySingleOrDefault<ArendaRecord>(selectQuery, new { Id = id });
        }
    }
}
