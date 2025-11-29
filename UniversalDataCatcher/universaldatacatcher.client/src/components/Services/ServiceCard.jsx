export default function ServiceCard({ service }) {
  const getStatusColor = () => {
    if (service.isRunning && !service.startTime) return "bg-green-500";
    if (service.startTime) return "bg-yellow-500";
    return "bg-red-500";
  };
  console.log("Service details:", service);
  return (
    <div
      onClick={() => { console.log(service.serviceName + " card clicked"); }}
      className={`
        cursor-pointer p-4 rounded-xl shadow-md transition-all duration-300 
        transform hover:scale-[1.02] ${getStatusColor()}
        text-white select-none
      `}
    >
      <h2 className="text-xl font-bold mb-2">{service.serviceName}</h2>

      <div className="space-y-1 text-sm opacity-90">
        <p>Progress: {service.progress ?? "N/A"}</p>
        <p>Running: {service.isRunning ? "Yes" : "No"}</p>
        <p>Sleep Time: {service.sleepTime ?? "None"}</p>
        <p>Start Time: {service.startTime ?? "None"}</p>
      </div>
    </div>
  );
}