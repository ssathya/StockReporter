using DalImplementation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Models;
using Models.Model;
using System;
using System.IO;
using System.Threading.Tasks;

namespace HandleDailyJobs
{
    /// <summary>
    ///
    /// </summary>
    internal class Program
    {
        #region Private Fields

        /// <summary>
        /// The context
        /// </summary>
        private static StockReporterContext _context;

        private static ILogger logger;

        /// <summary>
        /// The repository
        /// </summary>
        private static IGenericRepository<CompanyDetail, string> _repository;

        #endregion Private Fields

        #region Private Methods

        /// <summary>
        /// Establishes the startup values.
        /// </summary>
        /// <param name="config">The configuration.</param>
        private static void EstablishStartupValues(IConfigurationRoot config)
        {
            DbContextOptions<StockReporterContext> options;
            var builder = new DbContextOptionsBuilder<StockReporterContext>();
            builder.UseSqlServer((config.GetConnectionString("StockReporterConnection")));
            options = builder.Options;
            _context = new StockReporterContext(options);
            _repository = new CompanyDetailsDAL(_context);
        }

        /// <summary>
        /// Gets the external data.
        /// </summary>
        /// <param name="temppath">The temppath.</param>
        private static void GetExternalData(string temppath)
        {
            logger.LogDebug("starting data download");
            var red = new ReadExternalData();
            var tasks = red.GetAllExternalData(temppath);
            Task.WaitAll(tasks);
            logger.LogDebug("Complete data download");
        }

        /// <summary>
        /// Loads the external files to database.
        /// </summary>
        private static void LoadExternalFilesToDb(string path)
        {
            logger.LogDebug("Starting to populate database");
            var loadExternalFilesToDb = new LoadExternalFilesToDb(_context, _repository);
            loadExternalFilesToDb.LodeData(path);
            logger.LogDebug("Completed populating database");
        }

        /// <summary>
        /// Mains the specified arguments.
        /// </summary>
        /// <param name="args">The arguments.</param>
        /// <exception cref="System.ArgumentException">
        /// Insufficient parameters to start the application.
        /// or
        /// Unknown command
        /// </exception>
        private static void Main(string[] args)
        {
            var serviceProvider = new ServiceCollection()
                .AddLogging()
                .BuildServiceProvider();

            serviceProvider
                .GetService<ILoggerFactory>()
                .AddConsole(LogLevel.Debug);

            logger = serviceProvider.GetService<ILoggerFactory>()
               .CreateLogger<Program>();
            if (!logger.IsEnabled(LogLevel.Debug))
                Console.WriteLine("Debug level logging not enabled");
            logger.LogDebug("Starting application");
            var config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();
            EstablishStartupValues(config);
            if (args.Length < 1)
            {
                throw new ArgumentException("Insufficient parameters to start the application.");
            }
            var temppath = Path.GetTempPath();
            var argument = args[0].ToLower().Replace(" ", "");
            switch (argument)
            {
                case "getexternaldata":
                    GetExternalData(temppath);
                    break;

                case "loadvaluestodb":
                    LoadExternalFilesToDb(temppath);
                    break;

                case "dogetandload":
                    GetExternalData(temppath);
                    LoadExternalFilesToDb(temppath);
                    break;

                default:
                    throw new ArgumentException("Unknown command");
            }
        }

        #endregion Private Methods
    }
}