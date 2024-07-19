using Microsoft.AspNetCore.Mvc;
using Services.Catalog.Dtos;
using Services.Catalog.Services;
using Shared.ControllerBases;

namespace Services.Catalog.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CourseController : CustomBaseController
    {
        private readonly ICourseService _courseService;

        public CourseController(ICourseService courseService)
        {
            _courseService = courseService;
        }
        [HttpGet]
        public async Task<IActionResult> GetAllAsync()
        {
            var response = await _courseService.GetAllAsync();
            return CreateActionResultInstance(response);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var response = await _courseService.GetByIdAsync(id);

            return CreateActionResultInstance(response);
        }

        [HttpGet("GetAllByUserId/{userId}")]
        public async Task<IActionResult> GetAllByUserId(string userId)
        {
            var response = await _courseService.GetAllByUserIdAsyc(userId);

            return CreateActionResultInstance(response);
        }

        [HttpPost]
        public async Task<IActionResult> CreateAsync(CourseCreateDto createCourseDto)
        {
            var response = await _courseService.CreateCourse(createCourseDto);
            return CreateActionResultInstance(response);
        }
        [HttpPut]
        public async Task<IActionResult> UpdateAsync(CourseUpdateDto courseUpdateDto)
        {
            var response = await _courseService.Update(courseUpdateDto);
            return CreateActionResultInstance(response);
        }
        [HttpDelete]
        public async Task<IActionResult> DeleteteAsync(string id)
        {
            var response = await _courseService.DeleteAsync(id);
            return CreateActionResultInstance(response);
        }
    }
}
