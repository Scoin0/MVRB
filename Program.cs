using MVRB.Services;
using Serilog;

public class Program
{
    static void Main(string[] args)
    {
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Verbose()
            .Enrich.FromLogContext()
            .WriteTo.Console()
            .CreateLogger();

        new DiscordService().InitAsync().GetAwaiter().GetResult();
    }
}