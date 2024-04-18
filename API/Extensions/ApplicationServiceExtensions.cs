
using API.Data;
using API.Interface;
using API.Services;
using Microsoft.EntityFrameworkCore;

namespace API.Extensions
{
    public static class ApplicationServiceExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration config)
        {
            services.AddDbContext<DataContext>(opts=>
            {
            opts.UseSqlite(config.GetConnectionString("DefaultConnection"));
            });

            services.AddCors();
            //Nedd to add service so our application can use it
            services.AddScoped<ITokenService, TokenService>();
            return services;
        }
    }
}