using Services.Basket.Dtos;
using Shared.Dtos;

namespace Services.Basket.Services
{
    public interface IBasketService
    {
        Task<ResponseDto<BasketDto>> GetBasket(string userId);

        Task<ResponseDto<bool>> SaveOrUpdate(BasketDto basketDto);

        Task<ResponseDto<bool>> Delete(string userId);

    }
}
