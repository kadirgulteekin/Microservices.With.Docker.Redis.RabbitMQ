using Dapper;
using Npgsql;
using Services.Discount.Models;
using Shared.Dtos;
using System.Data;
using System.Reflection.Metadata.Ecma335;

namespace Services.Discount.Services
{
    public class DiscountService : IDiscountService
    {

        private readonly IConfiguration _configuration;
        private readonly IDbConnection _dbConnection;

        public DiscountService(IConfiguration configuration)
        {
            _configuration = configuration;
            _dbConnection = new NpgsqlConnection(_configuration.GetConnectionString("PostreSql"));
        }

        public async Task<ResponseDto<NoContent>> Delete(int id)
        {
            var status = await _dbConnection.ExecuteAsync("DELETE FROM discount where id=@Id", new { Id = id });

            return status > 0 ? ResponseDto<NoContent>.Success(204) : ResponseDto<NoContent>.Failed("Discount not found", 404);

        }

        public async Task<ResponseDto<List<Models.Discount>>> GetAll()
        {
            var discount = await _dbConnection.QueryAsync<Models.Discount>("Select * from discount");
            return ResponseDto<List<Models.Discount>>.Success(discount.ToList(),200);

        }

        public async Task<ResponseDto<Models.Discount>> GetByCodeAndUserId(string code, string userId)
        {
            var isExistingDiscount = (await _dbConnection.QueryAsync<Models.Discount>("Select * from discount where userid = @UserId and code = @Code" , new {UserId= userId,Code = code})).FirstOrDefault();

            return isExistingDiscount == null ? ResponseDto<Models.Discount>.Failed("Discount not found", 404) :
                ResponseDto<Models.Discount>.Success(isExistingDiscount,200);
            
        }

        public async Task<ResponseDto<Models.Discount>> GetById(int id)
        {
            var discounts = (await _dbConnection.QueryAsync<Models.Discount>("Select * from discount where id=@Id", new { Id=id })).SingleOrDefault();
            if(discounts is null)
            {
                return ResponseDto<Models.Discount>.Failed("Discoutn not found!", 404);
            }

            return ResponseDto<Models.Discount>.Success(discounts, 200);
        }

        public async Task<ResponseDto<NoContent>> Save(Models.Discount discount)
        {
            var saveStatus = await _dbConnection.ExecuteAsync("INSERT INTO discount (userid,rate,code) VALUES " +
                "(@UserId,@Rate,@Code)",discount);
            if (saveStatus>0)
            {
                return ResponseDto<NoContent>.Success(200);

            }
            return ResponseDto<NoContent>.Failed("Discount could not save!",500);
        }

        public async Task<ResponseDto<NoContent>> Update(Models.Discount discount)
        {
            var isExistingDiscount = (await _dbConnection.QueryAsync<Models.Discount>("Select * from discount where id = @Id", discount)).SingleOrDefault();

            if(isExistingDiscount is null)
            {
                return ResponseDto<NoContent>.Failed("could not found discount", 404);
            }

            var status = await _dbConnection.ExecuteAsync("UPDATE discount set userid = @UserId," +
                "code = @Code,rate = @Rate where id=@Id",new
                {
                    Id = discount.Id,
                    UserId = discount.UserId,
                    Code = discount.Code,
                    Rate = discount.Rate
                });
            if (status>0)
            {
                return ResponseDto<NoContent>.Success(204);
            }
            return ResponseDto<NoContent>.Failed("an error occurred while updating",500);
        }
    }
}
