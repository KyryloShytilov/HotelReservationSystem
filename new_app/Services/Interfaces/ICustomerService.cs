using System.ServiceModel;
using new_app.Models;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace new_app.Services.Interfaces;

[ServiceContract]
public interface ICustomerService
{
    [OperationContract]
    Task<List<Customer>> GetCustomersAsync();
    
    [OperationContract]
    Task<Customer> GetCustomerAsync(int id);
    
    [OperationContract]
    Task DeleteCustomerAsync(int id);
}