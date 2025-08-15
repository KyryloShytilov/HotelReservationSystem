/*
Example SOAP client code that can be used to test the service:

using System;
using System.ServiceModel;
using System.Threading.Tasks;
using new_app.Services.Interfaces;

public class SoapClientExample
{
    public static async Task Main()
    {
        // Create client with endpoint
        var binding = new BasicHttpBinding(BasicHttpSecurityMode.Transport);
        binding.MaxReceivedMessageSize = 2147483647;
        
        var endpointAddress = new EndpointAddress("https://localhost:7000/soap/hotels");
        
        // Creating client factory
        var channelFactory = new ChannelFactory<IHotelService>(binding, endpointAddress);
        
        // Add authentication
        channelFactory.Credentials.UserName.UserName = "username";
        channelFactory.Credentials.UserName.Password = "apikey";
        
        try
        {
            // Create client and call operations
            var client = channelFactory.CreateChannel();
            
            // Call SOAP operations
            var hotels = await client.GetHotelsAsync();
            foreach (var hotel in hotels)
            {
                Console.WriteLine($"Hotel: {hotel.Name}, Stars: {hotel.Stars}");
            }
            
            // Close the client
            ((IClientChannel)client).Close();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }
}

// Alternative using WCF Client Generator
// You can also use the WCF-connected service feature in Visual Studio to generate a client from the WSDL:
// 1. Right-click on the project and select "Add > Connected Service"
// 2. Choose "WCF Web Service Reference Provider"
// 3. Enter the WSDL URL (e.g., https://localhost:7000/soap/hotels?wsdl)
// 4. Configure the namespace and other options
// 5. Click "Finish" to generate the client

// Then you can use it like:
// var client = new HotelServiceClient(binding, endpointAddress);
// var hotels = await client.GetHotelsAsync();
*/