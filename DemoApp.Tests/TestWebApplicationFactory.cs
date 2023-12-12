using System.Data.Common;
using System.Net.Http.Headers;
using DemoApp.Lib;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace DemoApp.Tests;

public class TestWebApplicationFactory<TStartup> : WebApplicationFactory<TStartup> where TStartup : class
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("TESTING");

        builder.ConfigureTestServices(services =>
        {
            //services.AddAuthentication(defaultScheme: TestAuthHandler.SchemeName)
            //    .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>(TestAuthHandler.SchemeName, options => { });
            //services.AddAuthorization(options =>
            //{
            //    options.AddPolicy(CustomAuthPolicies.ServiceOnly, builder =>
            //    {
            //        builder.RequireClaim(CustomClaims.Scope, CustomScopes.ApiAccess);
            //        builder.AddAuthenticationSchemes(TestAuthHandler.SchemeName);
            //        builder.RequireAuthenticatedUser();
            //    });
            //});

            services.AddSingleton<IHttpClientFactory>(_ => new TestHttpClientFactory<TStartup>(this));

            var dbContextDescriptor = services.SingleOrDefault(
                d => d.ServiceType ==
                     typeof(DbContextOptions<DemoDbContext>));

            services.Remove(dbContextDescriptor);

            var dbConnectionDescriptor = services.SingleOrDefault(
                d => d.ServiceType ==
                     typeof(DbConnection));

            services.Remove(dbConnectionDescriptor);

            // Create open SqliteConnection so EF won't automatically close it.
            services.AddSingleton<DbConnection>(container =>
            {
                var connection = new SqliteConnection("DataSource=:memory:");
                //connection.Open();

                return connection;
            });

            services.AddDbContext<DemoDbContext>((container, options) =>
            {
                var connection = container.GetRequiredService<DbConnection>();
                options.UseSqlite(connection);
            });
        });
    }
}

public class TestHttpClientFactory<TStartup> : IHttpClientFactory where TStartup : class
{
    private readonly WebApplicationFactory<TStartup> _factory;

    public TestHttpClientFactory(WebApplicationFactory<TStartup> factory)
    {
        _factory = factory;
    }

    public HttpClient CreateClient(string name)
    {
        var httpClient = _factory.CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = false });
        //httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(scheme: TestAuthHandler.SchemeName);
        return httpClient;
    }
}