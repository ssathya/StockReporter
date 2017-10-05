using System;
using Models;
using Models.Model;
using System.Collections.Generic;
using ListedSecurities;
using System.Linq;

namespace HandleDailyJobs
{
    internal class LoadExternalFilesToDb
    {
        private readonly StockReporterContext _context;
        private IGenericRepository<CompanyDetail, string> _repository;
        private string delimiter;
        private bool isETF, isMutualFund;

        private LoadFileToDb loadFileToDb;

        public LoadExternalFilesToDb(StockReporterContext context, IGenericRepository<CompanyDetail, string> repository)
        {
            _context = context;
            _repository = repository;
            delimiter = "|";
            isETF = false;
            isMutualFund = false;
            string[] columnNames = { };
            var fileName = "";
            loadFileToDb = new LoadFileToDb(fileName, columnNames);
        }

        internal void LodeData(string path)
        {
            Console.WriteLine("Dropping old records");
            ClearTable();
            delimiter = "|";
            isETF = false;
            isMutualFund = false;

            string[] columnNames = { "Symbol", "Security Name" };
            var fileName = @"symboldirectory/nasdaqlisted.txt";
            Console.WriteLine($"Processing {fileName}");
            LoadCompanyList(path, fileName, columnNames);

            isMutualFund = true;
            columnNames = new string[] { "Fund Symbol", "Fund Name" };
            fileName = @"symboldirectory/mfundslist.txt";
            Console.WriteLine($"Processing {fileName}");
            LoadCompanyList(path, fileName, columnNames);
            isMutualFund = false;

            columnNames = new string[] { "NASDAQ Symbol", "Security Name" };
            fileName = @"symboldirectory/otherlisted.txt";
            Console.WriteLine($"Processing {fileName}");
            LoadCompanyList(path, fileName, columnNames);

            isETF = true;
            delimiter = ",";
            columnNames = new string[] { "Symbol", "Name" };
            fileName = @"symboldirectory/ETFList.txt";
            Console.WriteLine($"Processing {fileName}");
            ProcessETFList(path, fileName, columnNames);
            //LoadCompanyList(path, fileName, columnNames);
            isETF = false;

            delimiter = ",";
            columnNames = new string[] { "Symbol", "Sector", "industry", "IPOyear", "Name" };
            fileName = @"symboldirectory/AMEXCompanyList.csv";
            Console.WriteLine($"Processing {fileName}");
            LoadCompanyDetails(path, fileName, columnNames);

            delimiter = ",";
            columnNames = new string[] { "Symbol", "Sector", "industry", "IPOyear", "Name" };
            fileName = @"symboldirectory/NYSECompanyList.csv";
            Console.WriteLine($"Processing {fileName}");
            LoadCompanyDetails(path, fileName, columnNames);

            delimiter = ",";
            columnNames = new string[] { "Symbol", "Sector", "industry", "IPOyear", "Name" };
            fileName = @"symboldirectory/NASDAQCompanyList.csv";
            Console.WriteLine($"Processing {fileName}");
            LoadCompanyDetails(path, fileName, columnNames);

            loadFileToDb.SaveRecordsToDb(_repository);
        }

        private void ProcessETFList(string path, string fileName, string[] columnNames)
        {
            var fileToUse = path + fileName;
            loadFileToDb.FileName = fileToUse;
            loadFileToDb.Columns = columnNames;
            loadFileToDb.Delimiter = delimiter;
            loadFileToDb.IsETF = isETF;
            loadFileToDb.IsMutaulFund = isMutualFund;
            //loadFileToDb.ParseCompanyDetailsFile();
            //loadFileToDb.SaveRecordsToDb(_repository);

            loadFileToDb.ParseSecurityListingFile();
        }

        private void LoadCompanyDetails(string path, string fileName, string[] columnNames)
        {
            var fileToUse = path + fileName;
            loadFileToDb.FileName = fileToUse;
            loadFileToDb.Columns = columnNames;
            loadFileToDb.Delimiter = delimiter;
            loadFileToDb.IsETF = isETF;
            loadFileToDb.IsMutaulFund = isMutualFund;
            loadFileToDb.ParseCompanyDetailsFile();
            //loadFileToDb.SaveRecordsToDb(_repository);
        }

        private void LoadCompanyList(string path, string fileName, string[] columnNames)
        {
            var fileToUse = path + fileName;
            loadFileToDb.FileName = fileToUse;
            loadFileToDb.Columns = columnNames;
            loadFileToDb.Delimiter = delimiter;
            loadFileToDb.IsETF = isETF;
            loadFileToDb.IsMutaulFund = isMutualFund;

            loadFileToDb.ParseSecurityListingFile();
            //loadFileToDb.SaveRecordsToDb(_repository);
        }

        private void ClearTable()
        {
            var itemsToDelete = _context.Set<CompanyDetail>();
            _context.CompanyDetails.RemoveRange(itemsToDelete);
            _context.SaveChanges();
        }
    }
}