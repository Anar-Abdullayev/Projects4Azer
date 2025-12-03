import { Button, CircularProgress } from "@mui/material";
import { useEffect, useState } from "react";
import SettingsIcon from '@mui/icons-material/Settings';
import SettingsDialog from "./SettingsDialog";
import axios from "axios";

const getDateTimeString = (dateString) => {
  if (!dateString) return "Davam edir";
  const date = new Date(dateString);
  return date.toLocaleString("az-AZ");
};

export default function ServiceCard({ service, onClick }) {
  const [loading, setLoading] = useState(false);
  const [dialogOpen, setDialogOpen] = useState(false);
  const [settings, setSettings] = useState({
    repeatMinutes: 30,
    dayDifference: 1,
  });

  const getStatusColor = () => {
    if (service.isRunning && !service.startTime) return "bg-[#4caf50]";
    if (service.startTime) return "bg-[#ff9800]";
    return "bg-[#ef5350]";
  };
  
  const handleServiceClick = async () => {
    setLoading(true);
    if (service.isRunning) {
      await axios.post(`/api/${service.serviceName}/stop`);
    } else {
      let serviceSettings = JSON.parse(localStorage.getItem(`settings_${service.serviceLabelName}`));
      const body = {
        dayDifference: serviceSettings?.dayDifference || 1,
        repeatEveryMinutes: serviceSettings?.repeatMinutes || 30,
      };
      await axios.post(`/api/${service.serviceName}/start`, body);
    }
  };
  const handleSaveSettings = (settings) => {
    setSettings(settings);
  }

  useEffect(() =>{
    const saved = localStorage.getItem(`settings_${service.serviceLabelName}`);
    if (saved) {
      const data = JSON.parse(saved);
      setSettings({
        repeatMinutes: data.repeatMinutes || 30,
        dayDifference: data.dayDifference || 1,
      });
    }
  }, []);

  useEffect(() => {
    if (loading) setLoading(false);
  }, [service.isRunning]);

  return (
    <div
      className={`
        p-4 rounded-xl shadow-md transition-all duration-300 
        transform hover:scale-[1.02] ${getStatusColor()}
        text-white select-none
      `}
    >
      <div className="flex justify-between">
        <h2 className="text-xl font-bold mb-2">{service.serviceLabelName}</h2>
        <SettingsIcon sx={{fontSize: 30}} className="cursor-pointer" onClick={() => {setDialogOpen(true)}}/>
      </div>

      <div className="space-y-1 text-sm opacity-90">
        <p>Yeni elan sayı: {service.progress ?? "N/A"}</p>
        <p>Status: {service.isRunning ? "İşləyir" : "Dayanıb"}</p>
        <p>Dövrün bitmə vaxtı: {getDateTimeString(service.sleepTime)}</p>
        <p>Dövrün başlama vaxtı: {getDateTimeString(service.startTime)}</p>
        <p>Gün fərqi: {settings.dayDifference}</p>
        <p>Yenidən başlama dəqiqəsi: {settings.repeatMinutes}</p>
      </div>
      <div className="flex gap-3">
        <Button
          variant="contained"
          className="flex-1 bg-white text-black h-10"
          onClick={handleServiceClick}
          
        >
          {loading ? <CircularProgress size={24} className="mr-2" /> : 
          service.isRunning ? "Dayandır" : "Başlat"}
        </Button>
      </div>
      <SettingsDialog open={dialogOpen} onClose={() => setDialogOpen(false)} onSave={handleSaveSettings} serviceName={service.serviceLabelName}/>
    </div>
  );
}
