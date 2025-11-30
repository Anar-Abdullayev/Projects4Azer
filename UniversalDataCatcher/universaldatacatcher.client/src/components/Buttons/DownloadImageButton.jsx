import { Button } from "@mui/material";
import axios from "axios";
import { useState } from "react";

export default function DownloadImageButton({ postId }) {
  const [isDownloading, setIsDownloading] = useState(false);
  const handleDownload = async (e) => {
    e.stopPropagation();
    setIsDownloading(true);
    const response = await axios.get(`/api/posts/${postId}/downloadimages`);
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
      {isDownloading ? "YÜKLƏNİR..." : "YÜKLƏ"}
    </Button>
  );
}
