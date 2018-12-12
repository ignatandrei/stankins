using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using StankinsAliveMonitor.SignalRHubs;
using Microsoft.AspNetCore.Http;

namespace StankinsStatusWeb
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
            services.AddCors(o => o.AddPolicy("CorsPolicy", builder =>
            {
                builder
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowAnyOrigin();
            }));

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
            services.AddSignalR();
            services.Configure<MonitorOptions>(Configuration.GetSection("MonitorData"));
            services.PostConfigure<MonitorOptions>((x) =>
            {
               
            });

            services.AddSingleton<ReplaySubject<ResultWithData>>(new ReplaySubject<ResultWithData>(new TimeSpan(0,10,0)));
            var m = new MonitorOptions();
            Configuration.Bind("MonitorData", m);
            services.AddSingleton(m);
            services.AddHostedService<RunTasks>();
            services.AddMediatR();
            

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
                app.UseHsts();
            }
            app.UseCors("CorsPolicy");
            //app.UseHttpsRedirection();
            app.UseDefaultFiles();
            app.UseStaticFiles();
            app.UseSignalR(routes =>
            {
                routes.MapHub<DataHub>("/DataHub");
            });
            app.UseMvc();
            //redirect to angular page if do not use MVC or static files
            app.Run(async (context) =>
            {
                context.Response.ContentType = "text/html";
                var fileBytes = await File.ReadAllBytesAsync(Path.Combine(env.WebRootPath, "index.html"));
                var ms = new MemoryStream(fileBytes);
                ms.Position = 0;
                await ms.CopyToAsync(context.Response.Body);
                context.Response.StatusCode = StatusCodes.Status200OK;
            });
            
        }
    }
}
