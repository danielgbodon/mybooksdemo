using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MyBooks.Core;
using MyBooks.Core.BookServices;
using MyBooks.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using MyBooks.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using MyBooks.Controllers;
using MyBooks.Core.Interfaces;
using Microsoft.AspNetCore.Http;
using MyBooks.Core.Middlewares;
using Microsoft.AspNetCore.Localization;
using System.Globalization;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.DataAnnotations;
using MyBooks.Core.Validations;

namespace MyBooks
{
    public class Startup
    {
        public IWebHostEnvironment Env { get; set; }

        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            Configuration = configuration;
            Env = env;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //INTERFACES IMPLEMENTATION
            services.AddControllersWithViews();
            services.AddScoped<IBookService, GoogleBookAPI>();
            services.AddScoped<IUser, ApplicationUser>();
            services.AddScoped<IBook, Book>();
            services.AddScoped<IBookList, BookList>();
            services.AddScoped<ICategory, Category>();
            services.AddScoped<IAuthor, Author>();
            services.AddScoped<IPublisher, Publisher>();
            services.AddTransient<IHttpContextAccessor, HttpContextAccessor>();
            services.AddSingleton<IValidationAttributeAdapterProvider, CustomValidationAttributeAdapterProvider>();

            //MULTILANGUAGE SUPPORT
            services.AddLocalization(options => options.ResourcesPath = "Resources/Languages");

            IList<CultureInfo> supportedCultures = AppEnvironment.SupportedCultures.Values.Select(sc => new CultureInfo(sc)).ToList();

            services.Configure<RequestLocalizationOptions>(options =>
            {
                options.DefaultRequestCulture = new RequestCulture(culture: supportedCultures.First(), uiCulture: supportedCultures.First());
                options.SupportedCultures = supportedCultures;
                options.SupportedUICultures = supportedCultures;
                options.RequestCultureProviders.Clear();
                //CUSTOM CULTURE PROVIDER BASED IN USER DATA IN DATABASE
                options.RequestCultureProviders.Add(new UserBasedRequestCultureProvider());
            });

            services.AddMvc()
                .AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix)
                .AddDataAnnotationsLocalization(o =>
                {
                    //MULTILANGUAGE ERRORS
                    o.DataAnnotationLocalizerProvider = (type, factory) =>
                    {
                        return factory.Create(typeof(ValidationResources));
                    };
                });

            //DATABASE
            services.AddDbContext<MyBooksContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("MyBooksDB")));

            //IDENTITY SETTINGS
            services.AddDefaultIdentity<UserModel>(options =>
            {
                options.SignIn.RequireConfirmedAccount = false;
                options.Password.RequiredLength = 6;
                options.Password.RequireDigit = true;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequiredUniqueChars = 3;
            })
            .AddErrorDescriber<LocalizedIdentityErrorDescriber>() //MULTILINGUAL ERROR MESSAGES
            .AddRoles<IdentityRole>()
            .AddEntityFrameworkStores<MyBooksContext>()
            .AddDefaultUI();

            //CUSTOM AUTHENTICATION REDIRECTION PATHS 
            services.ConfigureApplicationCookie(o =>
            {
                o.LoginPath = "/user/login";
                o.LogoutPath = "/user/logout";
                o.AccessDeniedPath = "/forbidden";
            });

            services.AddControllers(config =>
            {
                var policy = new AuthorizationPolicyBuilder()
                                 .RequireAuthenticatedUser()
                                 .Build();
                config.Filters.Add(new AuthorizeFilter(policy));
            });

            IMvcBuilder builder = services.AddRazorPages();

            #if DEBUG
            if (Env.IsDevelopment())
            {
                builder.AddRazorRuntimeCompilation();
            }
            #endif
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app)
        {
            if (Env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseAuthentication();

            app.UseRequestLocalization();

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseUserLogged();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
