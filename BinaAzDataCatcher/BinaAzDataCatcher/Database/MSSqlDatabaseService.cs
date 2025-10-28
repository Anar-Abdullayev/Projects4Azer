using BinaAzDataCatcher.Models;
using Dapper;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;

namespace BinaAzDataCatcher.Database
{
    public class MSSqlDatabaseService
    {
        private static readonly string _mssqlServerName = "(localdb)\\MSSQLLocalDB";
        private static readonly string _databaseName = "BinaazDb";
        private static readonly string _databaseTableName = "records";
        private readonly string _masterConnectionString = @$"Server={_mssqlServerName};Database=master;Trusted_Connection=True;";
        private readonly string _connectionString = @$"Server={_mssqlServerName};Database={_databaseName};Trusted_Connection=True;";

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
                                main_title NVARCHAR(MAX) NULL,
                                address NVARCHAR(MAX) NULL,
                                amount NVARCHAR(35) NULL,
                                currency NVARCHAR(15) NULL,
                                repair NVARCHAR(15) NULL,
                                cixaris NVARCHAR(15) NULL,
                                ipoteka NVARCHAR(15) NULL,
                                building_type NVARCHAR(60) NULL,
                                area NVARCHAR(MAX) NULL,
                                landarea NVARCHAR(MAX) NULL,
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
                                floor NVARCHAR(15) NULL,
                                updated DATETIME NULL,
                                sayt NVARCHAR(30) NULL,
                                sayt_link NVARCHAR(MAX) NULL
                            );
                        END
                ");
            }
        }
        public void InsertRecord(BinaAzProperty record)
        {

            using var connection = new SqlConnection(_connectionString);
            var dateNow = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            string sqlQuery = @$"
                INSERT INTO dbo.{_databaseTableName} (
                main_title, address, amount, currency, repair, cixaris, ipoteka, building_type, area, landarea, bina_id, category, insertdate, item_id, 
                poster_name, poster_note, post_tip, poster_type, poster_phone, room, floor, updated, sayt, sayt_link
                )

                VALUES (
                    @MainTitle, @FullAddress, @Price, @Currency, @Repair, @Cixaris, @Ipoteka, @BuildingType, @Area, @LandArea, @Id, @Category, '{dateNow}',
                    @Id, @Owner, @Description, @RentLong,
                    @OwnerType, @PhoneNumbers, @RoomCount, @Floor, @UpdatedTime, 'BinaAz', @AdvLink
                );";

            connection.Execute(sqlQuery, record);
        }

        public BinaAzProperty? FindById(int id)
        {
            using var connection = new SqlConnection(_connectionString);
            string selectQuery = $"SELECT * FROM dbo.{_databaseTableName} WHERE bina_id = @Id;";
            return connection.QuerySingleOrDefault<BinaAzProperty>(selectQuery, new { Id = id });
        }
    }
}
