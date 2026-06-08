using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using System;
using System.Linq;
using TechMoveGLMS.API;
using TechMoveGLMS.API.Data;

namespace TechMoveGLMS.Tests.IntegrationTests
{
    public class TestWebApplicationFactory : WebApplicationFactory<Program>, IDisposable
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                // Remove the existing DbContext registration
                var descriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(DbContextOptions<AppDbContext>));

                if (descriptor != null)
                    services.Remove(descriptor);

                // Add In-Memory database for testing
                services.AddDbContext<AppDbContext>(options =>
                    options.UseInMemoryDatabase("TestDatabase"));
            });
        }
    }
}