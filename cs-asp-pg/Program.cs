using Dapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Npgsql;
using System.Data;
using System.IO;

var config = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
    .Build();

var builder = new WebHostBuilder()
    .UseContentRoot(Directory.GetCurrentDirectory())
    .UseConfiguration(config)
    .UseDefaultServiceProvider(configure =>
    {
        configure.ValidateOnBuild = false;
        configure.ValidateScopes = false;
    })
    .UseKestrel(options =>
    {
        options.ListenAnyIP(5000, c => c.Protocols = HttpProtocols.Http1);
    })
    .UseStartup<Startup>();

builder.Build().Run();

record Fortune(int Id, string Message);

record World(int Id, int RandomNumber);

class Startup
{
    private readonly IConfiguration _configuration;

    public Startup(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddRouting();
        services.AddScoped<IDbConnection>(_ => new NpgsqlConnection(_configuration.GetConnectionString("postgres")));
    }

    public void Configure(IApplicationBuilder app)
    {
        app.UseRouting();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapGet("/text", context => context.Response.WriteAsync("Hello World"));

            endpoints.MapGet("/json", context => context.Response.WriteAsJsonAsync(new Fortune(1, "Hello World")));

            endpoints.MapGet("/single", async context =>
            {
                var connection = context.RequestServices.GetRequiredService<IDbConnection>();
                var results = await connection.QueryAsync<Fortune>("SELECT id, message FROM fortune ORDER BY id");
                await context.Response.WriteAsJsonAsync(results);
            });

            endpoints.MapGet("/multiple", async context =>
            {
                var connection = context.RequestServices.GetRequiredService<IDbConnection>();
                var item = await connection.QuerySingleAsync<World>("SELECT id, randomnumber FROM world WHERE id = 260");
                var results = await connection.QueryAsync<World>("SELECT id, randomnumber FROM world WHERE randomnumber > @RandomNumber ORDER BY id", item);
                await context.Response.WriteAsJsonAsync(results);
            });
        });
    }
}