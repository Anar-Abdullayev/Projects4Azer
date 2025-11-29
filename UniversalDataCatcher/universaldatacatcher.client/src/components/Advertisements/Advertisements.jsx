import { useEffect, useState } from "react";
import AdvertisementList from "./AdvertisementList";
import Filters from "./Filters";
import axios from "axios";

function Advertisements() {
  const [data, setData] = useState([]);
  const [totalRows, setTotalRows] = useState(0);
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

  const handleFilterSearch = async (filters) => {
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

  return (
    <div className="w-full p-5 mb-3 rounded-2xl shadow-xl bg-white">
      <Filters onSearch={handleFilterSearch} />
      <AdvertisementList rows={data} totalRows={totalRows} />
    </div>
  );
}

export default Advertisements;
