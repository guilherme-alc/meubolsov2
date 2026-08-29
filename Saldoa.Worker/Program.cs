using Saldoa.Infrastructure;
using Saldoa.Application;

namespace Saldoa.Worker;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = Host.CreateApplicationBuilder(args);

        builder.Services.AddInfrastructure(builder.Configuration);
        builder.Services.AddApplication();

        var host = builder.Build();
        host.Run();
    }
}
