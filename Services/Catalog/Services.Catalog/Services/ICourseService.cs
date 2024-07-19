using Services.Catalog.Dtos;
using Shared.Dtos;

namespace Services.Catalog.Services
{
    public interface ICourseService
    {
        Task<ResponseDto<List<CourseDto>>> GetAllAsync();
        Task<ResponseDto<CourseDto>> GetByIdAsync(string id);
        Task<ResponseDto<List<CourseDto>>> GetAllByUserIdAsyc(string userId);
        Task<ResponseDto<CourseDto>> CreateCourse(CourseCreateDto courseCreateDto);
        Task<ResponseDto<NoContent>> Update(CourseUpdateDto courseUpdateDto);
        Task<ResponseDto<NoContent>> DeleteAsync(string id);
    }
}
