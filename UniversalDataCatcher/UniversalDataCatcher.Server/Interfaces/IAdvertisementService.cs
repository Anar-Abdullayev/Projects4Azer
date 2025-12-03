using UniversalDataCatcher.Server.Dtos;
using UniversalDataCatcher.Server.Entities;

namespace UniversalDataCatcher.Server.Interfaces
{
    public interface IAdvertisementService
    {
        void SetTable(string tableName);
        Task<IEnumerable<Advertisement>> GetAllAsync(AdvertisementFilter? filter = null);
        Task<Advertisement?> GetAsync(int id);
        Task<int> CountAsync(AdvertisementFilter? filter);
        Task StartSearchingRepeatedAdverts();
        Task SetPostsCopyDate(List<int> postIds);
    }
}
