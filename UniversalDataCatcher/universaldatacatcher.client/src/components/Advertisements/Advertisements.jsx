import { useEffect, useState } from "react";
import AdvertisementList from "./AdvertisementList";
import Filters from "./Filters";
import axios from "axios";
import { Pagination } from "@mui/material";
import { ToastContainer } from "react-toastify";

function Advertisements() {
  const [data, setData] = useState([]);
  const [totalRows, setTotalRows] = useState(0);
  const [filters, setFilters] = useState({});
  const [page, setPage] = useState(1);

  async function fetchData() {
    const response = await axios.post("/api/posts", {
      page: page,
      pageSize: 50,
      ...filters,
    });
    const jsonData = response.data;
    setTotalRows(jsonData.totalCount);
    setData(jsonData.posts);
  }

  useEffect(() => {
    fetchData();
  }, [page]);

  const handleFilterSearch = async () => {
    fetchData();
  };

  const handlePageChange = (event, value) => {
    setPage(value);
  };
  const handleFilterChange = (field, value) => {
    setFilters((prev) => ({ ...prev, [field]: value }));
  };
  const handleFilterReset = () => {
    setFilters({});
  };
  const maxPageCount = Math.round(totalRows / 50);
  return (
    <div className="w-full p-5 mb-3 rounded-2xl shadow-xl bg-white">
      <Filters
        filters={filters}
        onSearch={handleFilterSearch}
        onFilterChange={handleFilterChange}
        onFilterReset={handleFilterReset}
      />
      <AdvertisementList
        rows={data}
        totalRows={totalRows}
        onRefresh={handleFilterSearch}
      />
      <div className="flex justify-end mt-4">
        <Pagination
          count={maxPageCount + 1}
          defaultPage={1}
          boundaryCount={2}
          siblingCount={2}
          page={page}
          onChange={handlePageChange}
        />
      </div>
      <ToastContainer />
    </div>
  );
}

export default Advertisements;
