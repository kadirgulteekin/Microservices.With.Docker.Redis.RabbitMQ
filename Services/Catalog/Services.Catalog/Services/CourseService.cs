using AutoMapper;
using MongoDB.Driver;
using Services.Catalog.Configuration;
using Services.Catalog.Dtos;
using Services.Catalog.Model;
using Shared.Dtos;

namespace Services.Catalog.Services
{
    public class CourseService : ICourseService
    {
        private readonly IMongoCollection<Course> _courseCollection;
        private readonly IMongoCollection<Category> _categoryCollection;

        private readonly IMapper _mapper;

        public CourseService(IMapper mapper,IDatabaseConfigurations _databaseConfiguration)
        {
            var client = new MongoClient(_databaseConfiguration.ConnectionString);
            var database = client.GetDatabase(_databaseConfiguration.DatabaseName);
            _courseCollection = database.GetCollection<Course>(_databaseConfiguration.CourceCollectionName);
            _categoryCollection = database.GetCollection<Category>(_databaseConfiguration.CategoryCollectionName);
            _mapper = mapper;
        }

        public async Task<ResponseDto<List<CourseDto>>> GetAllAsync()
        {
            var courses = await _courseCollection.Find(FilterDefinition<Course>.Empty).ToListAsync();

            if(courses != null && courses.Count>0)
            {
                var categories = await _categoryCollection.Find(FilterDefinition<Category>.Empty).ToListAsync();

                var categoryDictionary = categories
                    .Where(c=>c.Id != null)
                    .ToDictionary(c => c.Id!, c => c);

                foreach (var course in courses)
                {
                    if (course.CategoryId != null && categoryDictionary.TryGetValue(course.CategoryId, out var category))
                    {
                        course.Category = category;
                    }
                }
            }
            return ResponseDto<List<CourseDto>>.Success(_mapper.Map<List<CourseDto>>(courses), StatusCodes.Status200OK);
        }

        public async Task<ResponseDto<CourseDto>> GetByIdAsync(string id)
        {
            var courses = await _courseCollection.Find<Course>(x => x.Id == id).FirstOrDefaultAsync();

            if (courses == null)
            {
                return ResponseDto<CourseDto>.Failed("CategoryNotFound", 404);
            }
            courses.Category = await _categoryCollection.Find<Category>(x => x.Id == courses.Id).FirstOrDefaultAsync();

            return ResponseDto<CourseDto>.Success(_mapper.Map<CourseDto>(courses), StatusCodes.Status200OK);
        }

        public async Task<ResponseDto<List<CourseDto>>> GetAllByUserIdAsyc(string userId)
        {
            var courses = await _courseCollection.Find<Course>(x => x.UserId == userId).ToListAsync();

            if (courses != null && courses.Count > 0)
            {
    
                foreach (var course in courses)
                {
                    var categories = await _categoryCollection.Find<Category>(x=>x.Id == course.CategoryId).ToListAsync();

                    var categoryDictionary = categories
                    .Where(c => c.Id != null)
                    .ToDictionary(c => c.Id!, c => c);


                    if (course.CategoryId != null && categoryDictionary.TryGetValue(course.CategoryId, out var category))
                    {
                        course.Category = category;
                    }
                }

            }

            return ResponseDto<List<CourseDto>>.Success(_mapper.Map<List<CourseDto>>(courses), StatusCodes.Status200OK);
        }

        public async Task<ResponseDto<CourseDto>> CreateCourse(CourseCreateDto courseCreateDto)
        {
            var newCourse = _mapper.Map<Course>(courseCreateDto);

            newCourse.CreatedDate = DateTime.Now;

            await _courseCollection.InsertOneAsync(newCourse);

            return ResponseDto<CourseDto>.Success(_mapper.Map<CourseDto>(newCourse),StatusCodes.Status200OK);
        }

        public async Task<ResponseDto<NoContent>> Update(CourseUpdateDto courseUpdateDto)
        {
            var updateCourse = _mapper.Map<Course>(courseUpdateDto);

            var result = await _courseCollection.FindOneAndReplaceAsync(x => x.Id == courseUpdateDto.Id, updateCourse);

            if(result ==null)
            {
                return ResponseDto<NoContent>.Failed("Course Not Found", StatusCodes.Status404NotFound);
            }

            return ResponseDto<NoContent>.Success(StatusCodes.Status200OK);

        }

        public async Task<ResponseDto<NoContent>> DeleteAsync(string id)
        {
            var courses = await _courseCollection.Find<Course>(x => x.Id == id).FirstOrDefaultAsync();

            if (courses == null)
            {
                return ResponseDto<NoContent>.Failed("CategoryNotFound", 404);
            }

            var result = await _courseCollection.DeleteOneAsync(x=>x.Id == courses.Id);

            return ResponseDto<NoContent>.Success(StatusCodes.Status200OK);
        }
    }
}
