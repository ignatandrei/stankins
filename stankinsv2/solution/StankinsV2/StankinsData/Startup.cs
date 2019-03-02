using System;
using System.IO;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Polly;

namespace StankinsDataWeb
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IHostingEnvironment hosting)
        {
            Configuration = configuration;
            Hosting = hosting;
        }

        public IConfiguration Configuration { get; }
        public IHostingEnvironment Hosting { get; }

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
            services.AddMemoryCache();
            services.AddApiVersioning(o=>{
                o.ReportApiVersions = true;
		        o.AssumeDefaultVersionWhenUnspecified = true;
                o.DefaultApiVersion = new ApiVersion(1, 0);
                });
            
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2)
                .ConfigureApplicationPartManager(apm => 
                    apm.FeatureProviders.Add(new GenericControllerFeatureProvider(Hosting)));;
            services.AddSwaggerDocument(c=>
            {
                c.Title = "Stankins Data Web";
                c.Description=" A generic ETL site; see https://github.com/ignatandrei/stankins";
                
            });
            services.AddScoped<IAsyncPolicy>((s)=> Define());
        }
        private IAsyncPolicy Define()
        {
            var timeout = Policy
                .TimeoutAsync(TimeSpan.FromMinutes(1));
                
                
            var retry = Policy.HandleInner<Exception>().WaitAndRetryAsync(
                3,
                retryAttempt=>TimeSpan.FromSeconds(2* retryAttempt)
                );

            return retry.WrapAsync(timeout);


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
              app.UseCors("CorsPolicy");
            //app.UseHttpsRedirection();

            app.UseDefaultFiles();
            app.UseStaticFiles();
            app.UseMiddleware<ErrorHandlingMiddleware>();
            app.UseSwagger(c=>{
            });
            app.UseSwaggerUi3(settings =>
            {
                
            });
            app.UseMvc();
            //redirect to angular page if do not use MVC or static files
            app.Run(async (context) =>
            {
                context.Response.ContentType = "text/html";
                var fileBytes = await File.ReadAllBytesAsync(Path.Combine(env.WebRootPath, "index.html"));
                var ms = new MemoryStream(fileBytes)
                {
                    Position = 0
                };
                await ms.CopyToAsync(context.Response.Body);
                context.Response.StatusCode = StatusCodes.Status200OK;
            });
        }
    }
}
