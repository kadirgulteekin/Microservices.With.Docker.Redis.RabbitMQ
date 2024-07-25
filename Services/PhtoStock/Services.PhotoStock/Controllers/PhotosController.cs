using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Services.PhotoStock.Dtos;
using Shared.ControllerBases;
using Shared.Dtos;


namespace Services.PhotoStock.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PhotosController : CustomBaseController
    {
        [HttpPost]
        public async Task<IActionResult> PhotoSave(IFormFile photo,CancellationToken cancellationToken)
        {
            if(photo != null && photo.Length > 0)
            {
                var path = Path.Combine(Directory.GetCurrentDirectory(),"wwwroot/photos", photo.FileName);

                using var stream = new FileStream(path, FileMode.Create);
                await photo.CopyToAsync(stream, cancellationToken).ConfigureAwait(false);

                var returnPath = "photos/" + photo.FileName;

                PhotoDto photoDto = new() { Url = returnPath };

                return CreateActionResultInstance(ResponseDto<PhotoDto>.Success(photoDto, 200));

            }

            return CreateActionResultInstance(ResponseDto<PhotoDto>.Failed("photo is empty",400));
        }

        public async Task<IActionResult> PhotoDelete(string photoUrl)
        {
            var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/photos", photoUrl);

            if (!System.IO.File.Exists(path))
            {
                return CreateActionResultInstance(ResponseDto<NoContent>.Failed("photo not found", 404));
            }

            await Task.Run(() => System.IO.File.Delete(path));

            return CreateActionResultInstance(ResponseDto<NoContent>.Success(204));
        }
    }
}
