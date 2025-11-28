import { TabContext, TabList, TabPanel } from "@mui/lab";
import { Box, Tab, Tabs } from "@mui/material";
import { useState } from "react";
import Advertisements from "../components/Advertisements/Advertisements";

function HomePage() {
    const [selectedTab, setSelectedTab] = useState(0);
    const handleChange = (event, newValue) => {
        setSelectedTab(newValue);
    }
    return (
        <>
            <Tabs value={selectedTab} onChange={handleChange} centered>
                <Tab label="ELANLAR" />
                <Tab label="SERVİSLƏR" />
            </Tabs>
            <div className="flex justify-center max-w-330 m-auto border-r border-l mt-5">
                {selectedTab == 0 && <Advertisements />}
                {/* {selectedTab == 1 && <ServicesList/>} */}
            </div>
        </>

    )
}

export default HomePage;