/*
Example SOAP client code that can be used to test the service:

using System;
using System.ServiceModel;
using System.Threading.Tasks;

// Create a client reference by adding a Connected Service to the WSDL URL
// For example: https://localhost:7000/soap/hotels?wsdl

public class SoapClientExample
{
    public static async Task Main()
    {
        // Create client with endpoint
        var binding = new BasicHttpBinding(BasicHttpSecurityMode.Transport);
        binding.MaxReceivedMessageSize = 2147483647;
        
        var endpointAddress = new EndpointAddress("https://localhost:7000/soap/hotels");
        var client = new HotelServiceClient(binding, endpointAddress);
        
        // Add authentication
        client.ClientCredentials.UserName.UserName = "username";
        client.ClientCredentials.UserName.Password = "password";
        
        try
        {
            // Call SOAP operations
            var hotels = await client.GetHotelsAsync();
            foreach (var hotel in hotels)
            {
                Console.WriteLine($"Hotel: {hotel.Name}, Stars: {hotel.Stars}");
            }
            
            // Close the client
            await client.CloseAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
            client.Abort();
        }
    }
}
*/