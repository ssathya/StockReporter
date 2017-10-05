using System;
using System.Collections.Generic;
using System.Text;

namespace HandleDailyJobs
{
    internal class MaintainFileList
    {
        internal static Dictionary<string, string> GetExternalFileList()
        {
            var fileList = new Dictionary<string, string>();
            fileList.Add(@"symboldirectory/nasdaqlisted.txt", @"ftp://ftp.nasdaqtrader.com");
            fileList.Add(@"symboldirectory/mfundslist.txt", @"ftp://ftp.nasdaqtrader.com");
            fileList.Add(@"symboldirectory/otherlisted.txt", @"ftp://ftp.nasdaqtrader.com");
            fileList.Add(@"symboldirectory/ETFList.txt", @"http://www.nasdaq.com/investing/etfs/etf-finder-results.aspx?download=Yes");
            fileList.Add(@"symboldirectory/NASDAQCompanyList.csv", @"http://www.nasdaq.com/screening/companies-by-name.aspx?letter=0&exchange=nasdaq&render=download");
            fileList.Add(@"symboldirectory/NYSECompanyList.csv", @"http://www.nasdaq.com/screening/companies-by-name.aspx?letter=0&exchange=nyse&render=download");
            fileList.Add(@"symboldirectory/AMEXCompanyList.csv", @"http://www.nasdaq.com/screening/companies-by-name.aspx?letter=0&exchange=amex&render=download");
            return fileList;
        }
    }
}