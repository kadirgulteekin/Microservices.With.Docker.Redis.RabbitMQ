using Services.Catalog.Dtos;
using Services.Catalog.Model;
using Shared.Dtos;

namespace Services.Catalog.Services
{
    public interface ICategoryService
    {
        Task<ResponseDto<List<CategoryDto>>> GetAllAsync();
        Task<ResponseDto<CategoryDto>> CreateCategoryAsync(CategoryDto categoryDto);
        Task<ResponseDto<CategoryDto>> GetCategoryByIdAsync(string id);

       
    }

}
