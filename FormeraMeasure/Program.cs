using Microsoft.AspNetCore.Hosting;
using System.IO;
using Microsoft.Extensions.Configuration;

namespace FormeraMeasure
{
  public class Program
  {
    public static void Main(string[] args)
    {
        var configuration = new ConfigurationBuilder()
        //.AddCommandLine(args)
        .Build();
        var hostUrl = configuration["hosturl"];

        if (string.IsNullOrEmpty(hostUrl))
            hostUrl = "http://0.0.0.0:5000";

        var host = new WebHostBuilder()
          .UseKestrel()
          .UseUrls(hostUrl)
          .UseContentRoot(Directory.GetCurrentDirectory())
          .UseIISIntegration()
          .UseStartup<Startup>()
          .Build();

      host.Run();
    }
  }
}