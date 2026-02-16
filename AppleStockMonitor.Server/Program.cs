using System.Reflection;
using Microsoft.OpenApi;

public partial class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.

        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(c =>
        {
            var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFilename);
            if (File.Exists(xmlPath))
            {
                c.IncludeXmlComments(xmlPath);
            }

            c.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "AppleStockMonitor API",
                Version = "v1",
                Description = "API for monitoring Apple stock prices and related data.",
                Contact = new OpenApiContact
                {
                    Name = "Mark Farrugia",
                    Email = "mark.e.farrugia@gmail.com",
                    Url = new Uri("https://github.com/MEK783/AppleStockMonitor")
                }
            });
        });

        var app = builder.Build();

        app.UseDefaultFiles();
        app.MapStaticAssets();

        app.MapOpenApi();
        app.UseSwagger();
        app.UseSwaggerUI();
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "AppleStockMonitor API V1");
            c.RoutePrefix = string.Empty; // Set Swagger UI at the app's root
        });

        app.UseHttpsRedirection();

        app.UseAuthorization();

        app.MapControllers();

        //app.MapFallbackToFile("/index.html");

        app.Run();
    }
}