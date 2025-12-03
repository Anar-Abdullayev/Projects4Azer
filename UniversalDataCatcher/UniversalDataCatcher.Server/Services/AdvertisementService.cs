using Dapper;
using System.Data;
using UniversalDataCatcher.Server.Dtos;
using UniversalDataCatcher.Server.Entities;
using UniversalDataCatcher.Server.Extentions;
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
        public async Task<int> CountAsync(AdvertisementFilter filter)
        {
            string sqlQuery = $"SELECT count(*) FROM {_tableName}";
            var wherePart = filter.GetWherePart();
            sqlQuery += wherePart.Item1;
            var count = await _db.ExecuteScalarAsync<int>(sqlQuery, wherePart.Item2);
            return count;
        }
        public async Task<IEnumerable<Advertisement>> GetAllAsync(AdvertisementFilter filter)
        {
            var sql = $"SELECT * FROM {_tableName}";
            var wherePart = filter.GetWherePart();
            sql += wherePart.Item1;
            sql += $" ORDER BY id DESC OFFSET {(filter.Page-1)*filter.PageSize} ROWS FETCH NEXT {filter.PageSize} ROWS ONLY";
            return await _db.QueryAsync<Advertisement>(sql, wherePart.Item2);
        }
        public async Task StartSearchingRepeatedAdverts()
        {
            var sql = "exec sp_UpdateReferenceIds_Final;";
            await _db.ExecuteAsync(sql);
        }
        public async Task<Advertisement?> GetAsync(int id)
        {
            var sqlQuery = $"SELECT * FROM {_tableName} WHERE id = @Id";
            return await _db.QueryFirstOrDefaultAsync<Advertisement>(sqlQuery, new {Id = id});
        }
        public async Task SetPostsCopyDate(List<int> postIds)
        {
            string ids = string.Join(",", postIds);
            string query = $"UPDATE {_tableName} SET status = @Date WHERE id in ({ids})";
            await _db.ExecuteAsync(query, new { Date = DateTime.Now });
        }
    }
}
