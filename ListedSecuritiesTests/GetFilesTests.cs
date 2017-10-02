using ListedSecurities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;

namespace ListedSecuritiesTests
{
    [TestClass]
    public class GetFilesTests
    {
        #region Public Methods

        [TestMethod]
        public void ReadNasdaqFromExternalSourceTest()
        {
            var temppath = Path.GetTempPath();
            var fileName = @"symboldirectory/nasdaqlisted.txt";
            var url = @"ftp://ftp.nasdaqtrader.com";
            var gf = new GetFiles(temppath, url, fileName);
            var result = gf.ReadExternalFile(url, fileName).Result;
            Assert.AreEqual(true, result);
        }

        [TestMethod]
        public void ReadMutualFundFileFromExternalSourceTest()
        {
            var temppath = Path.GetTempPath();
            var fileName = @"symboldirectory/mfundslist.txt";
            var url = @"ftp://ftp.nasdaqtrader.com";
            var gf = new GetFiles(temppath, url, fileName);
            var result = gf.ReadExternalFile(url, fileName).Result;
            Assert.AreEqual(true, result);
        }

        [TestMethod]
        public void ReadOtherListedFromExternalSourceTest()
        {
            var temppath = Path.GetTempPath();
            var fileName = @"symboldirectory/otherlisted.txt";
            var url = @"ftp://ftp.nasdaqtrader.com";
            var gf = new GetFiles(temppath, url, fileName);
            var result = gf.ReadExternalFile(url, fileName).Result;
            Assert.AreEqual(true, result);
        }

        [TestMethod]
        public void ReadETFListFormExternalSourceTest()
        {
            var temppath = Path.GetTempPath();
            var fileName = @"symboldirectory/ETFList.csv";
            var url = @"http://www.nasdaq.com/investing/etfs/etf-finder-results.aspx?download=Yes";
            var gf = new GetFiles(temppath, url, fileName);
            var result = gf.ReadExternalFile(url, fileName).Result;
            Assert.AreEqual(true, result);
        }

        #endregion Public Methods
    }
}