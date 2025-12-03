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
            var universalTableName = configuration["GlobalSettings:Tables:Universal"]!;
            var arendaTableName = configuration["GlobalSettings:Tables:ArendaAz"]!;
            var lalafoTableName = configuration["GlobalSettings:Tables:LalafoAz"]!;
            var binaTableName = configuration["GlobalSettings:Tables:BinaAz"]!;
            var tapTableName = configuration["GlobalSettings:Tables:TapAz"]!;
            var evtenTableName = configuration["GlobalSettings:Tables:Ev10Az"]!;
            var yeniEmlakTableName = configuration["GlobalSettings:Tables:YeniEmlak"]!;
            var emlakTableName = configuration["GlobalSettings:Tables:Emlak"]!;
            EnsureTableCreated(universalTableName);
            EnsureTableCreated(arendaTableName);
            EnsureTableCreated(lalafoTableName);
            EnsureTableCreated(binaTableName);
            EnsureTableCreated(tapTableName);
            EnsureTableCreated(evtenTableName);
            EnsureTableCreated(yeniEmlakTableName);
            EnsureTableCreated(emlakTableName);
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

        private static void EnsureTableCreated(string tableName)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                connection.Execute(@$"
                        IF NOT EXISTS (
                            SELECT * FROM INFORMATION_SCHEMA.TABLES 
                            WHERE TABLE_SCHEMA = 'dbo' 
                            AND TABLE_NAME = '{tableName}'
                        )
                        BEGIN
                            CREATE TABLE dbo.{tableName} (
                                id BIGINT IDENTITY(1,1) PRIMARY KEY,
                                bina_id int NULL,
                                main_title NVARCHAR(MAX) NULL,
                                address NVARCHAR(MAX) NULL,
                                area NVARCHAR(MAX) NULL,
                                torpaqarea NVARCHAR(MAX) NULL,
                                amount NVARCHAR(35) NULL,
                                currency NVARCHAR(15) NULL DEFAULT 'AZN',
                                renovation NVARCHAR(15) NULL,
                                [document] NVARCHAR(15) NULL,
                                ipoteka NVARCHAR(15) NULL,
                                binatype NVARCHAR(60) NULL,
                                room NVARCHAR(MAX) NULL,
                                floor NVARCHAR(15) NULL,
                                category NVARCHAR(MAX) NULL,
                                item_id VARCHAR(255) NULL,
                                poster_name NVARCHAR(200) NULL,
                                poster_note NVARCHAR(MAX) NULL,
                                post_tip NVARCHAR(MAX) NULL,
                                poster_type NVARCHAR(MAX) NULL,
                                poster_phone VARCHAR(255) NULL,
                                post_create_date DATETIME NULL,
                                insertdate DATETIME NULL DEFAULT GETDATE(),
                                updated DATETIME NULL DEFAULT GETDATE(),
                                sayt NVARCHAR(30) NULL,
                                sayt_link NVARCHAR(MAX) NULL,
                                imageUrls NVARCHAR(MAX) NULL,
                                status INT NULL,
                                ReferenceId BIGINT NULL
                            );
                        END
                ");
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
