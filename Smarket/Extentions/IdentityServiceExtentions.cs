using Microsoft.AspNetCore.Identity;
using Smarket.DataAccess;
using Smarket.Models;


namespace Smarket.Extentions
{
	public static class IdentityServiceExtentions
	{
		public static IServiceCollection AddIdentityService(this IServiceCollection services, IConfiguration configuration)
		{
			services.AddIdentityCore<User>(option =>
			{
				option.Password.RequireNonAlphanumeric = false;
				option.SignIn.RequireConfirmedEmail = false;
				option.SignIn.RequireConfirmedPhoneNumber = false;
			})
			 .AddRoles<Role>()
			 .AddRoleManager<RoleManager<Role>>()
			 .AddSignInManager<SignInManager<User>>()
			 .AddRoleValidator<RoleValidator<Role>>()
			 .AddEntityFrameworkStores<ApplicationDbContext>()
			 .AddDefaultTokenProviders();
			services.AddAuthentication(options =>
			{
				options.DefaultScheme = IdentityConstants.ApplicationScheme;
				options.DefaultAuthenticateScheme = IdentityConstants.ApplicationScheme;
				options.DefaultChallengeScheme = IdentityConstants.ApplicationScheme;

			})
			.AddCookie("Identity.Application")
			.AddCookie("Identity.External")
			.AddCookie("Identity.TwoFactorRememberMe")
			.AddCookie("Identity.TwoFactorUserId");






            return services;
		}
	}
}