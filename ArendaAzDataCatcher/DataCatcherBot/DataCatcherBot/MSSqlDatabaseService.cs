using Dapper;
using Microsoft.Data.SqlClient;
using System.Text.Json;

namespace DataCatcherBot
{
    public class MSSqlDatabaseService
    {
        private readonly string _databaseName = "ArendaDB"; 
        private readonly string _masterConnectionString = @"Server=(localdb)\MSSQLLocalDB;Database=master;Trusted_Connection=True;";
        private readonly string _connectionString = @"Server=(localdb)\MSSQLLocalDB;Database=ArendaDB;Trusted_Connection=True;";

        public MSSqlDatabaseService()
        {
            EnsureDatabaseAndTable();
        }

        private void EnsureDatabaseAndTable()
        {
            using (var connection = new SqlConnection(_masterConnectionString))
            {
                connection.Open();
                connection.Execute($@"
                    IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = N'{_databaseName}')
                    BEGIN
                        CREATE DATABASE [{_databaseName}];
                    END
                ");
                connection.Close();
            }

            // 2. Ensure table exists inside the database
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                connection.Execute(@"
                    IF NOT EXISTS (SELECT * 
                                   FROM INFORMATION_SCHEMA.TABLES 
                                   WHERE TABLE_SCHEMA = 'dbo' 
                                   AND TABLE_NAME = 'Records')
                    BEGIN
                        CREATE TABLE dbo.Records (
                            Id NVARCHAR(50) NOT NULL PRIMARY KEY,
                            MainTitle NVARCHAR(500) NOT NULL,
                            SecondaryTitle NVARCHAR(500) NULL,
                            Address NVARCHAR(500) NOT NULL,
                            Description NVARCHAR(MAX) NOT NULL,
                            Price FLOAT NOT NULL,
                            PropertySize FLOAT NULL,
                            RoomCount INT NULL,
                            ContactNumbers NVARCHAR(MAX) NULL,
                            Owner NVARCHAR(200) NOT NULL,
                            PropertyFeatures NVARCHAR(MAX) NULL,
                            PropertyMainInfos NVARCHAR(MAX) NULL,
                            Created_At DATETIME NOT NULL DEFAULT GETDATE()
                        );
                    END
                ");
                connection.Close();
            }
        }

        public void InsertRecord(ArendaProperty record)
        {
            using var connection = new SqlConnection(_connectionString);
            string sqlQuery = @"
                INSERT INTO dbo.Records (
                    Id, MainTitle, SecondaryTitle, Address, Description,
                    Price, PropertySize, RoomCount, ContactNumbers, Owner,
                    PropertyFeatures, PropertyMainInfos
                )
                VALUES (
                    @Id, @MainTitle, @SecondaryTitle, @Address, @Description,
                    @Price, @PropertySize, @RoomCount, @ContactNumbers, @Owner,
                    @PropertyFeatures, @PropertyMainInfos
                );";

            var parameters = new
            {
                record.Id,
                record.MainTitle,
                record.SecondaryTitle,
                record.Address,
                record.Description,
                record.Price,
                record.PropertySize,
                record.RoomCount,
                ContactNumbers = record.ContactNumbers != null ? JsonSerializer.Serialize(record.ContactNumbers) : null,
                record.Owner,
                PropertyFeatures = record.PropertyFeatures != null ? JsonSerializer.Serialize(record.PropertyFeatures) : null,
                PropertyMainInfos = record.PropertyMainInfos != null ? JsonSerializer.Serialize(record.PropertyMainInfos) : null
            };

            connection.Execute(sqlQuery, parameters);
        }

        public ArendaRecord? FindById(string id)
        {
            using var connection = new SqlConnection(_connectionString);
            string selectQuery = "SELECT Id FROM Records WHERE Id = @Id;";
            return connection.QuerySingleOrDefault<ArendaRecord>(selectQuery, new { Id = id });
        }
    }
}
