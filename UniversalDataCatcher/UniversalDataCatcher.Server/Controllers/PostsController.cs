using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using UniversalDataCatcher.Server.Dtos;
using UniversalDataCatcher.Server.Entities;
using UniversalDataCatcher.Server.Interfaces;

namespace UniversalDataCatcher.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PostsController : ControllerBase
    {
        private IAdvertisementService _advertisementService;
        public PostsController(IAdvertisementService advertisementService)
        {
            _advertisementService = advertisementService;
            _advertisementService.SetTable("post");
        }

        [HttpPost]
        public async Task<ActionResult> GetPosts([FromBody] AdvertisementFilter filter)
        {
            if (filter.HideRepeats)
                await _advertisementService.StartSearchingRepeatedAdverts();
            var result = await _advertisementService.GetAllAsync(filter);
            var count = await _advertisementService.CountAsync(filter);
            return Ok(new
            {
                Posts = result,
                TotalCount = count
            });
        }

        [HttpGet("{id}/downloadimages")]
        public async Task<ActionResult> DownloadImages(int id, [FromServices] IImageService _imageService, [FromServices] IConfiguration configuration)
        {
            var advertisement = await _advertisementService.GetAsync(id);
            if (advertisement == null)
                return BadRequest("Post not found");

            if (string.IsNullOrEmpty(advertisement.ImageUrls))
                return BadRequest("Post has empty image urls");

            var imageUrls = advertisement.ImageUrls.Split(", ");
            var baseFolder = configuration["ImageSettings:BaseFolder"];
            var folderPath = baseFolder + advertisement.Sayt;
            int index = 0;
            foreach (var imageUrl in imageUrls)
            {
                var byteArray = await _imageService.DownloadImageByteArray(imageUrl);
                var fileExtention = Path.GetExtension(imageUrl);
                var filename = $"image_{index++}{fileExtention}";
                await _imageService.UploadImageTo(folderPath, advertisement.Bina_Id.ToString()!, filename, byteArray);
            }

            return Ok();
        }

        [HttpPost("copy")]
        public async Task<ActionResult> SetCopiedPosts([FromBody] CopiedItemsRequestDto request)
        {
            if (!ModelState.IsValid)
                return BadRequest("At least one post must be sent inside array");
            await _advertisementService.SetPostsCopyDate(request.CopiedPosts);
            return Ok();
        }
    }
}
