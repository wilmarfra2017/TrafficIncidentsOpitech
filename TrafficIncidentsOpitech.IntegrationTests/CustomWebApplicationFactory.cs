using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using TrafficIncidentsOpitech.Infrastructure.Persistence;

namespace TrafficIncidentsOpitech.IntegrationTests;

public sealed class CustomWebApplicationFactory : WebApplicationFactory<TrafficIncidentsOpitech.Api.Program>
{
    private SqliteConnection? _connection;

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Development");

        builder.ConfigureServices(services =>
        {            
            services.RemoveAll(typeof(DbContextOptions<TrafficIncidentsDbContext>));
            services.RemoveAll(typeof(TrafficIncidentsDbContext));
            
            _connection = new SqliteConnection("DataSource=:memory:");
            _connection.Open();

            services.AddDbContext<TrafficIncidentsDbContext>(options =>
            {
                options.UseSqlite(_connection);
                options.EnableSensitiveDataLogging();
            });
            
            using var sp = services.BuildServiceProvider();
            using var scope = sp.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<TrafficIncidentsDbContext>();
            db.Database.EnsureCreated();
        });
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        if (disposing)
        {
            _connection?.Close();
            _connection?.Dispose();
        }
    }
}
