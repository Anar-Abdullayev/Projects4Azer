import {
  Dialog,
  DialogTitle,
  DialogContent,
  DialogActions,
  Button,
  TextField,
  Stack,
} from "@mui/material";
import { useEffect, useState } from "react";

export default function SettingsDialog({ open, onClose, onSave, serviceName }) {
  const [dayDifference, setDayDifference] = useState(1);
  const [repeatMinutes, setRepeatMinutes] = useState(30);

  // Load settings on dialog open
  useEffect(() => {
    if (!open) return;

    const saved = localStorage.getItem(`settings_${serviceName}`);
    if (saved) {
      const data = JSON.parse(saved);
      setRepeatMinutes(data.repeatMinutes || 30);
      setDayDifference(data.dayDifference || 1);
    }
  }, [open, serviceName]);

  const handleSave = () => {
    const data = {
      repeatMinutes,
      dayDifference,
    };

    localStorage.setItem(`settings_${serviceName}`, JSON.stringify(data));
    onSave && onSave(data);
    onClose();
  };
  const handleRepeatMinutesChange = (e) => {
    if (e.target.value < 1) return;
    setRepeatMinutes(Number(e.target.value));
  }
  const handledayDifferenceChange = (e) => {
    if (e.target.value < 0) return;
    setDayDifference(Number(e.target.value));
  }
  return (
    <Dialog open={open} onClose={onClose} maxWidth="xs" fullWidth>
      <DialogTitle>{serviceName} üçün tənzimlənmələr </DialogTitle>

      <DialogContent>
        <Stack spacing={2} mt={1}>
          <TextField
            type="number"
            label="Təkrarlanma dəqiqəsi"
            value={repeatMinutes}
            onChange={handleRepeatMinutesChange}
            fullWidth
          />

          <TextField
            type="number"
            label="Gün fərqi"
            value={dayDifference}
            onChange={handledayDifferenceChange}
            fullWidth
          />
        </Stack>
      </DialogContent>

      <DialogActions>
        <Button onClick={onClose} color="inherit">
          GERİ
        </Button>
        <Button onClick={handleSave} variant="contained">
          YADDA SAXLA
        </Button>
      </DialogActions>
    </Dialog>
  );
}
