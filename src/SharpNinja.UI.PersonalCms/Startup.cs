using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.AzureAD.UI;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using RapidCMS.Core.Abstractions.Config;
using RapidCMS.Repositories;
using RapidCMS.UI.Extensions;

using SharpNinja.UI.PersonalCms.Components;
using SharpNinja.UI.PersonalCms.Data;
using SharpNinja.UI.PersonalCms.Shared;

namespace SharpNinja.UI.PersonalCms
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddAuthentication(AzureADDefaults.AuthenticationScheme)
                .AddAzureAD(options => Configuration.Bind("AzureAd", options));

            services.AddControllersWithViews(options =>
            {
                var policy = new AuthorizationPolicyBuilder()
                    .RequireAuthenticatedUser()
                    .Build();
                options.Filters.Add(new AuthorizeFilter(policy));
            });

            services.AddRazorPages();
            services.AddServerSideBlazor();
            services.AddRapidCMS(config =>
            {
                config.AllowAnonymousUser();

                config.SetCustomLoginStatus(typeof(LoginDisplay));

                config.SetSiteName("Personal CMS");

                config.SetCustomLoginStatus(typeof(LoginStatus));

                config.AddCollection<Video, JsonRepository<Video>>("video", "Video", 
                    collection =>
                {
                    collection
                        .SetTreeView(x => x.VideoName)
                        .SetListView(view =>
                        {
                            view.AddRow(row =>
                            {
                                row.AddField(p => p.Id);
                                row.AddField(p => p.VideoName);
                                row.AddField(p => p.VideoUrl);
                            });
                        });
                });

                config.Dashboard.AddSection(typeof(DashboardComponent));
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapBlazorHub();
                endpoints.MapFallbackToPage("/_Host");
            });
        }
    }
}
