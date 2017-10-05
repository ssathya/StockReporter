using CsvHelper;
using Models;
using Models.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ListedSecurities
{
    /// <summary>
    ///
    /// </summary>
    public class LoadFileToDb
    {
        #region Public Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="LoadFileToDb"/> class.
        /// </summary>
        /// <param name="filePath">The file path.</param>
        /// <param name="columns">The columns.</param>
        public LoadFileToDb(string filePath, string[] columns)
        {
            FileName = filePath;
            Columns = columns;
            CompanyDetails = new List<CompanyDetail>();
            Delimiter = "|";
        }

        #endregion Public Constructors

        #region Public Properties

        /// <summary>
        /// Gets or sets the columns.
        /// </summary>
        /// <value>
        /// The columns.
        /// </value>
        public string[] Columns { get; set; }

        /// <summary>
        /// Gets or sets the company details.
        /// </summary>
        /// <value>
        /// The company details.
        /// </value>
        public List<CompanyDetail> CompanyDetails { get; set; }

        /// <summary>
        /// Gets or sets the delimiter.
        /// </summary>
        /// <value>
        /// The delimiter.
        /// </value>
        public string Delimiter { get; set; }

        /// <summary>
        /// Gets or sets the file path.
        /// </summary>
        /// <value>
        /// The file path.
        /// </value>
        public string FileName { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is etf.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is etf; otherwise, <c>false</c>.
        /// </value>
        public bool IsETF { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is mutaul fund.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is mutaul fund; otherwise, <c>false</c>.
        /// </value>
        public bool IsMutaulFund { get; set; }

        #endregion Public Properties

        #region Public Methods

        /// <summary>
        /// Parses the company details file.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception">
        /// File path is not set
        /// or
        /// Columns to read not set
        /// </exception>
        public bool ParseCompanyDetailsFile()
        {
            var counter = 0;
            if (FileName == null)
            {
                throw new Exception("File path is not set");
            }
            if (Columns.Length == 0)
            {
                throw new Exception("Columns to read not set");
            }
            SetIdentifierFields(out string symbol, out string ipoYear,
                out string sector, out string industry, out string name);
            using (var reader = File.OpenText(FileName))
            {
                var csv = new CsvReader(reader);
                ConfigureReader(csv);
                while (csv.Read())
                {
                    ExtractDataFromRecord(symbol, ipoYear, sector, industry, name, csv,
                        out string securitySymbol, out string securitySector,
                        out string securityIndustry,
                        out int securityIPOYear,
                        out string securityName);
                    if (string.IsNullOrWhiteSpace(securitySector) ||
                        string.IsNullOrWhiteSpace(securityIndustry) ||
                        securitySector.Equals("n/a") ||
                        securityIndustry.Equals("n/a"))
                        continue;
                    var cd = CompanyDetails.FirstOrDefault(r => r.Symbol.Equals(securitySymbol));
                    if (cd == null)
                    {
                        cd = new CompanyDetail
                        {
                            Symbol = securitySymbol,
                            IPOyear = securityIPOYear,
                            Sector = securitySector,
                            Industry = securityIndustry,
                            SecurityName = securityName
                        };
                        CompanyDetails.Add(cd);
                        counter++;
                    }
                    else
                    {
                        cd.IPOyear = securityIPOYear;
                        cd.Sector = securitySector;
                        cd.Industry = securityIndustry;
                    }
                }
            }
            Console.WriteLine($"New records count {counter}");
            return true;
        }

        /// <summary>
        /// Parses the security listing file.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception">
        /// File path is not set
        /// or
        /// Columns to read not set
        /// </exception>
        public bool ParseSecurityListingFile()
        {
            if (FileName == null)
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
            using (var reader = File.OpenText(FileName))
            {
                var csv = new CsvReader(reader);
                ConfigureReader(csv);
                while (csv.Read())
                {
                    CompanyDetail newRecord = BuildCompanyDetails(csv, symbol, securityName);
                    newRecord.IsExTrdFund = IsETF == true ? IsETF : newRecord.IsExTrdFund;
                    var oldRecord = CompanyDetails.Find(cd => cd.Symbol.Equals(newRecord));
                    if (oldRecord != null)
                    {
                        oldRecord.Industry = string.IsNullOrWhiteSpace(newRecord.Industry) ? oldRecord.Industry : newRecord.Industry;
                        oldRecord.IPOyear = newRecord.IPOyear == 0 ? oldRecord.IPOyear : newRecord.IPOyear;
                        oldRecord.IsExTrdFund = newRecord.IsExTrdFund == false ? oldRecord.IsExTrdFund : newRecord.IsExTrdFund;
                        oldRecord.IsMutualFund = newRecord.IsMutualFund == false ? oldRecord.IsMutualFund : newRecord.IsMutualFund;
                        oldRecord.Sector = string.IsNullOrWhiteSpace(newRecord.Sector) ? oldRecord.Sector : newRecord.Sector;
                    }
                    else
                    {
                        CompanyDetails.Add(newRecord);
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// Saves the records to database.
        /// </summary>
        /// <param name="repository">The repository.</param>
        /// <returns></returns>
        public int SaveRecordsToDb(IGenericRepository<CompanyDetail, string> repository)
        {
            List<Task<bool>> dbUpdateResults = new List<Task<bool>>();
            var counter = 0;
            bool result;
            foreach (var companyDetail in CompanyDetails)
            {
                if (string.IsNullOrWhiteSpace(companyDetail.Symbol) ||
                string.IsNullOrWhiteSpace(companyDetail.SecurityName))
                {
                    Console.WriteLine("Skipping record");
                }
                else
                {
                    repository.Add(companyDetail, false);
                }
                if (++counter % 512 == 0)
                {
                    while (dbUpdateResults.Count >= 1)
                    {
                        RemoveCompletedTasks(dbUpdateResults);
                    }
                    dbUpdateResults.Add(repository.SaveAsync());
                    Console.WriteLine($"Uploaded {counter} rows");
                }
            }
            Console.WriteLine("Waiting for save to complete");
            while (dbUpdateResults.Count != 0)
            {
                RemoveCompletedTasks(dbUpdateResults);
            }
            result = repository.SaveAsync().Result;
            Console.WriteLine("Database update completed");
            return counter;
        }

        private static void RemoveCompletedTasks(List<Task<bool>> dbUpdateResults)
        {
            var completedTasks = dbUpdateResults.
                                    FirstOrDefault(t => t.IsCompleted == true);
            while (completedTasks != null)
            {
                dbUpdateResults.Remove(completedTasks);
                completedTasks = dbUpdateResults.
                FirstOrDefault(t => t.IsCompleted == true);
            }
        }

        #endregion Public Methods

        #region Private Methods

        /// <summary>
        /// Builds the company details.
        /// </summary>
        /// <param name="csv">The CSV.</param>
        /// <param name="symbol">The symbol.</param>
        /// <param name="securityName">Name of the security.</param>
        /// <returns></returns>
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

        /// <summary>
        /// Configures the reader.
        /// </summary>
        /// <param name="csv">The CSV.</param>
        private void ConfigureReader(CsvReader csv)
        {
            csv.Configuration.HasHeaderRecord = true;
            csv.Configuration.Delimiter = Delimiter;
            csv.Configuration.SkipEmptyRecords = true;
            csv.Configuration.TrimFields = true;
            csv.Configuration.TrimHeaders = true;
        }

        /// <summary>
        /// Extracts the data from record.
        /// </summary>
        /// <param name="symbol">The symbol.</param>
        /// <param name="ipoYear">The ipo year.</param>
        /// <param name="sector">The sector.</param>
        /// <param name="industry">The industry.</param>
        /// <param name="csv">The CSV.</param>
        /// <param name="securitySymbol">The security symbol.</param>
        /// <param name="securitySector">The security sector.</param>
        /// <param name="securityIndustry">The security industry.</param>
        /// <param name="securityIPOYear">The security ipo year.</param>
        private void ExtractDataFromRecord(string symbol, string ipoYear, string sector,
                                            string industry, string name, CsvReader csv, out string securitySymbol,
                                            out string securitySector, out string securityIndustry,
                                            out int securityIPOYear, out string securityName)
        {
            securitySymbol = csv.GetField<string>(symbol);
            var securityIPO = csv.GetField<string>(ipoYear);
            securitySector = csv.GetField<string>(sector);
            securityIndustry = csv.GetField<string>(industry);
            securitySymbol = ReplaceCart(securitySymbol);
            securityName = csv.GetField<string>(name);
            securityIPOYear = 0;
            int.TryParse(securityIPO, out securityIPOYear);
            securityIPOYear = securityIPOYear == 0 ? 1950 : securityIPOYear;
        }

        private string ReplaceCart(string input)
        {
            string pattern = @"\^";
            string substitution = @"-";
            var regex = new Regex(pattern);
            return regex.Replace(input, substitution);
        }

        /// <summary>
        /// Sets the identifier fields.
        /// </summary>
        /// <param name="symbol">The symbol.</param>
        /// <param name="ipoYear">The ipo year.</param>
        /// <param name="sector">The sector.</param>
        /// <param name="industry">The industry.</param>
        private void SetIdentifierFields(out string symbol, out string ipoYear, out string sector,
            out string industry, out string name)
        {
            symbol = Columns.FirstOrDefault(c => c.Contains("Symbol"));
            ipoYear = Columns.FirstOrDefault(c => c.Contains("IPOyear"));
            sector = Columns.FirstOrDefault(c => c.Contains("Sector"));
            industry = Columns.FirstOrDefault(c => c.Contains("industry"));
            name = Columns.FirstOrDefault(c => c.Contains("Name"));
            symbol = string.IsNullOrWhiteSpace(symbol) ? "Symbol" : symbol;
            ipoYear = string.IsNullOrWhiteSpace(ipoYear) ? "IPOyear" : ipoYear;
            sector = string.IsNullOrWhiteSpace(sector) ? "Sector" : sector;
            industry = string.IsNullOrWhiteSpace(industry) ? "industry" : industry;
            name = string.IsNullOrWhiteSpace(name) ? "Name" : name;
        }

        /// <summary>
        /// Sets the identifier fields.
        /// </summary>
        /// <param name="symbol">The symbol.</param>
        /// <param name="securityName">Name of the security.</param>
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