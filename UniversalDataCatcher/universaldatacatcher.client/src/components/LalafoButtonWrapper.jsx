import axios from "axios";
import ServiceControlButton from "./ServiceControlButton";
import { useEffect, useState } from "react";

function LalafoButtonWrapper() {
  const [isRunning, setIsRunning] = useState(false);
  useEffect(() => {
    async function fetchStatus() {
      const response = await axios.get("/api/lalafo/status");
      if (response.status === 200) {
        setIsRunning(response.data.isRunning);
      } else {
        console.log("Failed to fetch status:", response.statusText);
      }
    }
    fetchStatus();
  }, []);
  const handleStartEvent = async () => {
    const response = await axios.post("/api/lalafo/start", {
      dayDifference: 1,
      repeatEveryMinutes: 30,
    });
    if (response.status === 200) {
      setIsRunning(true);
    } else console.log(response.statusText);
  };
  const handleStopEvent = async () => {
    const response = await axios.post("/api/lalafo/stop");
    if (response.status === 200) {
      setIsRunning(false);
    }
  };
  return (
    <div>
      <ServiceControlButton
        serviceName={"Lalafo.az"}
        onStart={handleStartEvent}
        onStop={handleStopEvent}
        isRunning={isRunning}
      />
    </div>
  );
}

export default LalafoButtonWrapper;
