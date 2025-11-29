using Dapper;
using UniversalDataCatcher.Server.Dtos;

namespace UniversalDataCatcher.Server.Extentions
{
    public static class AdvertisementFilterExtentions
    {
        public static Tuple<string, DynamicParameters> GetWherePart(this AdvertisementFilter filter)
        {
            var where = new List<string>();
            var param = new DynamicParameters();

            if (filter is not null)
            {
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
                    param.Add("@PosterType", filter.Poster_Type.ToLower());
                }

                if (!string.IsNullOrEmpty(filter.City))
                {
                    where.Add("address LIKE @CityPattern");
                    param.Add("@CityPattern", filter.City + "%");
                }

                if (filter.HideRepeats)
                {
                    where.Add("ReferenceId is null");
                }
            }

            if (where.Count > 0)
            {
                string wherePartQuery = " WHERE " + string.Join(" AND ", where);
                Tuple<string, DynamicParameters> tuple = new Tuple<string, DynamicParameters>(wherePartQuery, param);
                return tuple;
            }
            return new Tuple<string, DynamicParameters>("", param);
        }
    }
}
