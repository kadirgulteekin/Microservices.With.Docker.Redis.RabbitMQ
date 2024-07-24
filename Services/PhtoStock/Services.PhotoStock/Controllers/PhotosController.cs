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
        public async Task<IActionResult> PhotoSave(IFormFile formFile,CancellationToken cancellationToken)
        {
            if(formFile !=null && formFile.Length > 0)
            {
                var path = Path.Combine(Directory.GetCurrentDirectory(),"wwwroot/photos",formFile.FileName);

                using var stream = new FileStream(path, FileMode.Create);
                await formFile.CopyToAsync(stream, cancellationToken).ConfigureAwait(false);

                var returnPath = "photos/" + formFile.FileName;

                PhotoDto photoDto = new() { Url = returnPath };

                return CreateActionResultInstance(ResponseDto<PhotoDto>.Success(photoDto, 200));

            }

            return CreateActionResultInstance(ResponseDto<PhotoDto>.Failed("photo is empty",400));
        }
    }
}
