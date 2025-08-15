using System.ServiceModel;
using new_app.DTOs;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace new_app.Services.Interfaces;

[ServiceContract]
public interface IHotelService
{
    [OperationContract]
    Task<List<HotelDto>> GetHotelsAsync();
    
    [OperationContract]
    Task<HotelDto> GetHotelAsync(int id);
    
    [OperationContract]
    Task<HotelDto> CreateHotelAsync(HotelDto hotelDto);
    
    [OperationContract]
    Task UpdateHotelAsync(int id, HotelDto hotelDto);
    
    [OperationContract]
    Task DeleteHotelAsync(int id);
}