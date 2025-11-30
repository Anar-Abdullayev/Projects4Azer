using UniversalDataCatcher.Server.Interfaces;

namespace UniversalDataCatcher.Server.Services
{
    public class LocalImageService : IImageService
    {
        public async Task<byte[]> DownloadImageByteArray(string url)
        {
            using var client = new HttpClient();
            var bytes = await client.GetByteArrayAsync(url);
            return bytes;
        }

        public async Task UploadImageTo(string baseFolder, string subFolder, string fileName, byte[] imageByteArray)
        {
            try
            {
                var downloadFolder = Path.Combine(baseFolder, subFolder);
                if (!Directory.Exists(downloadFolder))
                    Directory.CreateDirectory(downloadFolder);
                var downloadPath = Path.Combine(downloadFolder, fileName);
                await File.WriteAllBytesAsync(downloadPath, imageByteArray);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
