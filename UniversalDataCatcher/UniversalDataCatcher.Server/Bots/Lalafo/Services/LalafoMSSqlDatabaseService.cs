using Dapper;
using Microsoft.Data.SqlClient;
using System.Text.Json;
using UniversalDataCatcher.Server.Bots.Lalafo.Models;
using UniversalDataCatcher.Server.Interfaces;
using UniversalDataCatcher.Server.Services;
using UniversalDataCatcher.Server.Services.Arenda.DTOs;
using UniversalDataCatcher.Server.Services.Arenda.Model;

namespace UniversalDataCatcher.Server.Bots.Lalafo.Services
{
    public class LalafoMSSqlDatabaseService : IMSSqlDatabaseService<LalafoProperty>
    {
        private string _connectionString = "";
        private string _tableName = "";
        public LalafoMSSqlDatabaseService()
        {
            _connectionString = MSSqlDatabaseService.GetConnectionString();
            _tableName = MSSqlDatabaseService.GetTableName("LalafoAz");
        }
        public LalafoProperty? FindById(int id)
        {
            using var connection = new SqlConnection(_connectionString);
            string selectQuery = $"SELECT bina_id FROM {_tableName} WHERE bina_id = @Id;";
            var record = connection.QuerySingleOrDefault<LalafoProperty>(selectQuery, new { Id = id });
            if (record == null)
                return null;
            return new LalafoProperty { Id = record.Id };
        }

        public void InsertRecord(LalafoProperty record)
        {
            using var connection = new SqlConnection(_connectionString);
            string sqlQuery = @$"
                INSERT INTO dbo.{_tableName} (
                    bina_id, main_title, address, poster_note, amount, area, room, poster_phone, poster_name, sayt, 
                    item_id, post_create_date, sayt_link,
                    binatype, category, floor, post_tip, torpaqarea, document, renovation, poster_type
                )
                VALUES (
                    @Id, @Title, @Address, @Description, @Price, @PropertyArea, @RoomCount, @Mobile, @Username, 'LalafoAz', 
                    @Id, @CreatedTime, @Url, @BuildingType, @BuildingType, @Floor, @Post_Type, @LandArea, @Document, @Repair, @Poster_Type
                );";

            
            var parameters = new
            {
                record.Id,
                record.Title,
                record.Address,
                record.Description,
                record.Price,
                record.PropertyArea,
                record.RoomCount,
                record.Mobile,
                record.Username,
                record.Url,
                record.BuildingType,
                record.Floor,
                record.Post_Type,
                record.LandArea,
                record.Document,
                record.Repair,
                record.Poster_Type,
                CreatedTime = DateTimeOffset.FromUnixTimeSeconds(record.CreatedTime).ToString(),
            };

            connection.Execute(sqlQuery, parameters);
        }
    }
}
