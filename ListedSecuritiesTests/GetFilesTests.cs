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
        public void ReadFileFromExternalSourceTest()
        {
            var temppath = Path.GetTempPath();
            var fileName = @"symboldirectory/nasdaqlisted.txt";
            var url = @"ftp://ftp.nasdaqtrader.com";
            var gf = new GetFiles(temppath, url, fileName);
            var result = gf.ReadExternalFile(url, fileName).Result;
            Assert.AreEqual(true, result);
        }

        #endregion Public Methods
    }
}