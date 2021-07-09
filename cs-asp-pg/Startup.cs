using Dapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using System.Data;

namespace benchmark
{
    public class Startup
    {
        private readonly IConfiguration _configuration;

        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddScoped<IDbConnection>(_ => new NpgsqlConnection(_configuration.GetConnectionString("postgres")));
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGet("/", async context =>
                {
                    using var connection = context.RequestServices.GetRequiredService<IDbConnection>();
                    var results = await connection.QueryAsync<Fortune>("SELECT * FROM fortune");
                    await context.Response.WriteAsJsonAsync(results);
                });
            });
        }
    }
}
