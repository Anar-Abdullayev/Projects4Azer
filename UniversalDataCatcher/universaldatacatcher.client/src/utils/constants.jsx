import { Button } from "@mui/material";
import { handleNavigation } from "./functions";

export const columns = [
    { field: "id", headerName: "ID", width: 90 },
    {
      field: "bina_Id",
      headerName: "Elan Id",
      flex: 1,
      editable: false,
      sortable: false,
    },
    {
      field: "poster_Name",
      headerName: "Elan Sahibi",
      flex: 1,
      editable: false,
      sortable: false,
    },
    {
      field: "poster_Type",
      headerName: "Növü",
      flex: 1,
      editable: false,
      sortable: false,
    },
    {
      field: "poster_Phone",
      headerName: "Əlaqə nömrəsi",
      flex: 1,
      editable: false,
      sortable: false,
    },
    {
      field: "sayt",
      headerName: "Sayt",
      flex: 1,
      editable: false,
      sortable: false,
    },
    {
      field: "actions",
      headerName: "Əməliyyatlar",
      flex: 1,
      editable: false,
      sortable: false,
      renderCell: (params) => (
        <div>
          <Button
            onClick={(e) => handleNavigation(e, params.row.sayt_Link)}
            variant="outlined"
            color="primary"
            size="small"
            style={{ marginRight: 16 }}
          >
            LİNKƏ KEÇ
          </Button>
        </div>
      ),
    },
  ];
  