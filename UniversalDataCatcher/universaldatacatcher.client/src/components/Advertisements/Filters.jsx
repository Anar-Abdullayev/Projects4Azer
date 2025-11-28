import { Button, Checkbox, FormControlLabel } from "@mui/material";
import SelectBox from "../Combobox/SelectBox";
const categoryItems = ["Həyət evi", "Mənzil", "Obyekt", "Ofis", "Qaraj", "Torpaq", "Digər"]
const buildingTypes = ["Köhnə tikili", "Yeni tikili"]
const advTypes = ["Satış", "Kirayə"]
const sellerTypes = ["Mülkiyyətçi", "Vasitəçi"]
const regions = ["Ağcabədi", "Ağdaş", "Ağstafa", "Ağsu", "Astara", "Abşeron", "Bakı", "Balakən", "Beyləqan", "Biləsuvar", "Bərdə",
    "Cəlilabad", "Daşkəsən", "Füzuli", "Goranboy", "Göyçay", "Göygöl", "Gədəbəy", "Gəncə", "Hacıqabul", "İmişli", "İsmayıllı",
    "Kürdəmir", "Lerik", "Lənkəran", "Masallı", "Mingəçevir", "Naftalan", "Naxçıvan", "Neftçala", "Oğuz", "Qax", "Qazax", "Qobustan",
    "Quba", "Qusar", "Qəbələ", "Saatlı", "Sabirabad", "Şabran", "Salyan", "Şamaxı", "Samux", "Şirvan", "Siyəzən", "Sumqayıt", "Şəki",
    "Şəmkir", "Tovuz", "Tərtər", "Ucar", "Xaçmaz", "Xırdalan", "Xızı", "Xudat", "Yardımlı", "Yevlax", "Zaqatala", "Zərdab"]
function Filters() {
    return (
        <div className="flex flex-col">
            <div className="flex">
                <div className="flex-1 flex-col">
                    <SelectBox labelText={'Kateqoriya'} menuItems={categoryItems} />
                    <SelectBox labelText={'Binanın tipi'} menuItems={buildingTypes} />
                    <SelectBox labelText={'Elanın növü'} menuItems={advTypes} />
                    <SelectBox labelText={'Satıcının tipi'} menuItems={sellerTypes} />
                    <SelectBox labelText={'Şəhər'} menuItems={regions} />
                </div>
                <div className="flex-1 flex-col">

                </div>
            </div>
            <div className="flex justify-end">
                <FormControlLabel control={<Checkbox/>} label="Təkrar elanları gizlət" />
            </div>
        </div>

    )
}

export default Filters;