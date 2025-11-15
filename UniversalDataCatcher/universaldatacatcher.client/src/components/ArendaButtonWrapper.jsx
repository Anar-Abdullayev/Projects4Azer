import axios from "axios";
import ServiceControlButton from "./ServiceControlButton";
import { useEffect, useState } from "react";

function ArendaButtonWrapper() {
  const [isRunning, setIsRunning] = useState(false);
    useEffect(() => {
        async function fetchStatus() {
            const response = await axios.get("/api/arenda/status");
            if (response.status === 200) {
                setIsRunning(response.data.isRunning);
            } else {
                console.log("Failed to fetch status:", response.statusText);
            }
        }
        fetchStatus();
    }, []);

  const handleStartEvent = async () => {
    const response = await axios.post("/api/arenda/start", {
      dayDifference: 1,
      repeatEveryMinutes: 30,
    });
    if (response.status === 200) {
      setIsRunning(true);
    } else console.log(response.statusText);
  };
  const handleStopEvent = async () => {
    const response = await axios.post("/api/arenda/stop");
    if (response.status === 200) {
      setIsRunning(false);
    }
  };
  return (
    <div>
      <ServiceControlButton
        serviceName={"Arenda.az"}
        onStart={handleStartEvent}
        onStop={handleStopEvent}
        isRunning={isRunning}
      />
    </div>
  );
}

export default ArendaButtonWrapper;
