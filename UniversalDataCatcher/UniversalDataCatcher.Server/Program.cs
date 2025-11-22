
using Serilog;
using System.Text;
using UniversalDataCatcher.Server.Bots.Bina.Services;
using UniversalDataCatcher.Server.Bots.EvTen.Services;
using UniversalDataCatcher.Server.Bots.Lalafo.Services;
using UniversalDataCatcher.Server.Bots.Tap.Services;
using UniversalDataCatcher.Server.Bots.YeniEmlak.Services;
using UniversalDataCatcher.Server.Services;
using UniversalDataCatcher.Server.Services.Arenda.Services;

namespace UniversalDataCatcher.Server
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;
            var builder = WebApplication.CreateBuilder(args);

            MSSqlDatabaseService.Initialize(builder.Configuration);

            //Log.Logger = new LoggerConfiguration()
            //    .Enrich.FromLogContext()
            //    .WriteTo.File("logs/info/info-.log",
            //    rollingInterval: RollingInterval.Day,
            //    restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Information,
            //    outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss} {ServiceName} - {Message:lj}{NewLine}{Exception}")
            //    .WriteTo.File("logs/error/error-.log",
            //    rollingInterval: RollingInterval.Day,
            //    restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Error,
            //    outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss} {ServiceName} - {Message:lj}{NewLine}{Exception}")
            //    .WriteTo.Console()
            //    .CreateLogger();
            builder.Services.AddSingleton<EvTenService>();
            builder.Services.AddSingleton<EvTenMSSqlDatabaseService>();
            builder.Services.AddSingleton<ArendaService>();
            builder.Services.AddSingleton<ArendaMSSqlDatabaseService>();
            builder.Services.AddSingleton<BinaService>();
            builder.Services.AddSingleton<BinaMSSqlDatabaseService>();
            builder.Services.AddSingleton<LalafoService>();
            builder.Services.AddSingleton<LalafoMSSqlDatabaseService>();
            builder.Services.AddSingleton<TapAzService>();
            builder.Services.AddSingleton<TapazMSSqlDatabaseService>();
            builder.Services.AddSingleton<YeniEmlakService>();
            builder.Services.AddSingleton<YeniemlakMSSqlDatabaseService>();


            builder.Services.AddAuthorization();
            builder.Services.AddControllers();

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Host.UseSerilog((ctx, lc) =>
            {
                lc.Enrich.FromLogContext()
                .WriteTo.File("logs/info/info-.log",
                rollingInterval: RollingInterval.Day,
                restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Information,
                outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss} {ServiceName} - {Message:lj}{NewLine}{Exception}")
                .WriteTo.File("logs/error/error-.log",
                rollingInterval: RollingInterval.Day,
                restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Error,
                outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss} {ServiceName} - {Message:lj}{NewLine}{Exception}")
                .WriteTo.Console(
                    outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss} {ServiceName} - {Message:lj}{NewLine}{Exception}");
            });
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
