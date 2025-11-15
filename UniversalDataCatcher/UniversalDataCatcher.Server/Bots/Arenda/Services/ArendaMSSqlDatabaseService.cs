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

        public ArendaProperty? FindById(int id)
        {
            using var connection = new SqlConnection(_connectionString);
            string selectQuery = $"SELECT bina_id, sayt_link FROM {_tableName} WHERE bina_id = @Id AND sayt = 'ArendaAz';";
            var record = connection.QuerySingleOrDefault<ArendaRecord>(selectQuery, new { Id = id});
            if (record == null)
                return null;
            return new ArendaProperty { Id = record.bina_id, Link = record.sayt_link };
        }

        public void InsertRecord(ArendaProperty record)
        {
            using var connection = new SqlConnection(_connectionString);
            string sqlQuery = @$"
                INSERT INTO dbo.{_tableName} (
                    bina_id, main_title, address, poster_note,
                    amount, area, room, poster_phone, poster_name, sayt, item_id, post_create_date, sayt_link,
                    binatype, category, floor, post_tip, torpaqarea, document, renovation, poster_type
                )
                VALUES (
                    @ElanId, @MainTitle, @Address, @Description,
                    @Price, @PropertySize, @RoomCount, @ContactNumbers, @Owner,
                    'ArendaAz', @Id, @Created_At, @Link, @SecondaryTitle, @SecondaryTitle, @Floor, @Post_Type, @TorpaqArea, @Document, @Repair, @Poster_Type
                );";

            var elanId = record.Id.Replace("elan_", "");
            var parameters = new
            {
                ElanId = elanId,
                record.Id,
                record.MainTitle,
                record.SecondaryTitle,
                record.Address,
                record.Description,
                record.Price,
                record.PropertySize,
                record.RoomCount,
                record.Owner,
                record.Link,
                record.Floor,
                record.Post_Type,
                record.ContactNumbers,
                record.TorpaqArea,
                record.Document,
                record.Repair,
                record.Poster_Type,
                PropertyFeatures = record.PropertyFeatures != null ? JsonSerializer.Serialize(record.PropertyFeatures) : null,
                PropertyMainInfos = record.PropertyMainInfos != null ? JsonSerializer.Serialize(record.PropertyMainInfos) : null,
                Created_At = record.Created_At.ToString(),
            };

            connection.Execute(sqlQuery, parameters);
        }
    }
}
