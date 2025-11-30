import { Button, Checkbox, FormControlLabel, Switch } from "@mui/material";
import SelectBox from "../Combobox/SelectBox";
import { useState } from "react";
const categoryItems = [
  "Həyət evi",
  "Mənzil",
  "Obyekt",
  "Ofis",
  "Qaraj",
  "Torpaq",
  "Digər",
];
const buildingTypes = ["Köhnə tikili", "Yeni tikili"];
const advTypes = ["Satış", "Kirayə"];
const sellerTypes = ["Mülkiyyətçi", "Vasitəçi"];
const regions = [
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
function Filters({filters, onSearch, onFilterChange}) {
    const handleValueChange = (field, value) => {
        onFilterChange(field, value);
    }
    
    const handleSearchClick = () => {
        onSearch(filters);
    }
  return (
    <div className="bg-gray-100 p-4 rounded-2xl mb-5">
      <div className="flex justify-between gap-5">
        <div className="flex flex-1 flex-col gap-3">
          <SelectBox selectedValue={filters.category} fieldName={'category'} labelText={"Kateqoriya"} menuItems={categoryItems} onChange={handleValueChange} />
          <SelectBox selectedValue={filters.postType} fieldName={'postType'} labelText={"Elanın növü"} menuItems={advTypes} onChange={handleValueChange} />
          <SelectBox selectedValue={filters.city} fieldName={'city'} labelText={"Şəhər"} menuItems={regions} onChange={handleValueChange} />
        </div>
        <div className="flex flex-1 flex-col gap-3">
          <SelectBox selectedValue={filters.buildingType} fieldName={'buildingType'} labelText={"Binanın tipi"} menuItems={buildingTypes} onChange={handleValueChange} />
          <SelectBox selectedValue={filters.poster_Type} fieldName={'poster_Type'} labelText={"Satıcının tipi"} menuItems={sellerTypes} onChange={handleValueChange} />
        </div>
      </div>
      <div className="flex justify-between mt-3">
        <FormControlLabel control={<Switch onChange={(e) => handleValueChange('hideRepeats', e.target.checked)}/>} label="Təkrar elanları gizlət" />
        <Button variant="contained" color="primary" onClick={handleSearchClick}>Axtar</Button>
      </div>
    </div>
  );
}

export default Filters;
