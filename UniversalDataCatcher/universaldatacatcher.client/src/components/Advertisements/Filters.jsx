import { Button, Checkbox, FormControlLabel, Switch } from "@mui/material";
import SelectBox from "../Combobox/SelectBox";
import { useState } from "react";
import {
  advTypes,
  buildingTypes,
  categoryItems,
  regions,
  sellerTypes,
} from "../../utils/constants";

function Filters({ filters, onSearch, onFilterChange, onFilterReset }) {
  const handleValueChange = (field, value) => {
    onFilterChange(field, value);
  };

  const handleSearchClick = () => {
    onSearch(filters);
  };
  const handleClearFilters = () => {
    onFilterReset();
  };
  return (
    <div className="bg-gray-100 p-4 rounded-2xl mb-5">
      <div className="flex justify-between gap-5">
        <div className="flex flex-1 flex-col gap-3">
          <SelectBox
            selectedValue={filters.category}
            fieldName={"category"}
            labelText={"Kateqoriya"}
            menuItems={categoryItems}
            onChange={handleValueChange}
          />
          <SelectBox
            selectedValue={filters.postType}
            fieldName={"postType"}
            labelText={"Elanın növü"}
            menuItems={advTypes}
            onChange={handleValueChange}
          />
          <SelectBox
            selectedValue={filters.city}
            fieldName={"city"}
            labelText={"Şəhər"}
            menuItems={regions}
            onChange={handleValueChange}
            multiple={true}
          />
        </div>
        <div className="flex flex-1 flex-col gap-3">
          <SelectBox
            selectedValue={filters.buildingType}
            fieldName={"buildingType"}
            labelText={"Binanın tipi"}
            menuItems={buildingTypes}
            onChange={handleValueChange}
          />
          <SelectBox
            selectedValue={filters.poster_Type}
            fieldName={"poster_Type"}
            labelText={"Satıcının tipi"}
            menuItems={sellerTypes}
            onChange={handleValueChange}
          />
        </div>
      </div>
      <div className="flex justify-between mt-3">
        <FormControlLabel
          control={
            <Switch
              checked={filters.hideRepeats ?? false}
              onChange={(e) =>
                handleValueChange("hideRepeats", e.target.checked)
              }
            />
          }
          label="Təkrar elanları gizlət"
        />
        <div className="flex gap-2">
          {filters && (
            <Button
              variant="contained"
              color="primary"
              onClick={handleClearFilters}
            >
              TƏMİZLƏ
            </Button>
          )}
          <Button
            variant="contained"
            color="primary"
            onClick={handleSearchClick}
          >
            AXTAR
          </Button>
        </div>
      </div>
    </div>
  );
}

export default Filters;
