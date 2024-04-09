clear;

# vars
$VERSION = 'dev'

# build .net
docker build --no-cache -f "src\MrJb.OpenTelemetry.Api.Customers\Dockerfile" -t mrjamiebowman/mrjb-otel-api-customers:$VERSION .