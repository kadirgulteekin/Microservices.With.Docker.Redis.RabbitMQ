using AutoMapper;
using Services.Catalog.Dtos;
using Services.Catalog.Model;

namespace Services.Catalog.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Course,CourseDto>().ReverseMap();
            CreateMap<Category,CategoryDto>().ReverseMap();
            CreateMap<Feature, FeatureDto>().ReverseMap();

            CreateMap<Course,CourseCreateDto>().ReverseMap();
            CreateMap<Course,CourseUpdateDto>().ReverseMap();
            
        }
    }
}
