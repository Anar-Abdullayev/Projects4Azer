using Dapper;
using Microsoft.Data.SqlClient;
using UniversalDataCatcher.Server.Bots.Tap.Models;
using UniversalDataCatcher.Server.Services;

namespace UniversalDataCatcher.Server.Bots.Tap.Services
{
    public class TapazMSSqlDatabaseService
    {
        private readonly string _connectionString;
        private readonly string _databaseTableName;
        public TapazMSSqlDatabaseService()
        {
            _connectionString = MSSqlDatabaseService.GetConnectionString();
            _databaseTableName = MSSqlDatabaseService.GetTableName("TapAz");
        }

        public void InsertRecord(TapAzProperty record)
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
                    @Id, @MainTitle, @Address, @Description, @Price, @Area, @RoomCount, @PhoneNumbers, @Owner, 'TapAz',
                    @Id, @CreatedAt, @AdvLink, null, @Category, null, @AdvType, @LandArea, null, null, @OwnerType
                );";
            var parameters = new
            {
                record.Id,
                record.MainTitle,
                record.Address,
                record.Description,
                record.Price,
                record.Area,
                record.RoomCount,
                record.PhoneNumbers,
                record.Owner,
                record.AdvLink,
                record.Category,
                record.AdvType,
                record.OwnerType,
                record.LandArea,
                CreatedAt = record.CreatedAt.ToString(),
            };
            connection.Execute(sqlQuery, parameters);
        }

        public TapAzProperty? FindById(int id)
        {
            using var connection = new SqlConnection(_connectionString);
            string selectQuery = $"SELECT * FROM dbo.{_databaseTableName} WHERE bina_id = @Id AND sayt = 'TapAz';";
            return connection.QuerySingleOrDefault<TapAzProperty>(selectQuery, new { Id = id });
        }
    }
}
