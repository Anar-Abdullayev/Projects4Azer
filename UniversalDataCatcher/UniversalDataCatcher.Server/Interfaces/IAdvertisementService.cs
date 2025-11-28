using UniversalDataCatcher.Server.Dtos;
using UniversalDataCatcher.Server.Entities;

namespace UniversalDataCatcher.Server.Interfaces
{
    public interface IAdvertisementService
    {
        void SetTable(string tableName);
        Task<int> AddAsync(Advertisement adv);
        Task<IEnumerable<Advertisement>> GetAllAsync(AdvertisementFilter? filter = null);
        Task<Advertisement?> GetAsync(int id, string website);
        Task<bool> DeleteAsync(int id);
        Task<int> CountAsync();
    }
}
