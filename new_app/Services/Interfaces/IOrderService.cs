using System.ServiceModel;
using new_app.DTOs;
using new_app.Models;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace new_app.Services.Interfaces;

[ServiceContract]
public interface IOrderService
{
    [OperationContract]
    Task<List<Order>> GetOrdersAsync();
    
    [OperationContract]
    Task<Order> GetOrderAsync(int id);
    
    [OperationContract]
    Task<Order> CreateOrderAsync(NewOrderDto orderDto);
    
    [OperationContract]
    Task DeleteOrderAsync(int id);
}