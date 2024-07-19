using AutoMapper;
using MongoDB.Driver;
using Services.Catalog.Configuration;
using Services.Catalog.Dtos;
using Services.Catalog.Model;
using Shared.Dtos;

namespace Services.Catalog.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly IMongoCollection<Category> _mongoCollection;
        private readonly IMapper _mapper;

        public CategoryService(IMapper mapper, IDatabaseConfigurations _databaseSettings)
        {
            var client = new MongoClient(_databaseSettings.ConnectionString);
            var database = client.GetDatabase(_databaseSettings.DatabaseName);
            _mongoCollection = database.GetCollection<Category>(_databaseSettings.CategoryCollectionName);   
            _mapper = mapper;
        }

        public async Task<ResponseDto<List<CategoryDto>>> GetAllAsync()
        {
            var categories = await _mongoCollection.Find(FilterDefinition<Category>.Empty).ToListAsync();

            return ResponseDto<List<CategoryDto>>.Success(_mapper.Map<List<CategoryDto>>(categories),StatusCodes.Status200OK);
        }

        public async Task<ResponseDto<CategoryDto>> CreateCategoryAsync(CategoryDto categoryDto)
        {
            var category = _mapper.Map<Category>(categoryDto);

            await _mongoCollection.InsertOneAsync(category);

            return ResponseDto<CategoryDto>.Success(_mapper.Map<CategoryDto>(category), StatusCodes.Status201Created);

        }

        public async Task<ResponseDto<CategoryDto>> GetCategoryByIdAsync(string id)
        {
            var isExistCategory = await _mongoCollection.Find<Category>(category => category.Id == id).FirstOrDefaultAsync();
            
            if(isExistCategory == null)
            {
               return ResponseDto<CategoryDto>.Failed("CategoryNotFound",404);
            }

            return ResponseDto<CategoryDto>.Success(_mapper.Map<CategoryDto>(isExistCategory), StatusCodes.Status200OK);



        }
    }
}
