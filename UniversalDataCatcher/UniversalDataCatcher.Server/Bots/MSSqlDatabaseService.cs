using Dapper;
using Microsoft.Data.SqlClient;

namespace UniversalDataCatcher.Server.Services
{
    public static class MSSqlDatabaseService
    {
        private static string _masterConnectionString;
        private static string _connectionString;
        private static Dictionary<string, string> _tables;
        public static void Initialize(IConfiguration configuration)
        {
            _masterConnectionString = configuration.GetConnectionString("BaseConnectionString")!;
            _connectionString = _masterConnectionString;
            var serverName = configuration["GlobalSettings:ServerHost"];
            var _databaseName = configuration["GlobalSettings:Database"];
            _tables = configuration.GetSection("GlobalSettings:Tables").Get<Dictionary<string, string>>()!;
            if (string.IsNullOrEmpty(_databaseName) || string.IsNullOrEmpty(serverName))
                throw new Exception("Servername or database name for connection is missed");

            _connectionString = _connectionString.Replace("XXXSERVERNAMEXXX", serverName).Replace("XXXDATABASENAMEXXX", _databaseName);
            _masterConnectionString = _masterConnectionString.Replace("XXXSERVERNAMEXXX", serverName).Replace("XXXDATABASENAMEXXX", "master");

            if (string.IsNullOrEmpty(_masterConnectionString))
                throw new Exception("Empty connection string");

            EnsureDatabaseCreated(_databaseName);
            var arendaTableName = configuration["GlobalSettings:Tables:ArendaAz"]!;
            EnsureArendaTableCreated(arendaTableName);
        }
        private static void EnsureDatabaseCreated(string _databaseName)
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
        }
        private static void EnsureArendaTableCreated(string tableName)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                connection.Execute($@"
                    IF NOT EXISTS (SELECT * 
                                   FROM INFORMATION_SCHEMA.TABLES 
                                   WHERE TABLE_SCHEMA = 'dbo' 
                                   AND TABLE_NAME = '{tableName}')
                    BEGIN
                        CREATE TABLE dbo.{tableName} (
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

        public static string GetConnectionString()
        {
            return _connectionString;
        }
        public static string GetTableName(string tableKey)
        {
            if (_tables.ContainsKey(tableKey))
                return _tables[tableKey];
            throw new Exception($"{tableKey} not found in configuration");
        }
    }
}
