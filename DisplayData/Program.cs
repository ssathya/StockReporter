using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

namespace DisplayData
{
    public class Program
    {
        #region Public Methods

        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .Build();

        public static void Main(string[] args)
        {
            BuildWebHost(args).Run();
        }

        #endregion Public Methods
    }
}