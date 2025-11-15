import { Stack } from "react-bootstrap";
import ArendaButtonWrapper from "./components/ArendaButtonWrapper";
import LalafoButtonWrapper from "./components/LalafoButtonWrapper";

function App() {
  return (
    <>
      <div>
        <Stack direction="horizontal" gap={5}>
          <ArendaButtonWrapper />
          <LalafoButtonWrapper />
        </Stack>
      </div>
    </>
  );
}

export default App;
