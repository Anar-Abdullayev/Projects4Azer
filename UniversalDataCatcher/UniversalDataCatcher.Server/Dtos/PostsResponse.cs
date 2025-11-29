using UniversalDataCatcher.Server.Entities;

namespace UniversalDataCatcher.Server.Dtos
{
    public class PostsResponse
    {
        public List<Advertisement> MyProperty { get; set; }
        public int TotalCount { get; set; }
    }
}
