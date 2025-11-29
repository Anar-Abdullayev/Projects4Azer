import { TabContext, TabList } from "@mui/lab";
import { Box, Tab, Tabs, Typography } from "@mui/material";
import { useState } from "react";
import Advertisements from "../components/Advertisements/Advertisements";
import ServicesPage from "./ServicesPage";

function TabPanel(props) {
  const { children, value, index, ...other } = props;

  return (
    <div
      className="w-full bg-gray-100 p-5"
      role="tabpanel"
      hidden={value !== index}
      id={`vertical-tabpanel-${index}`}
      aria-labelledby={`vertical-tab-${index}`}
      {...other}
    >
      {value === index && <div>{children}</div>}
    </div>
  );
}

function a11yProps(index) {
  return {
    id: `vertical-tab-${index}`,
    "aria-controls": `vertical-tabpanel-${index}`,
  };
}
function HomePage() {
  const [value, setSelectedTab] = useState(0);
  const handleChange = (event, newValue) => {
    setSelectedTab(newValue);
  };
  return (
    <Box
      sx={{
        flexGrow: 1,
        bgcolor: "background.paper",
        display: "flex",
        minHeight: "100vh",
      }}
    >
      <Tabs
      className="w-56 pt-5"
        orientation="vertical"
        variant="scrollable"
        value={value}
        onChange={handleChange}
        aria-label="Vertical tabs example"
        sx={{ borderRight: 1, borderColor: "divider" }}
      >
        <Tab label="Elanlar" {...a11yProps(0)} />
        <Tab label="Servislər" {...a11yProps(1)} />
      </Tabs>
      <TabPanel value={value} index={0}>
        <Advertisements />
      </TabPanel>
      <TabPanel value={value} index={1}>
        <ServicesPage />
      </TabPanel>
    </Box>
  );
}

export default HomePage;
