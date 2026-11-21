using System.Diagnostics;
using System.Diagnostics.Metrics;
using System.Reflection;

namespace MrJB.OTel.Api.Products;

internal static class OTel
{
    internal const string ActivitySourceName = "MrJB.OTel.Api.Products";

    internal static readonly string Version =
        typeof(OTel).Assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion.Split('+')[0] ?? "1.0.0";

    internal static readonly ActivitySource ActivitySource = new(ActivitySourceName, Version);

    public static class MetricNames
    {
        public const string BaseName = "mrjb.otel";

        public const string GetFeaturedProducts = $"{BaseName}.products.featured.get";
    }

    public static class Meters
    {
        public static readonly Meter Meter = new(ActivitySourceName, Version);

        private static readonly Counter<int> GetFeaturedProducts = Meter.CreateCounter<int>(MetricNames.GetFeaturedProducts, description: "Tracks when featured products are retrieved.");

        public static void AddGetFeaturedProducts(int count = 1) => GetFeaturedProducts.Add(count);
        public static void AddGetFeaturedProducts(int count, TagList tagList) => GetFeaturedProducts.Add(count, tagList);
    }
}
