using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SMS.Model;
using Swashbuckle.AspNetCore.Swagger;

namespace SMS
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            PlivoCodingAssignmentContext.ConnectionString = Configuration.GetConnectionString("SMSAPIConnectionString");
            CacheHandler.CacheServiceBaseUri = Configuration.GetValue<string>("CacheServiceBaseUri");

            // Add framework services.
            services.AddDbContext<PlivoCodingAssignmentContext>();
            services.AddScoped<AuthenticationService>();

            services.AddCors(o => o.AddPolicy("PlivoSMSPolicy", builder =>
            {
                builder.AllowAnyOrigin()
                       .AllowAnyMethod()
                       .AllowAnyHeader();
            }));

            services.AddMvc();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info { Title = "Plivo SMS API V1.0", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            app.UseWhen(x => (x.Request.Path.StartsWithSegments("/api/inbound/sms", StringComparison.OrdinalIgnoreCase)),
            builder =>
            {
                builder.UseMiddleware<AuthenticationService>();
            });

            app.UseWhen(x => (x.Request.Path.StartsWithSegments("/api/outbound/sms", StringComparison.OrdinalIgnoreCase)),
            builder =>
            {
                builder.UseMiddleware<AuthenticationService>();
            });

            app.UseCors("PlivoSMSPolicy");

            app.UseMvc();
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Plivo SMS API V1.0");
            });
        }
    }
}
