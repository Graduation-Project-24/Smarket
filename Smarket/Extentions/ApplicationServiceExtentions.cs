using Microsoft.EntityFrameworkCore;
using Smarket.DataAccess;
using Smarket.DataAccess.Repository;
using Smarket.DataAccess.Repository.IRepository;

namespace Smarket.Extentions
{
    public static class ApplicationServiceExtentions
    {
        public static IServiceCollection AddApplicationService(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
                 options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            return services;
        }
    }
}