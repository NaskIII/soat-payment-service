using Application.Interfaces;
using Domain.BaseInterfaces;
using Domain.RepositoryInterfaces;
using Domain.Security;
using Infraestructure.DatabaseContext;
using Infraestructure.OpenApiConfiguration;
using Infraestructure.OrderGateway;
using Infraestructure.PaymentGAteway;
using Infraestructure.Repositories;
using Infraestructure.Security;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;
using System.Text;

namespace Infraestructure
{
    public static class DependencyInjection
    {

        public static IServiceCollection AddInfraestructure(this IServiceCollection services, IConfiguration configuration)
        {
            #region Authentication JWT
            var jwtSection = configuration.GetSection("Jwt");

            string? secretKey = jwtSection["SecretKey"];
            string? issuer = jwtSection["Issuer"];
            string? audience = jwtSection["Audience"];

            if (string.IsNullOrWhiteSpace(secretKey) || string.IsNullOrWhiteSpace(issuer) || string.IsNullOrWhiteSpace(audience))
                throw new InvalidOperationException("JWT settings are not properly configured in appsettings.");

            var key = Encoding.UTF8.GetBytes(secretKey);

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = true;
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),

                    ValidateIssuer = true,
                    ValidIssuer = issuer,

                    ValidateAudience = true,
                    ValidAudience = audience,

                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                };

                options.Events = new JwtBearerEvents
                {
                    OnTokenValidated = context =>
                    {
                        return Task.CompletedTask;
                    }
                };
            });

            #endregion

            #region Open API & Scalar
            services.AddOpenApi(options =>
            {
                options.AddDocumentTransformer((document, _, _) =>
                {
                    document.Info.Title = "Lanchonete API";
                    document.Info.Description = "API para lanchonete e pedidos.";
                    document.Info.Version = "v1";

                    return Task.CompletedTask;
                });

                options.AddDocumentTransformer<BearerSecuritySchemeTransformer>();
            });

            services.Configure<ScalarOptions>(options =>
            {
                options
                    .WithTitle("Lanchonete API")
                    .WithSidebar(true);

                options.Servers = [];

                options.AddServer(new ScalarServer("https://localhost:8081", "Endpoint com proteção SSL"));
                options.AddServer(new ScalarServer("http://localhost:8080", "Endpoint sem proteção SSL"));

                options.WithDefaultHttpClient(ScalarTarget.Shell, ScalarClient.Curl);
            });
            #endregion

            #region Database EFCore
            var connectionString = configuration.GetConnectionString("DBConnectionString");

            services.AddDbContext<ApplicationDatabaseContext>(options => options.UseNpgsql(connectionString));
            #endregion

            #region Repository
            services.AddScoped<IPaymentRepository, PaymentRepository>();
            services.AddScoped<IAccountServiceRepository, AccountServiceRepository>();
            #endregion

            #region Services
            services.AddHttpClient();
            services.AddSingleton<ITokenService, TokenService>();
            services.AddSingleton<IPasswordHasher, BCryptPasswordHasher>();
            services.AddSingleton<IPaymentGateway, MercadoPagoGateway>();

            services.AddHttpClient<IUpdateOrderStatusGateway, UpdateOrderStatusGateway>(client =>
            {
                client.BaseAddress = new Uri(configuration["OrderService:BaseUrl"] ?? throw new ArgumentNullException("OrderService BaseUrl is not configured."));
            });
            #endregion

            return services;
        }
    }
}