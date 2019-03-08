using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace StankinsDataWeb
{
    public class AngularMiddlerware
    {
        private readonly RequestDelegate next;
        private readonly IHostingEnvironment env;

        public AngularMiddlerware(RequestDelegate next, IHostingEnvironment env)
        {
            this.next = next;
            this.env = env;
        }

        public async Task Invoke(HttpContext context /* other dependencies */)
        {
            await next(context);
            int sc = context.Response.StatusCode;
            if (sc != 404)
            {
                return;
            }

            context.Response.StatusCode = 200;
            context.Response.ContentType = "text/html";
            byte[] fileBytes = await File.ReadAllBytesAsync(Path.Combine(env.WebRootPath, "index.html"));
            MemoryStream ms = new MemoryStream(fileBytes)
            {
                Position = 0
            };
            await ms.CopyToAsync(context.Response.Body);



        }

    }
    public class ErrorHandlingMiddleware
    {
        private readonly RequestDelegate next;

        public ErrorHandlingMiddleware(RequestDelegate next)
        {
            this.next = next;
        }

        public async Task Invoke(HttpContext context /* other dependencies */)
        {
            try
            {
                await next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            HttpStatusCode code = HttpStatusCode.InternalServerError; // 500 if unexpected
            string st = exception == null ? "<no exception defined>" : exception.StackTrace;
            string types = "";
            string message = "";
            Exception ex = exception;
            while (ex != null)
            {
                message += ex.Message + "=>";
                types += ex.GetType().FullName + "=>";
                ex = ex.InnerException;
            }
            string result = JsonConvert.SerializeObject(new { error = message, exType = types, st });
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)code;
            return context.Response.WriteAsync(result);
        }
    }
}
