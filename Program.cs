using MVRB.Services;

public class Program
{
    static void Main(string[] args) => new DiscordService().InitAsync().GetAwaiter().GetResult();
}