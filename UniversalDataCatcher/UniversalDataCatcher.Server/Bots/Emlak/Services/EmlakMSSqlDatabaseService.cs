using Dapper;
using Microsoft.Data.SqlClient;
using UniversalDataCatcher.Server.Bots.Emlak.Models;
using UniversalDataCatcher.Server.Bots.YeniEmlak.Models;
using UniversalDataCatcher.Server.Services;

namespace UniversalDataCatcher.Server.Bots.Emlak.Services
{
    public class EmlakMSSqlDatabaseService
    {
        private readonly string _connectionString;
        private readonly string _databaseTableName;
        public EmlakMSSqlDatabaseService()
        {
            _connectionString = MSSqlDatabaseService.GetConnectionString();
            _databaseTableName = MSSqlDatabaseService.GetTableName("Emlak");
        }

        public void InsertRecord(EmlakProperty record)
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
                    @Id, @MainTitle, @Address, @Description, @Price, @Area, @Rooms, @PosterPhone, @PosterName, 'Emlak',
                    @Id, @CreatedAt, @AdvLink, null, @Category, @Floor, @PostType, @LandArea, @Document, @Renovation, @PosterType, @ImageUrls
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
                record.CreatedAt,
                record.AdvLink,
                Floor = record.ApartmentFloor is not null ? record.ApartmentFloor : record.Floor is not null ? record.Floor : null,
                record.PosterType,
                record.Renovation,
                record.PostType,
                record.Document,
                record.LandArea,
                record.Category,
                record.MainTitle,
                record.ImageUrls
            };
            connection.Execute(sqlQuery, parameters);
        }

        public EmlakProperty? FindById(int id)
        {
            using var connection = new SqlConnection(_connectionString);
            string selectQuery = $"SELECT * FROM dbo.{_databaseTableName} WHERE bina_id = @Id AND sayt = 'Emlak';";
            return connection.QuerySingleOrDefault<EmlakProperty>(selectQuery, new { Id = id });
        }
    }
}
