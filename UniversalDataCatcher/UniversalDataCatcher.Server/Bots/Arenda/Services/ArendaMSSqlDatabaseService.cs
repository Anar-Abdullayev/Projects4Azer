using Dapper;
using Microsoft.Data.SqlClient;
using System.Text.Json;
using UniversalDataCatcher.Server.Interfaces;
using UniversalDataCatcher.Server.Services.Arenda.DTOs;
using UniversalDataCatcher.Server.Services.Arenda.Model;

namespace UniversalDataCatcher.Server.Services.Arenda.Services
{
    public class ArendaMSSqlDatabaseService : IMSSqlDatabaseService<ArendaProperty>
    {
        private string _connectionString = "";
        private string _tableName = "";

        public ArendaMSSqlDatabaseService()
        {
            _connectionString = MSSqlDatabaseService.GetConnectionString();
            _tableName = MSSqlDatabaseService.GetTableName("ArendaAz");
        }

        public ArendaProperty? FindById(string id)
        {
            using var connection = new SqlConnection(_connectionString);
            string selectQuery = $"SELECT Id FROM {_tableName} WHERE Id = @Id;";
            var record = connection.QuerySingleOrDefault<ArendaRecord>(selectQuery, new { Id = id });
            if (record == null)
                return null;
            return new ArendaProperty { Id = record.Id };
        }

        public void InsertRecord(ArendaProperty record)
        {
            using var connection = new SqlConnection(_connectionString);
            string sqlQuery = @$"
                INSERT INTO dbo.{_tableName} (
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
    }
}
