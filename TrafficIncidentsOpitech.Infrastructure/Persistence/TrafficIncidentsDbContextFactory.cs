using System;
using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace TrafficIncidentsOpitech.Infrastructure.Persistence
{
    public class TrafficIncidentsDbContextFactory : IDesignTimeDbContextFactory<TrafficIncidentsDbContext>
    {
        public TrafficIncidentsDbContext CreateDbContext(string[] args)
        {
            var currentDir = Directory.GetCurrentDirectory();

            var candidate1 = Path.Combine(currentDir, "appsettings.json");
            var candidate2 = Path.GetFullPath(Path.Combine(currentDir, "..", "TrafficIncidentsOpitech.Api", "appsettings.json"));

            string basePath;

            if (File.Exists(candidate1))
            {
                basePath = currentDir;
            }
            else if (File.Exists(candidate2))
            {
                basePath = Path.GetDirectoryName(candidate2)!;
            }
            else
            {
                throw new InvalidOperationException(
                    $"appsettings.json no se encuentra. CurrentDir: {currentDir}. Tried: {candidate1} and {candidate2}"
                );
            }

            var appsettingsPath = Path.Combine(basePath, "appsettings.json");
            var appsettingsDevPath = Path.Combine(basePath, "appsettings.Development.json");

            var configuration = new ConfigurationBuilder()
                .AddJsonFile(appsettingsPath, optional: false)
                .AddJsonFile(appsettingsDevPath, optional: true)
                .AddEnvironmentVariables()
                .Build();

            var connectionString = configuration["ConnectionStrings:DefaultConnection"];

            if (string.IsNullOrWhiteSpace(connectionString))
                throw new InvalidOperationException($"No se encontró la cadena de conexión 'DefaultConnection'. Archivo {appsettingsPath}");

            var optionsBuilder = new DbContextOptionsBuilder<TrafficIncidentsDbContext>();
            optionsBuilder.UseSqlServer(connectionString);

            return new TrafficIncidentsDbContext(optionsBuilder.Options);
        }
    }
}
