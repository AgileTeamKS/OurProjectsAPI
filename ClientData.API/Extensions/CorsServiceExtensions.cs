using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ClientData.API.Extensions
{
    public static class CorsServiceExtensions
    {
        static readonly string MyAllowSpecificOrigins = "_myAllowSpecificOrigins";
        public static void ConfigureCors(this IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy(MyAllowSpecificOrigins,
                   builder =>
                   {
                       builder.WithOrigins("http://103.228.144.82/OurProject2021").AllowAnyHeader().AllowAnyMethod();
                   });
            });
        }
    }
}
