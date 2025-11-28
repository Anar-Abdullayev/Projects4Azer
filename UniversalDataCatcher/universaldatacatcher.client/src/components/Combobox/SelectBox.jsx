import { FormControl, InputLabel, MenuItem, Select } from "@mui/material";
import { useState } from "react";

function SelectBox({ labelText, menuItems }) {
    const [value, setValue] = useState("");
    return (
        <FormControl sx={{m:1, width: '100%'}} size="small">
            <InputLabel id="demo-select-small-label">{labelText}</InputLabel>
            <Select
                labelId="demo-select-small-label"
                id="demo-select-small"
                value={value}
                label={labelText}
                onChange={(e) => { setValue(e.target.value) }}
            >
                <MenuItem value=''><em>Hamısı</em></MenuItem>
                {menuItems && menuItems.map((x) => <MenuItem value={x}>{x}</MenuItem>)}
            </Select>
        </FormControl>
    )
}

export default SelectBox;