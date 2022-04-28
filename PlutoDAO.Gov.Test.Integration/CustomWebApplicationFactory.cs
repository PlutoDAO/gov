using System;
using System.Linq;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using PlutoDAO.Gov.Application.Providers;

namespace PlutoDAO.Gov.Test.Integration
{
    public class CustomWebApplicationFactory<TStartup> : WebApplicationFactory<TStartup> where TStartup : class
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DateTimeProvider));

                services.Remove(descriptor);
                services.AddScoped(_ => new DateTimeProvider(DateTime.Today.AddDays(31)));
            });
        }
    }
}
