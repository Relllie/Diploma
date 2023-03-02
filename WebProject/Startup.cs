using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using WebProject.Domain.Models;
using WebProject.Domain.Repositories;
using WebProject.Persistence.Contexts;
using WebProject.Persistence.Repositories;
using WebProject.Domain.Services;
using WebProject.Domain.Models.Books;

namespace WebProject
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();

            services.AddDbContext<BiblioDbContext>(options =>
            {
                options.UseSqlite(Configuration.GetConnectionString("SqliteConnection"));
            });

            services.AddScoped<IUnitOfWork, UnitOfWork>();

            services.AddScoped<IRepository<Role>, RoleRepository>();
            services.AddScoped<IRepository<User>, UserRepository>();
            services.AddScoped<IRepository<UserRole>, UserRoleRepository>();

            services.AddScoped<IRepository<Genre>, GenreRepository>();
            services.AddScoped<IRepository<Book>, BookRepository>();
            services.AddScoped<IRepository<BooksOnShelves>, BooksOnShelvesRepository>();
            services.AddScoped<IRepository<TakenBooks>, TakenBooksRepository>();

            services.AddScoped<DemoDataService>();

            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.LoginPath = new Microsoft.AspNetCore.Http.PathString("/Account/Login");
                    options.AccessDeniedPath = new Microsoft.AspNetCore.Http.PathString("/Account/Login");
                });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, BiblioDbContext context, DemoDataService demoDataService)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                context.Database.EnsureDeleted();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            if (context.Database.EnsureCreated())
            {
                demoDataService.Generate().Wait();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
