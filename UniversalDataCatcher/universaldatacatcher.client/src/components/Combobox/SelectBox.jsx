import { FormControl, InputLabel, MenuItem, Select } from "@mui/material";

function SelectBox({ fieldName, labelText, menuItems, selectedValue, onChange }) {
    const handleChange = (value) => {
        if (value === '')
            value = null;
        onChange(fieldName, value);
    }
    
    return (
        <FormControl sx={{width: '100%'}} size="small">
            <InputLabel id="demo-select-small-label">{labelText}</InputLabel>
            <Select
                labelId="demo-select-small-label"
                id="demo-select-small"
                value={selectedValue || ''}
                label={labelText}
                onChange={(e) => { handleChange(e.target.value) }}
            >
                <MenuItem value=''><em>Hamısı</em></MenuItem>
                {menuItems && menuItems.map((x) => <MenuItem value={x}>{x}</MenuItem>)}
            </Select>
        </FormControl>
    )
}

export default SelectBox;