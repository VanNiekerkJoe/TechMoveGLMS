using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TechMoveGLMS.API;
using TechMoveGLMS.API.Data;

namespace TechMoveGLMS.Tests.IntegrationTests
{
    public class TestFixture : IDisposable
    {
        public HttpClient Client { get; private set; }
        private readonly WebApplicationFactory<Program> _factory;

        public TestFixture()
        {
            _factory = new WebApplicationFactory<Program>()
                .WithWebHostBuilder(builder =>
                {
                    builder.ConfigureServices(services =>
                    {
                        // Remove the real DbContext registration
                        var descriptor = services.SingleOrDefault(
                            d => d.ServiceType == typeof(DbContextOptions<AppDbContext>));

                        if (descriptor != null)
                            services.Remove(descriptor);

                        // Add In-Memory database for testing
                        services.AddDbContext<AppDbContext>(options =>
                            options.UseInMemoryDatabase("TestDatabase"));
                    });
                });

            Client = _factory.CreateClient();
        }

        public void Dispose()
        {
            Client?.Dispose();
            _factory?.Dispose();
        }
    }
}