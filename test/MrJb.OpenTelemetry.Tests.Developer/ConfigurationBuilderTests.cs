using Newtonsoft.Json.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace MrJb.OpenTelemetry.Tests.Developer;

public class ConfigurationBuilderTests
{
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

    [Fact]
    public Task Create_Configuration_Console_Test()
    {
        // configuration builder
        var appConfig = new
        {
            //IdentityServer = new IdentityServerConfiguration()
            //{
            //    ConnectionString = "connection-string",
            //    Google = new IdentityServerConfiguration.GoogleConfiguration()
            //    {
            //        Id = "google-id",
            //        Secret = "secret"
            //    }
            //},
            //Cache = new CacheConfiguration()
            //{
            //    ConnectionString = "connection-string",
            //    AbsoluteExpirationRelativeToNow = new TimeSpan(0, 01, 15, 00),
            //    InstanceName = "redis-master",
            //    SlidingExpiration = new TimeSpan(0, 01, 15, 00)
            //},
            //SigningKeys = new KeyVaultCertConfig()
            //{
            //    KeyVaultUri = "key-vault-uri"
            //}
        };

        // config
        var config = new
        {
            IdentityServer = appConfig
        };

        // json
        JsonSerializerOptions options = new JsonSerializerOptions
        {
            Converters = {
            new JsonStringEnumConverter()
        }
        };

        var json = JsonSerializer.Serialize(config, options);

        // merge json
        JObject loggingConfig = JObject.Parse(_loggingJson);
        JObject configJson = JObject.Parse(json);

        loggingConfig.Merge(configJson, new JsonMergeSettings
        {
            MergeArrayHandling = MergeArrayHandling.Union
        });
        json = loggingConfig.ToString();

        // output
        Console.WriteLine(json);

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
