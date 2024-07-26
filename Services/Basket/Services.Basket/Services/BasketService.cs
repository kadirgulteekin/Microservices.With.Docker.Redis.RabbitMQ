using Services.Basket.Dtos;
using Shared.Dtos;
using System.Text.Json;

namespace Services.Basket.Services
{

    public class BasketService : IBasketService
    {
        private readonly RedisService _redisService;

        public BasketService(RedisService redisService)
        {
            _redisService = redisService;
        }

        public async Task<ResponseDto<bool>> Delete(string userId)
        {
            var status = await _redisService.GetDb().KeyDeleteAsync(userId);
            return status ? ResponseDto<bool>.Success(204) : ResponseDto<bool>.Failed("Basket couldn't delete", 404);


        }

        public async Task<ResponseDto<BasketDto>> GetBasket(string userId)
        {
            var isExistBasket = await _redisService.GetDb().StringGetAsync(userId);

            return string.IsNullOrEmpty(isExistBasket)
                 ? ResponseDto<BasketDto>.Failed("Basket not found", 404)
                 : ResponseDto<BasketDto>.Success(JsonSerializer.Deserialize<BasketDto>(isExistBasket)
                 ?? throw new InvalidOperationException("Deserialization returned null"),
                 200);

        }

        public async Task<ResponseDto<bool>> SaveOrUpdate(BasketDto basketDto)
        {
            var status = await _redisService.GetDb().StringSetAsync(basketDto.UserId, JsonSerializer.Serialize(basketDto));

            return status ? ResponseDto<bool>.Success(204) : ResponseDto<bool>.Failed("Basket couldn't update or save", 500);
        }
    }
}
