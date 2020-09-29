using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Planny.Controllers;
using Planny.Data.Models;
using Planny.DataContext;

namespace Planny
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddDbContext<PlannyContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("connectionString")));
            services.Configure<IISServerOptions>(options =>
            {
                options.AutomaticAuthentication = true;
            });

            services.AddCors(o => o.AddPolicy("PolicaZaTestiranje", builder =>
            {
                builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
            }));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseCors("PolicaZaTestiranje");
            //app.UseMiddleware<KorisnikController>();


            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseDeveloperExceptionPage();
            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
