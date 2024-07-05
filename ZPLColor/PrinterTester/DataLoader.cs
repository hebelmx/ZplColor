using Newtonsoft.Json;

namespace PrinterTester;

public class DataLoader
{
    public static List<TestData> LoadTestData(string filePath)
    {
        var jsonData = File.ReadAllText(filePath);
        var testDatas = JsonConvert.DeserializeObject<List<TestData>>(jsonData,
            new JsonSerializerSettings
            {
                Converters = new List<JsonConverter> { new ColorConverter() }
            });

        return testDatas;
    }
}