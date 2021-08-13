/*
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */
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
using MvcExample.Models;
using AutoMapper;

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

            services.AddAutoMapper(opt =>
            {
                opt.CreateMap<DataModel, ViewModel>(MemberList.Destination)
                    .ForMember(x => x.Permissions, opt => opt.MapFromSourceObject());

                opt.CreateMap<DataModel, Permissions>()
                    .ForMember(x => x.CanEdit, opt => opt.MapFromPolicy("can_edit"))
                    .ForMember(x => x.CanDelete, opt => opt.MapFromPolicy("can_delete"));
            });

            //Add OPA
            services.AddOpa(opt =>
                // Add automapper support
                opt.AddAutomapperSupport()
                //Add policy and data sync
                .AddSync(s =>
                    // Add syncs here, such as syncing with an existing OPA server
                    //s.UseOpaServer("opaServerUrl")

                    //Add a local policy
                    s.UseLocal(local =>
                    {
                        var policyFiles = Directory.GetFiles("policies");
                        foreach(var file in policyFiles)
                        {
                            var policy = File.ReadAllText(file);
                            local.AddPolicy(policy);
                        }
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
                opt.AddPolicy("read", x => x.RequireOpaPolicy("mvcexample.read", "securedata"));
                opt.AddPolicy("create", x => x.RequireOpaPolicy("mvcexample.create", "securedata"));
                opt.AddPolicy("update", x => x.RequireOpaPolicy("mvcexample.update", "securedata"));
                opt.AddPolicy("can_edit", x => x.RequireOpaPolicy("mvcexample.can_edit", "securedata"));
                opt.AddPolicy("delete", x => x.RequireOpaPolicy("mvcexample.delete", "securedata"));
                opt.AddPolicy("can_delete", x => x.RequireOpaPolicy("mvcexample.can_delete", "securedata"));
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
