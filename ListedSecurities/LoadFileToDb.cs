using CsvHelper;
using Models;
using Models.Model;
using System;
using System.Collections.Generic;
using System.IO;

namespace ListedSecurities
{
    public class LoadFileToDb
    {
        #region Public Constructors

        public LoadFileToDb(string filePath, string[] columns)
        {
            _filePath = filePath;
            Columns = columns;
            CompanyDetails = new List<CompanyDetail>();
            Delimiter = "|";
        }

        #endregion Public Constructors

        #region Public Properties

        public string[] Columns { get; set; }
        public List<CompanyDetail> CompanyDetails { get; set; }
        public bool IsETF { get; set; }
        public bool IsMutaulFund { get; set; }
        public string Delimiter { get; set; }

        #endregion Public Properties

        #region Private Properties

        private string _filePath { get; set; }

        #endregion Private Properties

        #region Public Methods

        public bool ParseFile()
        {
            if (_filePath == null)
            {
                throw new Exception("File path is not set");
            }
            if (Columns.Length == 0)
            {
                throw new Exception("Columns to read not set");
            }
            var symbol = "";
            var securityName = "";
            SetIdentifierFields(ref symbol, ref securityName);
            using (var reader = File.OpenText(_filePath))
            {
                var csv = new CsvReader(reader);
                ConfigureReader(csv);
                while (csv.Read())
                {
                    CompanyDetail newRecord = BuildCompanyDetails(csv, symbol, securityName);
                    newRecord.IsExTrdFund = IsETF == true ? IsETF : newRecord.IsExTrdFund;
                    CompanyDetails.Add(newRecord);
                }
            }
            return true;
        }

        public int SaveRecordsToDb(IGenericRepository<CompanyDetail, string> repository)
        {
            var counter = 0;
            bool result;
            foreach (var companyDetail in CompanyDetails)
            {
                var oldRecord = repository.Get(companyDetail.Symbol);
                if (oldRecord != null)
                {
                    companyDetail.Id = oldRecord.Id;
                    repository.Update(companyDetail, false);
                }
                else
                {
                    repository.Add(companyDetail, false);
                }
                counter++;
            }
            result = repository.SaveAsync().Result;
            return counter;
        }

        #endregion Public Methods

        #region Private Methods

        private void ConfigureReader(CsvReader csv)
        {
            csv.Configuration.HasHeaderRecord = true;
            csv.Configuration.Delimiter = Delimiter;
            csv.Configuration.SkipEmptyRecords = true;
            csv.Configuration.TrimFields = true;
            csv.Configuration.TrimHeaders = true;
        }

        private CompanyDetail BuildCompanyDetails(CsvReader csv, string symbol, string securityName)
        {
            if (!csv.TryGetField<string>("ETF", out string etfValue))
            {
                etfValue = "N";
            }
            return new CompanyDetail
            {
                Symbol = csv.GetField<string>(symbol),
                SecurityName = csv.GetField<string>(securityName),
                IsExTrdFund = etfValue.Equals("Y") ? true : false,
                IsMutualFund = IsMutaulFund
            };
        }

        private void SetIdentifierFields(ref string symbol, ref string securityName)
        {
            foreach (var column in Columns)
            {
                if (column.Contains("Symbol"))
                {
                    symbol = column;
                }
                else if (column.Contains("Name"))
                {
                    securityName = column;
                }
            }
        }

        #endregion Private Methods
    }
}