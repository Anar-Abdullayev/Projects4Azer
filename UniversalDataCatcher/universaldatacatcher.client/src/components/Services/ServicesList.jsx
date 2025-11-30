import axios from "axios";
import { useState } from "react";
import { useEffect } from "react";
import ServiceCard from "./ServiceCard";
import { Button } from "@mui/material";
import { useCallback } from "react";

let interval = null;

function ServicesList() {
  const [services, setServices] = useState([]);
  const [isLoadingServices, setIsLoadingServices] = useState([]);
  const fetchServices = useCallback(async () => {
    const response = await axios.get("/api/bots/status");
    setServices(response.data);
  });
  useEffect(() => {
    fetchServices();
  }, []);

  useEffect(() => {
    if (interval === null)
      interval = setInterval(() => {
        fetchServices();
      }, 3000);

    return () => {
      clearInterval(interval);
      interval = null;
    };
  }, []);
  const handleServiceClick = async (serviceName) => {
    const service = services.find((s) => s.serviceName === serviceName);
    console.log("Service clicked:", service.serviceName);
    if (service.isRunning) {
      await axios.post(`/api/${service.serviceName}/stop`);
    } else {
      await axios.post(`/api/${service.serviceName}/start`, {
        dayDifference: 1,
        repeatEveryMinutes: 30,
      });
    }

  };

  return (
    <div>
      <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-5">
        {services.map((service) => (
          <ServiceCard
            key={service.serviceName}
            service={service}
            onClick={handleServiceClick}
          />
        ))}
      </div>
    </div>
  );
}

export default ServicesList;
