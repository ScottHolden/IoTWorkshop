using System;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.Devices.Client;
using Microsoft.Azure.Devices.Shared;
using Newtonsoft.Json;

namespace ConsoleTest
{
    public class WriteLineMessage
    {
        public string Text {get;set;}
    }
    class Program
    {
        private static Task<MethodResponse> WriteLine(MethodRequest methodRequest, object userContext)
        {
            var message = JsonConvert.DeserializeObject<WriteLineMessage>(methodRequest.DataAsJson);

            Console.WriteLine(message.Text);

            string result = "{\"result\":\"Ok\"}";
            return Task.FromResult(new MethodResponse(Encoding.UTF8.GetBytes(result), 200));
        }

        static async Task Main(string[] args)
        {
            string connectionString = @"";

            var client = DeviceClient.CreateFromConnectionString(connectionString);
            var random = new Random();

            await client.SetMethodHandlerAsync("WriteLine", WriteLine, null);
            
            var upperLimit = 100;

            var twin = await client.GetTwinAsync();
            var desiredProperties = twin.Properties.Desired;
            int desiredUpperLimit = upperLimit;
            if(desiredProperties != null && 
                desiredProperties.Contains("upperLimit") &&
                int.TryParse(desiredProperties["upperLimit"].ToString(), out desiredUpperLimit))
            {
                upperLimit = desiredUpperLimit;
            }

            TwinCollection reportedProperties= new TwinCollection();
            reportedProperties["upperLimit"] = upperLimit;
            await client.UpdateReportedPropertiesAsync(reportedProperties);

            Console.WriteLine("Upper limit is set to " + upperLimit);

            while(true)
            {
                var message = new {
                    Example = random.Next(upperLimit)
                };

                var messageJson = JsonConvert.SerializeObject(message);
                var messageBytes = Encoding.UTF8.GetBytes(messageJson);

                await client.SendEventAsync(new Message(messageBytes));
                Console.WriteLine("Message sent");
                await Task.Delay(2000);
            }
        }
    }
}
