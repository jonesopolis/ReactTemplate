using System.IO;
using System.Text;
using Db;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

namespace Web
{
    public class Startup
    {
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseIdentity();
            app.UseJwtBearerAuthentication(new JwtBearerOptions
                                           {
                                               AutomaticAuthenticate = true,
                                               AutomaticChallenge = true,
                                               TokenValidationParameters = new TokenValidationParameters
                                                                           {
                                                                               IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration.GetSection("AppConfiguration:Key").Value)),
                                                                               ValidAudience = Configuration.GetSection("AppConfiguration:SiteUrl").Value,
                                                                               ValidateIssuerSigningKey = true,
                                                                               ValidateLifetime = true,
                                                                               ValidIssuer = Configuration.GetSection("AppConfiguration:SiteUrl").Value
                                                                           }
                                           });

            app.UseStaticFiles();
            app.UseMvcWithDefaultRoute();
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<Context>(options => options.UseSqlServer(@"Server=(localdb)\mssqllocaldb;Database=MyDb;Trusted_Connection=True"));

            services.AddIdentity<User, IdentityRole>()
                    .AddEntityFrameworkStores<Context>();


            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json");

            Configuration = builder.Build();

            services.AddSingleton(Configuration);
            services.AddMvc();

            services.Configure<IdentityOptions>(options =>
                                                {
                                                    options.Password.RequireDigit = false;
                                                    options.Password.RequiredLength = 3;
                                                    options.Password.RequireNonAlphanumeric = false;
                                                    options.Password.RequireUppercase = false;
                                                    options.Password.RequireLowercase = false;
                                                    options.Cookies.ApplicationCookie.LoginPath = "/";
                                                });
        }

        public IConfigurationRoot Configuration { get; set; }

        public static void Main(string[] args)
        {
            var host = new WebHostBuilder()
                .UseKestrel()
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseIISIntegration()
                .UseStartup<Startup>()
                .UseApplicationInsights()
                .Build();

            host.Run();
        }
    }
}