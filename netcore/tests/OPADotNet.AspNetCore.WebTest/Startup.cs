using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OPADotNet.AspNetCore.WebTest
{
    public class Startup
    {
        private static string moduleData = @"
package localpolicy

default allow = false

allow {
  input.identity = data.admin.name
}
";

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            //Add OPA and connects to an OPA server to get the policies and data required.
            services.AddOpa(x => x
                    .OpaServerSync("http://127.0.0.1:8181")
                    .Local(opt =>
                    {
                        opt.AddPolicy(moduleData);
                    })
                );

            services.AddAuthorization(opt =>
            {
                opt.AddPolicy("read", x => x.RequireOpaPolicy("example", "reports"));
                opt.AddPolicy("test", x => x.RequireOpaPolicy("localpolicy", "testdata"));
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
