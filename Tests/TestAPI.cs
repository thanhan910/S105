using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Text.Json;

namespace S105.Hubs
{
    [TestClass]
    public class PTVAPI3Test
    {
        [TestMethod]
        public void TestGetURL()
        {
            // Get API keys from json file local-env.json
            var json = File.ReadAllText("local-env.json");
            var values = JsonSerializer.Deserialize<JsonElement>(json);
            string apiKey = values.GetProperty("PTV_TIMETABLE_API_KEY").GetString() ?? "";
            string developerId = values.GetProperty("PTV_TIMETABLE_DEV_ID").GetString() ?? "";

            // Convert to int
            int devId = Int32.Parse(developerId);

            var client = new PTVAPI3(apiKey, devId);
            var result = client.GetURL("/v3/routes");
            Console.WriteLine(result);

            // Assert that result is not null
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void TestGetData()
        {
            // Get API keys from json file local-env.json
            var json = File.ReadAllText("local-env.json");
            var values = JsonSerializer.Deserialize<JsonElement>(json);
            string apiKey = values.GetProperty("PTV_TIMETABLE_API_KEY").GetString() ?? "";
            string developerId = values.GetProperty("PTV_TIMETABLE_DEV_ID").GetString() ?? "";

            // Convert to int
            int devId = Int32.Parse(developerId);

            var client = new PTVAPI3(apiKey, devId);
            var result = client.GetData("/v3/routes");
            Console.WriteLine(result);
            
            // Assert that result is not null
            Assert.IsNotNull(result);
        }
    }
}
