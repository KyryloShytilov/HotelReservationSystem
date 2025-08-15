using HotelReservationSystem.Services.Contracts;
using SoapCore;

namespace HotelReservationSystem
{
    public static class SoapServices
    {
        public static void AddSoapServices(this IServiceCollection services)
        {
            // Register SOAP service interfaces and implementations
            services.AddScoped<ICustomerService, CustomerService>();
            services.AddScoped<IHotelService, HotelService>();
            services.AddScoped<IOrderService, OrderService>();
        }
        
        public static void UseSoapEndpoints(this IEndpointRouteBuilder endpoints)
        {
            // SOAP Service Endpoints
            endpoints.UseSoapEndpoint<ICustomerService>("/Services/CustomerService.svc", new SoapEncoderOptions(), SoapSerializer.DataContractSerializer);
            endpoints.UseSoapEndpoint<IHotelService>("/Services/HotelService.svc", new SoapEncoderOptions(), SoapSerializer.DataContractSerializer);
            endpoints.UseSoapEndpoint<IOrderService>("/Services/OrderService.svc", new SoapEncoderOptions(), SoapSerializer.DataContractSerializer);
        }
    }
}