using Dapper;
using System.Data;
using UniversalDataCatcher.Server.Dtos;
using UniversalDataCatcher.Server.Entities;
using UniversalDataCatcher.Server.Interfaces;

namespace UniversalDataCatcher.Server.Services
{
    public class AdvertisementService : IAdvertisementService
    {
        private IDbConnection _db;
        private string _tableName;
        public AdvertisementService(IDbConnection db)
        {
            _db = db;
        }
        public void SetTable(string tableName)
        {
            _tableName = tableName;
        }
        public async Task<int> AddAsync(Advertisement adv)
        {
            string sqlQuery = @$"
                INSERT INTO dbo.{_tableName} (
                    bina_id, main_title, address, area, torpaqarea, amount, currency, renovation, document, ipoteka, binatype, room, floor, category, item_id, poster_name, poster_note, post_tip, poster_type, poster_phone, post_create_date, insertdate, updated, sayt, sayt_link, status, ReferenceId
                )

                VALUES (
                    @BinaId, @MainTitle, @Address, @Area, @TorpaqArea, @Amount, @Currency, @Renovation, @Document, @Ipoteka, @BinaType, @Room, @Floor, @Category, @ItemId, @PosterName, @PosterNote, @PostTip, @PosterType, @PosterPhone, @PostCreateDate, @InsertDate, @UpdateDate, @Website, @AdvLink, @Status, @ReferenceId
                );";

            int newId = await _db.ExecuteScalarAsync<int>(sqlQuery, adv);
            return newId;
        }

        public async Task<int> CountAsync()
        {
            string sqlQuery = $"SELECT count(*) FROM {_tableName}";
            var count = await _db.ExecuteScalarAsync<int>(sqlQuery);
            return count;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            string sqlQuery = $"DELETE FROM {_tableName} WHERE id = @Id";
            var isExecuted = await _db.ExecuteAsync(sqlQuery, new { Id = id });
            return isExecuted == 1;
        }

        public async Task<IEnumerable<Advertisement>> GetAllAsync(AdvertisementFilter? filter = null)
        {
            var where = new List<string>();
            var param = new DynamicParameters();
            if (filter is null)
                return await _db.QueryAsync<Advertisement>($"SELECT TOP(100) * FROM {_tableName} ORDER BY id DESC;");

            if (!string.IsNullOrEmpty(filter.Category))
            {
                where.Add("category = @Category");
                param.Add("@Category", filter.Category);
            }

            if (!string.IsNullOrEmpty(filter.BuildingType))
            {
                where.Add("binatype = @BuildingType");
                param.Add("@BuildingType", filter.BuildingType);
            }

            if (!string.IsNullOrEmpty(filter.PostType))
            {
                where.Add("post_tip = @PostType");
                param.Add("@PostType", filter.PostType);
            }

            if (!string.IsNullOrEmpty(filter.Poster_Type))
            {
                where.Add("poster_type = @PosterType");
                param.Add("@PosterType", filter.Poster_Type);
            }

            if (!string.IsNullOrEmpty(filter.City))
            {
                where.Add("address LIKE @CityPattern");
                param.Add("@CityPattern", filter.City + "%");
            }

            var sql = $"SELECT * FROM {_tableName}";

            if (where.Count > 0)
                sql += " WHERE " + string.Join(" AND ", where);

            return await _db.QueryAsync<Advertisement>(sql, param);
        }

        public async Task<Advertisement?> GetAsync(int id, string website)
        {
            string sql = $"SELECT * FROM {_tableName} WHERE bina_id = @Id and sayt = @Website";
            var adv = await _db.ExecuteScalarAsync<Advertisement>(sql, new { Id = id, Website = website });
            return adv;
        }

    }
}
