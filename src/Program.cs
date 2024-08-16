using Almond;
using Microsoft.Extensions.DependencyInjection;

var services = new ServiceCollection()
        .ConfigureServices()
        .AddDefaultServices()
        .BuildServiceProvider();

var application = services.GetRequiredService<AlmondAlerts>();

await application.ExecuteAsync();