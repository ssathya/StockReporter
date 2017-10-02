using DalImplementation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Models;
using Models.Model;
using System.Linq;

namespace DALTests
{
    [TestClass]
    public class CompanyDetailsDALTests
    {
        private StockReporterContext _context;
        private IGenericRepository<CompanyDetail, string> _repository;

        [TestInitialize]
        public void Initialize()
        {
            var config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")

                .Build();
            var connectionString = config["ConnectionStrings:StockReporterConnection"];
            var dbCO = new DbContextOptionsBuilder<StockReporterContext>()
                .UseSqlServer(connectionString)
                .Options;
            _context = new StockReporterContext(dbCO);
            _repository = new CompanyDetailsDAL(_context);
        }

        [TestMethod]
        public void AddRecordToDbTests()
        {
            _context.Database.ExecuteSqlCommand("Truncate table CompanyDetails");
            var cd = new CompanyDetail
            {
                IsExTrdFund = true,
                IsMutualFund = false,
                Symbol = "IJT",
                SecurityName = @"iShares S&P Small-Cap 600 Growth ETF"
            };
            _repository.Add(cd);

            var recordCount = _context.CompanyDetails.Count();

            Assert.AreEqual(1, recordCount);
        }
    }
}