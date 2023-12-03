using Microsoft.EntityFrameworkCore;
using Smarket.DataAccess;
using Smarket.DataAccess.Repository;
using Smarket.DataAccess.Repository.IRepository;
using Smarket.Services.IServices;
using Smarket.Services;
using Smarket.Settings;

namespace Smarket.Extentions
{
    public static class ApplicationServiceExtentions
    {
        public static IServiceCollection AddApplicationService(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
                 options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));
            services.AddScoped<IUnitOfWork, UnitOfWork>();

           services.AddAutoMapper(typeof(Program).Assembly);
            services.Configure<StripeSettings>(configuration.GetSection("Stripe"));
            services.AddTransient<ITokenService, TokenService>();
            services.AddSingleton<IEmailService, EmailService>();
            services.Configure<CloudinarySettings>(configuration.GetSection("CloudinarySettings"));
            services.AddScoped<IImageService, ImageService>();

            return services;
        }
    }
}