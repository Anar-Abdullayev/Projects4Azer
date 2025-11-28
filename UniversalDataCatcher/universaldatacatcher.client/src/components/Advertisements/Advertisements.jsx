import AdvertisementList from "./AdvertisementList";
import Filters from "./Filters";

function Advertisements(){
    
    return (
        <div className="w-full ps-5 pe-5 mb-3">
            <Filters/>
            <AdvertisementList />
        </div>
    )
}

export default Advertisements;