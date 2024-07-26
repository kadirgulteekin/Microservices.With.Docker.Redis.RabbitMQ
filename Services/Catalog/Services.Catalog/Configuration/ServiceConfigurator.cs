
using Services.Catalog.Services;
using Shared.Services;

namespace Services.Catalog.Configuration
{
    public class ServiceConfigurator
    {
        public static void Configure(IServiceCollection serviceDescriptors)
        {
            serviceDescriptors.AddScoped<ICategoryService,CategoryService>();
            serviceDescriptors.AddScoped<ICourseService,CourseService>();
           
        }
    }
}
