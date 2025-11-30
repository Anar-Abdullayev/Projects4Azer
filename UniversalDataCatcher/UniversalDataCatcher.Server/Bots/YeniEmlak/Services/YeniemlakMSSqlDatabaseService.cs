using Dapper;
using Microsoft.Data.SqlClient;
using UniversalDataCatcher.Server.Bots.EvTen.Models;
using UniversalDataCatcher.Server.Bots.YeniEmlak.Models;
using UniversalDataCatcher.Server.Services;

namespace UniversalDataCatcher.Server.Bots.YeniEmlak.Services
{
    public class YeniemlakMSSqlDatabaseService
    {
        private readonly string _connectionString;
        private readonly string _databaseTableName;
        public YeniemlakMSSqlDatabaseService()
        {
            _connectionString = MSSqlDatabaseService.GetConnectionString();
            _databaseTableName = MSSqlDatabaseService.GetTableName("YeniEmlak");
        }

        public void InsertRecord(YeniEmlakProperty record)
        {

            using var connection = new SqlConnection(_connectionString);
            var dateNow = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            string sqlQuery = @$"
                INSERT INTO dbo.{_databaseTableName} (
                    bina_id, main_title, address, poster_note, amount, area, room, poster_phone, poster_name, sayt, 
                    item_id, post_create_date, sayt_link,
                    binatype, category, floor, post_tip, torpaqarea, document, renovation, poster_type, imageUrls
                )

                VALUES (
                    @Id, @MainTitle, @Address, @Description, @Price, @Area, @Rooms, @PosterPhone, @PosterName, 'YeniEmlak',
                    @Id, @CreatedAt, @AdvLink, @Category, @BinaType, @Floor, @PostType, @LandArea, @Document, @Renovation, @PosterType, @ImageUrls
                );";
            var parameters = new
            {
                record.Id,
                record.Address,
                record.Description,
                record.Price,
                record.Area,
                record.Rooms,
                record.PosterPhone,
                record.PosterName,
                CreatedAt = record.CreatedAt.ToString(),
                record.AdvLink,
                record.BinaType,
                record.Floor,
                record.PosterType,
                record.Renovation,
                record.PostType,
                record.Document,
                record.LandArea,
                record.Category,
                MainTitle = record.PostType + " " + record.Address,
                record.ImageUrls
            };
            connection.Execute(sqlQuery, parameters);
        }

        public YeniEmlakProperty? FindById(int id)
        {
            using var connection = new SqlConnection(_connectionString);
            string selectQuery = $"SELECT * FROM dbo.{_databaseTableName} WHERE bina_id = @Id AND sayt = 'YeniEmlak';";
            return connection.QuerySingleOrDefault<YeniEmlakProperty>(selectQuery, new { Id = id });
        }
    }
}
