using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MvcExample.AuthenticationMock;
using MvcExample.Database;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace MvcExample
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
            services.AddControllersWithViews();

            services.AddDbContext<DataDbContext>(opt =>
            {
                opt.UseInMemoryDatabase("database");
            });

            //Add OPA
            services.AddOpa(opt => 
                //Add policy and data sync
                opt.AddSync(s =>
                    // Add syncs here, such as syncing with an existing OPA server
                    //s.UseOpaServer("opaServerUrl")

                    //Add a local policy
                    s.UseLocal(local =>
                    {
                        var policy = File.ReadAllText("policy.rego");
                        local.AddPolicy(policy);
                    })
                )
            );

            // Mocking a logged in user just for the sample
            services.AddAuthentication("BasicAuthentication")
                .AddScheme<AuthenticationSchemeOptions,
                              MockAuthenticatedUser>("BasicAuthentication", null);

            services.AddAuthorization(opt =>
            {
                // Add policies, and require the OPA policy, the policy is defined in policy.rego
                opt.AddPolicy("read", x => x.RequireOpaPolicy("mvcexample", "securedata", "GET"));
                opt.AddPolicy("create", x => x.RequireOpaPolicy("mvcexample", "securedata", "POST"));
                opt.AddPolicy("update", x => x.RequireOpaPolicy("mvcexample", "securedata", "PUT"));
                opt.AddPolicy("delete", x => x.RequireOpaPolicy("mvcexample", "securedata", "DELETE"));
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
                app.UseExceptionHandler("/Home/Error");
            }
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Data}/{action=Index}/{id?}");
            });
        }
    }
}
