using DalImplementation;
using ListedSecurities;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Models;
using Models.Model;
using System;
using System.IO;
using System.Linq;

namespace ListedSecuritiesTests
{
    [TestClass]
    public class LoadFileToDbTests
    {
        #region Private Fields

        private StockReporterContext _context;
        private IGenericRepository<CompanyDetail, string> _repository;
        private bool isETF, isMutalFund;

        #endregion Private Fields

        #region Public Methods

        [TestInitialize]
        public void Initialize()
        {
            DbContextOptions<StockReporterContext> options;
            var builder = new DbContextOptionsBuilder<StockReporterContext>();
            builder.UseInMemoryDatabase("StockReporter");
            options = builder.Options;
            _context = new StockReporterContext(options);
            _repository = new CompanyDetailsDAL(_context);
            isETF = false;
            isMutalFund = false;
        }

        [TestMethod]
        public void ParseFileTest()
        {
            var fileName = @"symboldirectory/nasdaqlisted.txt";
            var temppath = Path.GetTempPath();
            fileName = temppath + fileName;
            string[] columnNames = { "Symbol", "Security Name" };
            var lftdb = new LoadFileToDb(fileName, columnNames);
            var loadResult = lftdb.ParseSecurityListingFile();
            Assert.IsTrue(loadResult);
        }

        [TestMethod]
        public void SaveMutualFundsToDbEmptyTableTest()
        {
            ClearTable();
            isMutalFund = true;
            string[] columnNames = { "Fund Symbol", "Fund Name" };
            var fileName = @"symboldirectory/mfundslist.txt";
            LoadFileToDb lftdb = PrepareCompanyDetailsList(columnNames, ref fileName);
            lftdb.IsMutaulFund = true;

            var resultCount = lftdb.SaveRecordsToDb(_repository);
            Assert.AreEqual(resultCount, lftdb.CompanyDetails.Count);
            var fairxRecord = _repository.Get("FAIRX");
            Assert.AreEqual("Fairholme Fd", fairxRecord.SecurityName);
        }

        [TestMethod]
        public void SaveOtherListedToDbEmptyTableTest()
        {
            ClearTable();
            string[] columnNames = { "NASDAQ Symbol", "Security Name" };
            var fileName = @"symboldirectory/otherlisted.txt";
            LoadFileToDb lftdb = PrepareCompanyDetailsList(columnNames, ref fileName);

            var resultCount = lftdb.SaveRecordsToDb(_repository);
            Assert.AreEqual(resultCount, lftdb.CompanyDetails.Count);
            var ibmRecord = _repository.Get("IBM");
            Assert.AreEqual("International Business Machines Corporation Common Stock", ibmRecord.SecurityName);
        }

        [TestMethod]
        public void SaveRecordsToDbEmptyTableTest()
        {
            ClearTable();
            string[] columnNames = { "Symbol", "Security Name" };
            var fileName = @"symboldirectory/nasdaqlisted.txt";
            LoadFileToDb lftdb = PrepareCompanyDetailsList(columnNames, ref fileName);

            var resultCount = lftdb.SaveRecordsToDb(_repository);
            Assert.AreEqual(resultCount, lftdb.CompanyDetails.Count);
            Console.WriteLine(_context.CompanyDetails.Count(r => r.IsExTrdFund == true));
        }

        [TestMethod]
        public void SaveETFRecordsToDbEmptyTableTest()
        {
            isETF = true;
            ClearTable();
            string[] columnNames = { "Symbol", "Name" };
            var fileName = @"symboldirectory/ETFList.csv";
            LoadFileToDb lftdb = PrepareCompanyDetailsList(columnNames, ref fileName, ",");

            var resultCount = lftdb.SaveRecordsToDb(_repository);
            Assert.AreEqual(resultCount, lftdb.CompanyDetails.Count);
            var iwoRecord = _context.CompanyDetails.FirstOrDefault(r => r.Symbol.Equals("IWO"));
            Assert.IsTrue(iwoRecord.IsExTrdFund);
        }

        [TestMethod]
        public void SaveRecordsToDbNonEmptyTableTest()
        {
            ClearTable();
            InsertTestRecordToTable(symbol: "F", securityName: "FORD MOTOR CO", isExTrdFund: false, isMutualFund: false);
            InsertTestRecordToTable(symbol: "FB", securityName: "FACEBOOK INC", isExTrdFund: false, isMutualFund: false);
            InsertTestRecordToTable(symbol: "ROKU", securityName: "ROKU INC", isExTrdFund: true, isMutualFund: true);
            _context.SaveChanges();

            string[] columnNames = { "Symbol", "Security Name" };
            var fileName = @"symboldirectory/nasdaqlisted.txt";
            LoadFileToDb lftdb = PrepareCompanyDetailsList(columnNames, ref fileName);

            var resultCount = lftdb.SaveRecordsToDb(_repository);
            var rokuCount = _context.CompanyDetails.Count(r => r.Symbol.Equals("ROKU"));
            var rokuRecord = _context.CompanyDetails.FirstOrDefault(r => r.Symbol.Equals("ROKU"));
            Assert.AreEqual(resultCount, lftdb.CompanyDetails.Count);
            Assert.IsFalse(rokuRecord.IsExTrdFund);
            Assert.IsFalse(rokuRecord.IsMutualFund);
            Assert.AreEqual(1, rokuCount);
        }

        [TestMethod]
        public void UpdateDatabaseWithCompanyDetailsTest()
        {
            ClearTable();
            string[] columnNames = { "NASDAQ Symbol", "Security Name" };
            var fileName = @"symboldirectory/otherlisted.txt";
            LoadFileToDb lftdb = PrepareCompanyDetailsList(columnNames, ref fileName);

            columnNames = new string[] { "Symbol", "Sector", "industry", "IPOyear" };
            var temppath = Path.GetTempPath();
            fileName = @"symboldirectory/NYSECompanyList.csv";
            var delimiter = ",";
            fileName = temppath + fileName;
            lftdb.FileName = fileName;
            lftdb.Columns = columnNames;
            lftdb.Delimiter = delimiter;
            var loadResult = lftdb.ParseCompanyDetailsFile();
            var resultCount = lftdb.SaveRecordsToDb(_repository);
            Assert.IsTrue(loadResult);
            var testRcd = _context.CompanyDetails.FirstOrDefault(s => s.Symbol.Equals("ICE"));
            Assert.AreEqual(2005, testRcd.IPOyear);
            Assert.AreEqual("Finance", testRcd.Sector);
        }

        #endregion Public Methods

        #region Private Methods

        private LoadFileToDb PrepareCompanyDetailsList(string[] columnNames, ref string fileName, string delimiter = "|")
        {
            var temppath = Path.GetTempPath();
            fileName = temppath + fileName;

            var lftdb = new LoadFileToDb(fileName, columnNames)
            {
                Delimiter = delimiter,
                IsETF = isETF,
                IsMutaulFund = isMutalFund
            };
            var loadResult = lftdb.ParseSecurityListingFile();
            return lftdb;
        }

        private void ClearTable()
        {
            var itemsToDelete = _context.Set<CompanyDetail>();
            _context.CompanyDetails.RemoveRange(itemsToDelete);
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

        #endregion Private Methods
    }
}