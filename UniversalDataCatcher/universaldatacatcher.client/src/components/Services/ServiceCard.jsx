import { Button, CircularProgress } from "@mui/material";
import axios from "axios";
import { useEffect, useState } from "react";

const getDateTimeString = (dateString) => {
  if (!dateString) return "Davam edir";
  const date = new Date(dateString);
  return date.toLocaleString("az-AZ");
};

export default function ServiceCard({ service, onClick }) {
  const [loading, setLoading] = useState(false);
  const getStatusColor = () => {
    if (service.isRunning && !service.startTime) return "bg-[#4caf50]";
    if (service.startTime) return "bg-[#ff9800]";
    return "bg-[#ef5350]";
  };

  const handleServiceClick = () => {
    setLoading(true);
    onClick(service.serviceName);
  };

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
      <h2 className="text-xl font-bold mb-2">{service.serviceLabelName}</h2>

      <div className="space-y-1 text-sm opacity-90">
        <p>Yeni elan sayı: {service.progress ?? "N/A"}</p>
        <p>Status: {service.isRunning ? "İşləyir" : "Dayanıb"}</p>
        <p>Dövrün bitmə vaxtı: {getDateTimeString(service.sleepTime)}</p>
        <p>Dövrün başlama vaxtı: {getDateTimeString(service.startTime)}</p>
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
    </div>
  );
}
