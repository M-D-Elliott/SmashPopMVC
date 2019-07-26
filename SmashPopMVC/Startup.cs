using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SmashPopMVC.Data;
using SmashPopMVC.Data.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Rewrite;
using SmashPopMVC.Data.Sync;
using SmashPopMVC.Service.Sync;
using SmashPopMVC.Services.Sync;
//using SmashPopMVC.Data.Async;
//using SmashPopMVC.Service.Async;
//using SmashPopMVC.Services.Async;
using SmashPopMVC.Services;
using Microsoft.AspNetCore.Mvc.Razor;

namespace SmashPopMVC
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
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();
            
            services.AddMemoryCache();

            // Add application services.
            services.AddTransient<IEmailSender, EmailSender>();
            services.AddScoped<IGame, GameService>();
            services.AddScoped<ICharacter, CharacterService>();
            services.AddScoped<IApplicationUser, ApplicationUserService>();
            services.AddScoped<IComment, CommentService>();
            services.AddScoped<ICommentPackager, CommentPackager>();
            services.AddScoped<IVote, VoteService>();
            services.AddScoped<ITally, TallyService>();
            services.AddScoped<IFriend, FriendService>();
            services.AddScoped<ITally, TallyService>();

            var policy = new AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser()
                //.RequireRole("Admin", "SuperUser")
                .Build();

            services.Configure<RazorViewEngineOptions>(options =>
            {
                options.AreaViewLocationFormats.Clear();
                options.AreaViewLocationFormats.Add("/Views/{1}/{0}.cshtml");
            });

            services.AddMvc(options =>
            {
                options.Filters.Add(new AuthorizeFilter(policy));
                options.Filters.Add(new RequireHttpsAttribute());
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseBrowserLink();
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();
            app.UseAuthentication();

            // Force all http to redirect to https.
            var options = new RewriteOptions()
                .AddRedirectToHttps();

            app.UseRewriter(options);

            app.UseMvc(routes =>
            {
                routes.MapAreaRoute(
                    name: "AsyncControllers",
                    areaName: "Async",
                    template: "Async/{controller=Home}/{action=Index}/{id?}");

                routes.MapAreaRoute(
                    name: "SyncControllers",
                    areaName: "Sync",
                    template: "{controller=Home}/{action=Index}/{id?}");

                routes.MapAreaRoute(
                    name: "SharedControllers",
                    areaName: "Shared",
                    template: "{controller=Home}/{action=Index}/{id?}");

                routes.MapRoute(
                   name: "default",
                   template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
