import { FormControl, InputLabel, MenuItem, Select } from "@mui/material";

function SelectBox({
    fieldName,
    labelText,
    menuItems,
    selectedValue,
    onChange,
    multiple = false
}) {
    const handleChange = (value) => {
        if (!multiple) {
            if (value === "") value = null;
            onChange(fieldName, value);
        } else {
            if (value.includes("")) {
                onChange(fieldName, []);
            } else {
                onChange(fieldName, value);
            }
        }
    };

    return (
        <FormControl sx={{ width: "100%" }} size="small">
            <InputLabel id={`${fieldName}-label`}>{labelText}</InputLabel>

            <Select
                labelId={`${fieldName}-label`}
                id={`${fieldName}-select`}
                multiple={multiple}
                value={
                    multiple
                        ? selectedValue || []
                        : selectedValue || ""
                }
                label={labelText}
                onChange={(e) => handleChange(e.target.value)}
                renderValue={
                    multiple
                        ? (selected) => selected.join(", ")
                        : undefined
                }
            >
                <MenuItem value="">
                    <em>Hamısı</em>
                </MenuItem>

                {menuItems?.map((x) => (
                    <MenuItem key={x} value={x}>
                        {x}
                    </MenuItem>
                ))}
            </Select>
        </FormControl>
    );
}

export default SelectBox;
