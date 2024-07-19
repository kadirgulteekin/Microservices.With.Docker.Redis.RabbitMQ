using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Services.Catalog.Dtos;
using Services.Catalog.Model;
using Services.Catalog.Services;
using Shared.ControllerBases;

namespace Services.Catalog.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CatalogController : CustomBaseController
    {
        private readonly ICategoryService _categoryService;

        public CatalogController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }
        [HttpGet]
        public async Task<IActionResult> GetAllAsync()
        {
            var response = await _categoryService.GetAllAsync();
            return CreateActionResultInstance(response);
        }
        [HttpPost]
        public async Task<IActionResult> CreateCategoryAsync(CategoryDto categoryDto)
        {
            var response = await _categoryService.CreateCategoryAsync(categoryDto);
            return CreateActionResultInstance(response);

        }


        [HttpGet("{id}")]
        public async Task<IActionResult> GetCategoryByIdAsync(string id)
        {
            var response = await _categoryService.GetCategoryByIdAsync(id);
            return CreateActionResultInstance(response);

        }


    }
}
