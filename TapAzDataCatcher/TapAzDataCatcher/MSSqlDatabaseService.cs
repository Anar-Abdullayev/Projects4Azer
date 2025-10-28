using Dapper;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using TapAzDataCatcher;

namespace DataCatcherBot
{
    public class MSSqlDatabaseService
    {
        private static readonly string _mssqlServerName = "(localdb)\\MSSQLLocalDB";
        private static readonly string _databaseName = "TapazDb";
        private static readonly string _databaseTableName = "post";
        private readonly string _masterConnectionString = @$"Server={_mssqlServerName};Database=master;Trusted_Connection=True;";
        private readonly string _connectionString = @$"Server=(localdb)\MSSQLLocalDB;Database={_databaseName};Trusted_Connection=True;";

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
            }

            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                connection.Execute(@$"
                        IF NOT EXISTS (
                            SELECT * FROM INFORMATION_SCHEMA.TABLES 
                            WHERE TABLE_SCHEMA = 'dbo' 
                            AND TABLE_NAME = '{_databaseTableName}'
                        )
                        BEGIN
                            CREATE TABLE dbo.{_databaseTableName} (
                                id BIGINT IDENTITY(1,1) PRIMARY KEY,
                                address NVARCHAR(MAX) NULL,
                                amount VARCHAR(255) NULL,
                                area NVARCHAR(MAX) NULL,
                                bina_id INT NULL,
                                category NVARCHAR(MAX) NULL,
                                insertdate VARCHAR(255) NULL,
                                item_id VARCHAR(255) NULL,
                                poster_name NVARCHAR(200) NULL,
                                poster_note NVARCHAR(MAX) NULL,
                                post_tip NVARCHAR(MAX) NULL,
                                poster_type NVARCHAR(MAX) NULL,
                                poster_phone VARCHAR(255) NULL,
                                room NVARCHAR(MAX) NULL,
                                updated VARCHAR(255) NULL,
                                sayt NVARCHAR(30) NULL,
                                sayt_link NVARCHAR(MAX) NULL
                            );
                        END
                ");
            }
        }
        public void InsertRecord(TapAzProperty record)
        {

            using var connection = new SqlConnection(_connectionString);
            var dateNow = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            string sqlQuery = @$"
                INSERT INTO dbo.{_databaseTableName} (
                address, amount, area, bina_id, category, insertdate, item_id, 
                poster_name, poster_note, post_tip, poster_type, poster_phone, room, updated, sayt, sayt_link
                )

                VALUES (
                    @MainTitle, @Price, @Area, @Id, @Category, '{dateNow}',
                    @Id, @Owner, @Description, @AdvType,
                    @OwnerType, @PhoneNumbers, @RoomCount, '{dateNow}', 'TapAz', @AdvLink
                );";

            connection.Execute(sqlQuery, record);
        }

        public TapAzProperty? FindById(int id)
        {
            using var connection = new SqlConnection(_connectionString);
            string selectQuery = $"SELECT * FROM dbo.{_databaseTableName} WHERE bina_id = @Id;";
            return connection.QuerySingleOrDefault<TapAzProperty>(selectQuery, new { Id = id });
        }
    }
}
