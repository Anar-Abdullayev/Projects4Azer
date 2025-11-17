using Dapper;
using Microsoft.Data.SqlClient;
using UniversalDataCatcher.Server.Bots.Bina.Models;
using UniversalDataCatcher.Server.Services;

namespace UniversalDataCatcher.Server.Bots.Bina.Services
{
    public class BinaMSSqlDatabaseService
    {
        private string _connectionString = "";
        private string _tableName = "";
        public BinaMSSqlDatabaseService()
        {
            _connectionString = MSSqlDatabaseService.GetConnectionString();
            _tableName = MSSqlDatabaseService.GetTableName("BinaAz");
        }

        public void InsertRecord(BinaAzProperty record)
        {

            using var connection = new SqlConnection(_connectionString);
            var dateNow = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            string sqlQuery = @$"
                INSERT INTO dbo.{_tableName} (
                    bina_id, main_title, address, poster_note, amount, area, room, poster_phone, poster_name, sayt, 
                    item_id, post_create_date, sayt_link, binatype, category, floor, post_tip, torpaqarea, document, 
                    renovation, poster_type, currency, ipoteka
                )

                VALUES (
                    @Id, @MainTitle, @FullAddress, @Description, @Price, @Area, @RoomCount, @PhoneNumbers, @Owner, 'BinaAz',
                    @Id, @CreatedAt, @AdvLink, @BuildingType, @Category, @Floor, @Post_Type, @LandArea, @Cixaris, @Repair, @OwnerType, @Currency,
                    @Ipoteka
                );";

            connection.Execute(sqlQuery, record);
        }

        public BinaAzProperty? FindById(int id)
        {
            using var connection = new SqlConnection(_connectionString);
            string selectQuery = $"SELECT * FROM dbo.{_tableName} WHERE bina_id = @Id AND sayt = 'BinaAz';";
            return connection.QuerySingleOrDefault<BinaAzProperty>(selectQuery, new { Id = id });
        }
    }
}
