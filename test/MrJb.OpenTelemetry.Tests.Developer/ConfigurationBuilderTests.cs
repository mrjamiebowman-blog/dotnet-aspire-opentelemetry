using MrJB.OpenTelemetry.Domain.Configuration;
using Newtonsoft.Json.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using Xunit.Abstractions;

namespace MrJb.OpenTelemetry.Tests.Developer;

public class ConfigurationBuilderTests
{
    private readonly ITestOutputHelper _testOutputHelper;

    private string _loggingJson = @"{
        'Logging': {
            'LogLevel': {
                'Default': 'Information',
                'Microsoft': 'Warning',
                'Microsoft.AspNetCore': 'Warning',
                'Microsoft.AspNetCore.HttpLogging': 'Information',
                'Microsoft.Hosting.Lifetime': 'Information'
            }
        },
        'AllowedHosts': '*',
    }";

    public ConfigurationBuilderTests(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
    }

    [Fact]
    public Task Create_Configuration_Console_Test()
    {
        // configuration builder
        var config = new
        {
            HoneyComb = new HoneyCombConfiguration()
            {
                ServiceName = "service-name",
                ApiKey = "api-key"
            },
            ApplicationInsights = new ApplicationInsightsConfiguration()
            {
                ConnectionString = "connection-string",
            }
         };

        // json
        JsonSerializerOptions options = new JsonSerializerOptions {
            Converters = {
                new JsonStringEnumConverter()
            }
        };

        // serialize
        var json = JsonSerializer.Serialize(config, options);

        // merge json
        JObject loggingConfig = JObject.Parse(_loggingJson);
        JObject configJson = JObject.Parse(json);

        loggingConfig.Merge(configJson, new JsonMergeSettings {
            MergeArrayHandling = MergeArrayHandling.Union
        });

        // to json string
        json = loggingConfig.ToString();

        // output
        _testOutputHelper.WriteLine(json);

        // create file

        return Task.CompletedTask;
    }

    [Fact]
    public Task Create_Configuration_Api_Customer_Test()
    {
        return Task.CompletedTask;
    }

    [Fact]
    public Task Create_Configuration_Api_Orders_Test()
    {
        return Task.CompletedTask;
    }
}
