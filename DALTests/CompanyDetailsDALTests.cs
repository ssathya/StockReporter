using DalImplementation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Models;
using Models.Model;
using System;
using System.Linq;

namespace DALTests
{
    [TestClass]
    public class CompanyDetailsDALTests
    {
        #region Private Fields

        private StockReporterContext _context;
        private IGenericRepository<CompanyDetail, string> _repository;

        #endregion Private Fields

        #region Public Methods

        [TestMethod]
        public void AddRecordToDbTests()
        {
            InsertTestRecordToTable();

            var recordCount = _context.CompanyDetails.Count();

            Assert.AreEqual(1, recordCount);
        }

        [TestMethod]
        public void DeleteRecordByIdTest()
        {
            InsertTestRecordToTable();
            var records = _context.CompanyDetails.ToList();
            Console.WriteLine(records.Count);
            foreach (var record in records)
            {
                _repository.Delete(record.Id);
            }
            var recordCount = _context.CompanyDetails.Count();
            Assert.AreEqual(0, recordCount);
        }

        [TestMethod]
        public void DeleteRecordByRecordTest()
        {
            InsertTestRecordToTable();
            var records = _context.CompanyDetails.ToList();
            Console.WriteLine(records.Count);
            foreach (var record in records)
            {
                _repository.Delete(record);
            }
            var recordCount = _context.CompanyDetails.Count();
            Assert.AreEqual(0, recordCount);
        }

        //insert multiple records and check if record count is equal.
        [TestMethod]
        public void GetAllTest()
        {
            DeleteAllRecords();
            InsertTestRecordToTable(symbol: "MSFT", securityName: "Microsoft Corp", isExTrdFund: false, isMutualFund: false);
            InsertTestRecordToTable("IJT", @"iShares S&P Small-Cap 600 Growth ETF", true, false);
            InsertTestRecordToTable(symbol: "F", securityName: "FORD MOTOR CO", isExTrdFund: false, isMutualFund: false);
            InsertTestRecordToTable(symbol: "FB", securityName: "FACEBOOK INC", isExTrdFund: false, isMutualFund: false);
            InsertTestRecordToTable(symbol: "ROKU", securityName: "ROKU INC", isExTrdFund: false, isMutualFund: false);
            _context.SaveChanges();

            var records = _repository.GetAll().ToList();

            Assert.AreEqual(5, records.Count);
        }

        [TestMethod]
        public void GetAsyncTest()
        {
            InsertTestRecordToTable(symbol: "MSFT", securityName: "Microsoft Corp", isExTrdFund: false, isMutualFund: false);
            InsertTestRecordToTable("IJT", @"iShares S&P Small-Cap 600 Growth ETF", true, false);
            InsertTestRecordToTable(symbol: "F", securityName: "FORD MOTOR CO", isExTrdFund: false, isMutualFund: false);
            InsertTestRecordToTable(symbol: "FB", securityName: "FACEBOOK INC", isExTrdFund: false, isMutualFund: false);
            InsertTestRecordToTable(symbol: "ROKU", securityName: "ROKU INC", isExTrdFund: false, isMutualFund: false);
            _context.SaveChanges();

            var speciman = _context.CompanyDetails.FirstOrDefault(r => r.Symbol.Equals("ROKU"));

            var testRecord = _repository.GetAsync(speciman.Id).Result;

            Assert.AreEqual(speciman, testRecord);
        }

        [TestMethod]
        public void GetTest()
        {
            InsertTestRecordToTable(symbol: "MSFT", securityName: "Microsoft Corp", isExTrdFund: false, isMutualFund: false);
            InsertTestRecordToTable("IJT", @"iShares S&P Small-Cap 600 Growth ETF", true, false);
            InsertTestRecordToTable(symbol: "F", securityName: "FORD MOTOR CO", isExTrdFund: false, isMutualFund: false);
            InsertTestRecordToTable(symbol: "FB", securityName: "FACEBOOK INC", isExTrdFund: false, isMutualFund: false);
            InsertTestRecordToTable(symbol: "ROKU", securityName: "ROKU INC", isExTrdFund: false, isMutualFund: false);
            InsertTestRecordToTable(symbol: "VWUSX", securityName: "VANGUARD US GROWTH INV", isExTrdFund: false, isMutualFund: true);
            _context.SaveChanges();

            var record = _repository.Get("FB");
            Assert.AreEqual(@"FACEBOOK INC", record.SecurityName);
        }

        [TestInitialize]
        public void Initialize()
        {
            var config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();

            DbContextOptions<StockReporterContext> options;
            var builder = new DbContextOptionsBuilder<StockReporterContext>();
            builder.UseInMemoryDatabase("StockReporter");
            options = builder.Options;
            _context = new StockReporterContext(options);
            _repository = new CompanyDetailsDAL(_context);
        }

        [TestMethod]
        public void UpdateTest()
        {
            InsertTestRecordToTable(symbol: "FAIRX", securityName: "FAIRHOLME", isExTrdFund: false, isMutualFund: false);
            _context.SaveChanges();

            var record = _repository.Get("FAIRX");
            if (record != null)
            {
                record.IsMutualFund = true;
                _repository.Update(record);
            }
            var resultRecord = _repository.Get("FAIRX");
            Assert.IsTrue(resultRecord.IsMutualFund);
        }

        #endregion Public Methods

        #region Private Methods

        private void DeleteAllRecords()
        {
            var records = _context.CompanyDetails.ToList();
            foreach (var record in records)
            {
                _context.Entry(record).State = EntityState.Deleted;
            }
            _context.SaveChanges();
        }

        private void InsertTestRecordToTable(string symbol, string securityName, bool isExTrdFund, bool isMutualFund)
        {
            var cd = new CompanyDetail
            {
                Symbol = symbol,
                SecurityName = securityName,
                IsExTrdFund = isExTrdFund,
                IsMutualFund = isMutualFund
            };
            _context.CompanyDetails.Add(cd);
        }

        private void InsertTestRecordToTable()
        {
            var cd = new CompanyDetail
            {
                IsExTrdFund = true,
                IsMutualFund = false,
                Symbol = "IJT",
                SecurityName = @"iShares S&P Small-Cap 600 Growth ETF"
            };
            _repository.Add(cd);
        }

        #endregion Private Methods
    }
}