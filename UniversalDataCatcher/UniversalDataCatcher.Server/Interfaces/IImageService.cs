namespace UniversalDataCatcher.Server.Interfaces
{
    public interface IImageService
    {
        Task<byte[]> DownloadImageByteArray(string url);
        Task UploadImageTo(string baseFolder, string subFolder, string fileName, byte[] imageByteArray);
    }
}
