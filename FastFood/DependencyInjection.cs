using Infraestructure.Serialization;

namespace FastFood
{
    public static class DependencyInjection
    {

        public static IServiceCollection AddFastFood(this IServiceCollection services, IConfiguration configuration)
        {
            #region Controllers
            services
                .AddControllers()
                .AddJsonOptions(options => options.JsonSerializerOptions.Converters.Add(new DateOnlyJsonConverter()));
            services.AddEndpointsApiExplorer();
            #endregion

            #region Cors
            services.AddCors(
                options =>
                {
                    string frontEndURl = configuration.GetValue<string>("System:BaseURL") ?? throw new ArgumentNullException("URL base do front end não foi encontradad.");

                    options.AddPolicy(
                        "AllowCors",
                        builder =>
                        {
                            builder.WithOrigins(frontEndURl).
                            AllowAnyMethod()
                            .AllowAnyHeader()
                            .AllowCredentials()
                            .SetIsOriginAllowed(origin => true);
                        });
                });
            #endregion

            return services;
        }
    }
}
