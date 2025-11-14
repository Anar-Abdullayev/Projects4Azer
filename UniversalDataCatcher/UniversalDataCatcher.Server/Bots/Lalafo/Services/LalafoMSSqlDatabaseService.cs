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
            _tableName = MSSqlDatabaseService.GetTableName("ArendaAz");
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
                    bina_id, main_title, address, poster_note,
                    amount, area, room, poster_phone, poster_name, sayt, item_id, post_create_date, sayt_link,
                    binatype, category, floor, post_tip, torpaqarea, document, renovation, poster_type
                )
                VALUES (
                    @Id, @MainTitle, @Address, @Description,
                    @Price, @PropertySize, @RoomCount, @ContactNumbers, @Owner,
                    'ArendaAz', @Id, @Created_At, @Link, @SecondaryTitle, @SecondaryTitle, @Floor, @Post_Type, @TorpaqArea, @Document, @Repair, @Poster_Type
                );";

            var parameters = new
            {
                record.Id,
                record.Title,
            };

            connection.Execute(sqlQuery, parameters);
        }
    }
}
