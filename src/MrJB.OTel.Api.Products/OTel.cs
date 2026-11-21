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

    internal static readonly Meter Meter = new(ActivitySourceName, Version);
}
