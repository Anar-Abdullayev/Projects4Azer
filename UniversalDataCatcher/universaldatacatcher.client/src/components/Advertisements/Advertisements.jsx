import { useEffect, useState } from "react";
import AdvertisementList from "./AdvertisementList";
import Filters from "./Filters";
import axios from "axios";

function Advertisements() {
  const [data, setData] = useState([]);
  const [totalRows, setTotalRows] = useState(0);
  const [filters, setFilters] = useState({});
  useEffect(() => {
    async function fetchData() {
      const response = await axios.post("/api/posts", {
        page: 1,
        pageSize: 50,
      });
      const jsonData = response.data;
      setTotalRows(jsonData.totalCount);
      setData(jsonData.posts);
    }
    fetchData();
  }, []);

  const handleFilterSearch = async () => {
    const body = {
      page: 1,
      pageSize: 50,
      ...filters,
    };
    const response = await axios.post("/api/posts", body);
    const jsonData = response.data.posts;
    const total = response.data.totalCount;
    setTotalRows(total);
    setData(jsonData);
  };

  const handleFilterChange = (field, value) => {
    setFilters((prev) => ({...prev, [field]: value}));
  }
  return (
    <div className="w-full p-5 mb-3 rounded-2xl shadow-xl bg-white">
      <Filters filters={filters} onSearch={handleFilterSearch} onFilterChange={handleFilterChange} />
      <AdvertisementList rows={data} totalRows={totalRows} onRefresh={handleFilterSearch}/>
    </div>
  );
}

export default Advertisements;
