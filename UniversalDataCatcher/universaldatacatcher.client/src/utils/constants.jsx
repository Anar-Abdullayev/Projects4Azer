import { Button } from "@mui/material";
import { handleNavigation } from "./functions";
import DownloadImageButton from "../components/Buttons/DownloadImageButton";

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
        <DownloadImageButton postId={params.row.id} />
      </div>
    ),
  },
];

export const categoryItems = [
  "Həyət evi",
  "Mənzil",
  "Obyekt",
  "Ofis",
  "Qaraj",
  "Torpaq",
  "Digər",
];
export const buildingTypes = ["Köhnə tikili", "Yeni tikili"];
export const advTypes = ["Satış", "Kirayə"];
export const sellerTypes = ["Mülkiyyətçi", "Vasitəçi"];
export const regions = [
  "Ağcabədi",
  "Ağdaş",
  "Ağstafa",
  "Ağsu",
  "Astara",
  "Abşeron",
  "Bakı",
  "Balakən",
  "Beyləqan",
  "Biləsuvar",
  "Bərdə",
  "Cəlilabad",
  "Daşkəsən",
  "Digah",
  "Füzuli",
  "Goranboy",
  "Göyçay",
  "Göygöl",
  "Gədəbəy",
  "Gəncə",
  "Hacıqabul",
  "İmişli",
  "İsmayıllı",
  "Kürdəmir",
  "Lerik",
  "Lənkəran",
  "Masallı",
  "Mingəçevir",
  "Naftalan",
  "Naxçıvan",
  "Neftçala",
  "Oğuz",
  "Qax",
  "Qazax",
  "Qobustan",
  "Quba",
  "Qusar",
  "Qəbələ",
  "Saatlı",
  "Sabirabad",
  "Şabran",
  "Salyan",
  "Şamaxı",
  "Samux",
  "Şirvan",
  "Siyəzən",
  "Sumqayıt",
  "Şəki",
  "Şəmkir",
  "Tovuz",
  "Tərtər",
  "Ucar",
  "Xaçmaz",
  "Xırdalan",
  "Xızı",
  "Xudat",
  "Yardımlı",
  "Yevlax",
  "Zaqatala",
  "Zərdab",
];
