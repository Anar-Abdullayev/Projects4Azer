using Dapper;
using Microsoft.Data.SqlClient;
using UniversalDataCatcher.Server.Bots.EvTen.Models;
using UniversalDataCatcher.Server.Bots.EvTen.StaticConstants;
using UniversalDataCatcher.Server.Bots.Tap.Models;
using UniversalDataCatcher.Server.Services;

namespace UniversalDataCatcher.Server.Bots.EvTen.Services
{
    public class EvTenMSSqlDatabaseService
    {
        private readonly string _connectionString;
        private readonly string _databaseTableName;
        public EvTenMSSqlDatabaseService()
        {
            _connectionString = MSSqlDatabaseService.GetConnectionString();
            _databaseTableName = MSSqlDatabaseService.GetTableName("Ev10Az");
        }

        public void InsertRecord(EvTenPropertyDetails record)
        {

            using var connection = new SqlConnection(_connectionString);
            var dateNow = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            string sqlQuery = @$"
                INSERT INTO dbo.{_databaseTableName} (
                    bina_id, main_title, address, poster_note, amount, area, room, poster_phone, poster_name, sayt, 
                    item_id, post_create_date, sayt_link,
                    binatype, category, floor, post_tip, torpaqarea, document, renovation, poster_type
                )

                VALUES (
                    @Id, null, @Address, @Description, @Price, @Area, @Rooms, @PhoneNumber, @OwnerName, 'Ev10',
                    @Id, @CreatedAt, @AdvLink, @PropertyType, null, @Floor, @PostType, @LandArea, @Document, @Renovation, @PosterType
                );";
            var parameters = new
            {
                record.Id,
                Address = $"{record.City}, {record.District}, {record.Address}",
                record.Description,
                record.Price,
                record.Area,
                record.Rooms,
                record.PhoneNumber,
                record.OwnerName,
                CreatedAt = record.RenewedAt.ToString(),
                record.AdvLink,
                record.PropertyType,
                record.Floor,
                record.PosterType,
                record.Renovation,
                record.PostType,
                record.Document,
                record.LandArea
            };
            connection.Execute(sqlQuery, parameters);
        }

        public EvTenPropertyDetails? FindById(int id)
        {
            using var connection = new SqlConnection(_connectionString);
            string selectQuery = $"SELECT * FROM dbo.{_databaseTableName} WHERE bina_id = @Id AND sayt = 'Ev10';";
            return connection.QuerySingleOrDefault<EvTenPropertyDetails>(selectQuery, new { Id = id });
        }
    }
}
