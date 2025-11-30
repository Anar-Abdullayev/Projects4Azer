import { Button, ButtonBase, Typography } from "@mui/material";
import { DataGrid } from "@mui/x-data-grid";
import { useCallback, useMemo } from "react";
import { columns } from "../../utils/constants.jsx";
import { useState } from "react";

function AdvertisementList({ rows, totalRows, onRefresh }) {
  const [selectedIds, setSelectedIds] = useState({});
  const calculateSelectedRowsCount = () => {
    if (selectedIds.type) {
      if (selectedIds.type === "include") {
        return selectedIds.ids.size;
      } else return rows.length - selectedIds.ids.size;
    }
    return 0;
  };

  const selectedRowsCount = calculateSelectedRowsCount();
  const handleCopy = () => {
    let selectedRows = [];
    if (selectedIds.type === "include") {
      selectedRows = rows.filter((row) => selectedIds.ids.has(row.id));
    } else if (selectedIds.type === "exclude") {
      selectedRows = rows.filter((row) => !selectedIds.ids.has(row.id));
    }
    let tableText = `Sıra | BinaId | Adı | Nömrəsi | Elanın linki\n`;
    tableText += `-----------------------------------------------\n`;

    selectedRows.forEach((row, index) => {
      tableText += `${index + 1} | ${row.bina_Id} | ${row.poster_Name} | ${
        row.poster_Phone
      } | ${row.sayt_Link}\n`;
    });

    navigator.clipboard
      .writeText(tableText)
      .then(() => {
        alert("Kopyalandı!");
      })
      .catch((err) => {
        console.error("Kopyalama uğursuz oldu: ", err);
      });
  };

  return (
    <div>
      <div className="flex gap-5 mb-3 items-center h-5">
        <Typography>Ümumi say: {totalRows}</Typography>
        <Typography>Seçilmişlərin sayı: {selectedRowsCount}</Typography>
        <Button size="small" variant="contained" onClick={onRefresh}>
          Yenilə
        </Button>
        {selectedRowsCount > 0 && (
          <Button
            size="small"
            variant="contained"
            color="info"
            onClick={handleCopy}
          >
            Kopyala
          </Button>
        )}
      </div>
      <DataGrid
        sx={{
          "& .MuiDataGrid-cell:focus": {
            outline: "none",
          },
          "& .MuiDataGrid-cell:focus-within": {
            outline: "none",
          },
        }}
        rows={rows}
        columns={columns}
        checkboxSelection
        hideFooter
        onRowSelectionModelChange={(newSelectedModel) => {
          setSelectedIds(newSelectedModel);
        }}
      />
    </div>
  );
}

export default AdvertisementList;
