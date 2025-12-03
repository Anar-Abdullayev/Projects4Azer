import { Button } from "@mui/material";
import axios from "axios";
import { useState } from "react";
import DownloadForOfflineOutlinedIcon from "@mui/icons-material/DownloadForOfflineOutlined";
import CircularProgress from "@mui/material/CircularProgress";
import { toast } from "react-toastify";

export default function DownloadImageButton({ postId }) {
  const [isDownloading, setIsDownloading] = useState(false);
  const handleDownload = async (e) => {
    e.stopPropagation();
    setIsDownloading(true);
    await toast.promise(
      axios.get(`/api/posts/${postId}/downloadimages`),
      {
        success: `(${postId}) Şəkil yükləndi`,
        error: `(${postId} yüklənməsi uğursuz oldu!)`,
      },
      {
        position: "bottom-right",
        autoClose: 1500
      }
    );
    setIsDownloading(false);
  };
  return (
    <Button
      onClick={handleDownload}
      variant="outlined"
      color="primary"
      size="small"
      style={{ marginRight: 16 }}
    >
      {isDownloading ? (
        <CircularProgress size={24} />
      ) : (
        <DownloadForOfflineOutlinedIcon />
      )}
    </Button>
  );
}
