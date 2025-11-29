import axios from "axios";
import { useState } from "react";
import { useEffect } from "react";
import ServiceCard from "./ServiceCard";
import { Button } from "@mui/material";
import { useCallback } from "react";

function ServicesList(){
    const [services, setServices] = useState([]);
    const fetchServices = useCallback(async () => {
            const response = await axios.get("/api/bots/status");
            setServices(response.data);
        }
    );
    useEffect(() => {
        fetchServices();
    }, []);
    return (
        <div className="flex gap-5">
            <div>
                <Button onClick={() => fetchServices()}>Yenilə</Button>
            </div>
            {services.map((service) => (
                <ServiceCard key={service.serviceName} service={service} />
            ))}
        </div>
    )
}  

export default ServicesList;