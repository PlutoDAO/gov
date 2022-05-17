using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using PlutoDAO.Gov.Application.Proposals;
using PlutoDAO.Gov.Application.Providers;
using PlutoDAO.Gov.Application.Votes;
using PlutoDAO.Gov.Infrastructure.Stellar;
using PlutoDAO.Gov.Infrastructure.Stellar.Proposals;
using PlutoDAO.Gov.Infrastructure.Stellar.Votes;
using stellar_dotnet_sdk;

namespace PlutoDAO.Gov.WebApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddDefaultPolicy(
                    builder =>
                    {
                        builder
                            .SetIsOriginAllowedToAllowWildcardSubdomains()
                            .AllowAnyHeader()
                            .AllowAnyOrigin()
                            .AllowAnyMethod();
                    });
            });

            services.AddScoped(_ => new SystemAccountConfiguration(
                Environment.GetEnvironmentVariable("PLUTODAO_PROPOSAL_MICROPAYMENT_SENDER_ACCOUNT_PRIVATE_KEY") ??
                throw new ApplicationException("PLUTODAO_PROPOSAL_MICROPAYMENT_SENDER_ACCOUNT_PRIVATE_KEY not set"),
                Environment.GetEnvironmentVariable("PLUTODAO_PROPOSAL_MICROPAYMENT_RECEIVER_ACCOUNT_PRIVATE_KEY") ??
                throw new ApplicationException("PLUTODAO_PROPOSAL_MICROPAYMENT_RECEIVER_ACCOUNT_PRIVATE_KEY not set"),
                Environment.GetEnvironmentVariable("PLUTODAO_ESCROW_ACCOUNT_PRIVATE_KEY") ??
                throw new ApplicationException("PLUTODAO_ESCROW_ACCOUNT_PRIVATE_KEY not set"),
                Environment.GetEnvironmentVariable("PLUTODAO_RESULTS_ACCOUNT_PRIVATE_KEY") ??
                throw new ApplicationException("PLUTODAO_RESULTS_ACCOUNT_PRIVATE_KEY not set")));

            services.AddScoped(_ => new Server(Environment.GetEnvironmentVariable("HORIZON_URL")));

            services.AddScoped<ProposalService>();
            services.AddScoped<VoteService>();
            services.AddScoped<IProposalRepository, ProposalRepository>();
            services.AddScoped<IVoteRepository, VoteRepository>();
            services.AddScoped(_ => new DateTimeProvider(DateTime.UtcNow));

            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo {Title = "PlutoDAO.Gov.WebApi", Version = "v1"});
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                Network.Use(new Network(Environment.GetEnvironmentVariable("HORIZON_NETWORK_PASSPHRASE")));
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "PlutoDAO.Gov.WebApi v1"));
            }
            else if (env.IsStaging())
            {
                Network.UseTestNetwork();
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "PlutoDAO.Gov.WebApi v1"));
            }
            else
            {
                Network.UsePublicNetwork();
            }

            app.UseHttpLogging();

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseCors();

            app.UseAuthorization();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}
