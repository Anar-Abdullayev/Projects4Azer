import Button from "react-bootstrap/Button";

function ServiceControlButton({serviceName, onStart, onStop, isRunning}) {
    return (
        <div>
            <h3>{serviceName}</h3>
            <Button variant={isRunning ? "danger" : "success"} onClick={!isRunning ? onStart : onStop}>{isRunning ? "Stop" : "Start"}</Button>
        </div>
    );
}

export default ServiceControlButton;