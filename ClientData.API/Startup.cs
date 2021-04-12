using System.Text;
using System.Threading.Tasks;
using ClientData.API.Data;
using ClientData.API.Extensions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

namespace ClientData.API
{
    public class Startup
    {
        //readonly string MyAllowSpecificOrigins = "_myAllowSpecificOrigins";
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            string cnnString = Configuration.GetConnectionString("DefaultConnection");
            //Provide sertices as well as connection string to DAL and injact DAL
            DAL.Configure.ConfigureServices(services, cnnString);
            services.AddControllers().AddNewtonsoftJson();

            //To add Swagger middleware
            services.AddSwaggerGen();

            //To access Applicaiton Specific Settings from appsettings.json using dependency injection
            ////https://docs.microsoft.com/en-us/aspnet/core/fundamentals/configuration/options?view=aspnetcore-3.
            services.Configure<MyAppSettingsOptions>(Configuration.GetSection(MyAppSettingsOptions.MyAppSettings));
            services.Configure<TokenSettingsOptions>(Configuration.GetSection(TokenSettingsOptions.TokenSettings));
            services.AddScoped < TokenGenerator > ();

            services.AddDbContext<ApplicationDBContext>(options => options.UseSqlServer(cnnString));
            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDBContext>()
                .AddDefaultTokenProviders();


            //To enable CORS
            //services.AddCors(options =>
            //{
            //    options.AddPolicy(MyAllowSpecificOrigins,
            //        builder =>
            //        {
            //            builder.WithOrigins("http://localhost:4200").AllowAnyHeader().AllowAnyMethod();
            //        });
            //});
            //Added as an Extension method 
            //Learning refefence - https://code-maze.com/net-core-web-development-part2/
            services.ConfigureCors();


            services.AddAuthentication(
                options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                   options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;

                }
                ).AddJwtBearer(
                options =>
                {
                    options.SaveToken = true;
                    options.RequireHttpsMetadata = false;
                    options.Audience = "ExamProc2020";
                    //options.Authority = "https://localhost:44372/";//Createing error
                    
                    options.TokenValidationParameters = new TokenValidationParameters()
                    {
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = "ExamProcAPI",
                        ValidAudience = "ExamProc2020",
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime=true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("aaaaaaaaaaaaaaaa"))
                    };
                    options.Events = new JwtBearerEvents
                    {

                        //OnMessageReceived Invoked when a protocol message is first received.
                        //https://docs.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.authentication.jwtbearer.jwtbearerevents?view=aspnetcore-3.0
                         
                        OnMessageReceived = context =>
                        {
                            //context is an object of MessageReceivedContext class. Contains method name Fail to indicate failure in authentication
                            
                            var logger = NLog.Web.NLogBuilder.ConfigureNLog("nlog.config").GetCurrentClassLogger();

                            //Getting null value always from context.Token and context.Principal 
                            var accessToken = context.Token;
                            var principal = context.Principal;
                            var user = context.HttpContext.User;
                            var request = context.Request;
                            logger.Info("Token: " + request.Headers["Authorization"]);
                            logger.Info("User-Agent: " + request.Headers["User-Agent"]);

                            logger.Info("origin: " + request.Headers["origin"]);
                            logger.Info("Message Received from: " + request.Headers["Referer"]);
                            logger.Info("Host: " + request.Headers["Host"]);
                            var result = context.Result;
                            var authenicationProperties =  context.Properties;
                            var data = context.Options.SecurityTokenValidators[0].ToString();

                            

                            //Fail method - Indicates that there was a failure during authentication.
                            //context.Fail("Invalid token");

                            return Task.CompletedTask;
                        },
                        OnTokenValidated = context =>
                        {
                            
                            var logger = NLog.Web.NLogBuilder.ConfigureNLog("nlog.config").GetCurrentClassLogger();
                            //To determine that request received from 
                            var origin = context.Request.Headers["origin"];

                            logger.Info(" OnTokenValidated Message Received from: " + origin);


                            var accessToken = context.SecurityToken;
                            return Task.CompletedTask;
                        },
                        OnAuthenticationFailed = context =>
                        {

                            var logger = NLog.Web.NLogBuilder.ConfigureNLog("nlog.config").GetCurrentClassLogger();
                            logger.Info("OnAuthenticationFailed Executed ");
                            var excep = context.Exception;
                            logger.Error(context.Exception, " Raised Exception from OnAuthenticationFailed ");
                            return Task.CompletedTask;
                        },
                        OnChallenge = context =>
                        {
                            var logger = NLog.Web.NLogBuilder.ConfigureNLog("nlog.config").GetCurrentClassLogger();
                            logger.Info(" OnChallenge ");
                            var error = context.Error;
                            
                            logger.Error(error, " Error from OnChallenge");
                            return Task.CompletedTask;
                        },
                        OnForbidden = context =>
                        {
                            var logger = NLog.Web.NLogBuilder.ConfigureNLog("nlog.config").GetCurrentClassLogger();
                            logger.Info(" OnForbidden ");
                            var result = context.Result;
                            return Task.CompletedTask;
                        }
                        

                    };
                });
            
            
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {

                // app.UseDeveloperExceptionPage("");
                app.UseExceptionHandler("/error");
            }
            app.UseHttpsRedirection();

            //to use Swagger
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("v1/swagger.json","Our Project API V1");
            });

            app.UseRouting();
            //To enable CORS
            app.UseCors("_myAllowSpecificOrigins");

            app.UseAuthentication();
          

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers().RequireCors("_myAllowSpecificOrigins");
                //endpoints.MapControllers();
            });
        }
    }
}
