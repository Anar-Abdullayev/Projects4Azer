
using Microsoft.Data.SqlClient;
using Serilog;
using System.Data;
using System.Text;
using UniversalDataCatcher.Server.Bots.Bina.Services;
using UniversalDataCatcher.Server.Bots.Emlak.Services;
using UniversalDataCatcher.Server.Bots.EvTen.Services;
using UniversalDataCatcher.Server.Bots.Lalafo.Services;
using UniversalDataCatcher.Server.Bots.Tap.Services;
using UniversalDataCatcher.Server.Bots.YeniEmlak.Services;
using UniversalDataCatcher.Server.Interfaces;
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
            var connectionString = MSSqlDatabaseService.GetConnectionString();
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
            builder.Services.AddSingleton<EmlakService>();
            builder.Services.AddSingleton<EmlakMSSqlDatabaseService>();
            builder.Services.AddScoped<IDbConnection>(sp => new SqlConnection(connectionString));
            builder.Services.AddScoped<IAdvertisementService, AdvertisementService>();

            builder.Services.AddAuthorization();
            builder.Services.AddControllers();

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Host.UseSerilog();
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
