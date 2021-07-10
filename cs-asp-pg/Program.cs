using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System.Data;
using System.IO;
using Npgsql;
using Dapper;

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
            endpoints.MapGet("/", async context =>
            {
                var connection = context.RequestServices.GetRequiredService<IDbConnection>();
                var results = await connection.QueryAsync<Fortune>("SELECT * FROM fortune");
                await context.Response.WriteAsJsonAsync(results);
            });
        });
    }
}