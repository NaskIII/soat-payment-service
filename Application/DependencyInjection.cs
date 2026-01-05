using Application.Interfaces;
using Application.UseCases.CheckoutUseCases;
using Application.UseCases.PaymentUseCase;
using Application.UseCases.ServiceAccountsUseCases;
using AutoMapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration configuration)
        {
            #region AutoMapper
            MapperConfiguration mapperConfig = new(cfg =>
            {
            });

            IMapper mapper = mapperConfig.CreateMapper();

            services.AddSingleton(mapper);
            #endregion

            #region Use Cases
            
            services.AddScoped<ICheckoutUseCase, CheckoutUseCase>();
            services.AddScoped<IProcessPaymentUseCase, ProcessPaymentUseCase>();
            services.AddScoped<IPaymentStatusUseCase, PaymentStatusUseCase>();
            services.AddScoped<IAuthenticateServiceAccount, AuthenticateUseCase>();

            #endregion

            return services;
        }
    }
}
