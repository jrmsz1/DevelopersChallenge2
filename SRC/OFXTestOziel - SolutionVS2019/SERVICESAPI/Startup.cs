using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SERVICESAPI.DataAccess;
using SERVICESAPI.DataAccess.Repository;
using SERVICESAPI.Proccess;
using SERVICESAPI.Proccess.Interfaces;

namespace SERVICESAPI
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

            //To do in the future:
            //Add JWT authentication
            //Unit of work

            //Oziel - For use EntityFramework with SQL SERVER
            /* services.AddEntityFrameworkSqlServer()
                 .AddDbContext<OFXContext>(
                     options => options.UseSqlServer(
                         Configuration.GetConnectionString("ConnectionMSSQL")));*/

            //Oziel - For use EntityFramework with Postgres
            services.AddEntityFrameworkNpgsql()
                .AddDbContext<OFXContext>(
                    options => options.UseNpgsql(
                        Configuration.GetConnectionString("ConnectionPostgres")));

            //Oziel - Dependency Injection 
            services.AddScoped<IProcessFileOFX, ProcessFileOFX>();

            //Oziel - AutoMapper
            services.AddAutoMapper(typeof(Startup));
            
            //Oziel - Pattern Unit Of Work
            services.AddTransient<IUnitOfWork, UnitOfWork>();

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseMvc();
        }
    }
}