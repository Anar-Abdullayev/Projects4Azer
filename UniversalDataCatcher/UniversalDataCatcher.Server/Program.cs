
using System.Text;
using UniversalDataCatcher.Server.Services;

namespace UniversalDataCatcher.Server
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;
            var builder = WebApplication.CreateBuilder(args);


            // MSSql Server-də database və table-ləri yaradır.
            MSSqlDatabaseService.Initialize(builder.Configuration);

            builder.Services.AddAuthorization();
            builder.Services.AddControllers();

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            app.UseDefaultFiles();
            app.UseStaticFiles();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            app.UseHttpsRedirection();
            app.UseAuthorization();
            app.MapFallbackToFile("/index.html");
            app.MapControllers();
            app.Run();
        }
    }
}
