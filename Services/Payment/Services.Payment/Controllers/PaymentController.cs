using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Services.Payment.Models;
using Shared.ControllerBases;
using Shared.Dtos;

namespace Services.Payment.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : CustomBaseController
    {
        [HttpPost]
        public IActionResult ReceivePatment(PaymentDto paymentDto)
        {
            return CreateActionResultInstance(ResponseDto<Shared.Dtos.NoContent>.Success(200));
        }

    }
}
