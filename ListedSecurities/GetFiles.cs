using FluentFTP;
using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace ListedSecurities
{
    public class GetFiles
    {
        #region Public Constructors

        public GetFiles(string destination = null, string extUrl = null, string fileName = null)
        {
            if (string.IsNullOrEmpty(destination))
            {
                Destination = Path.GetTempPath();
                return;
            }
            else
            {
                Destination = destination;
            }
            ExternalUrl = extUrl;
            FileName = fileName;
        }

        #endregion Public Constructors

        #region Public Properties

        public string Destination { get; set; }

        public string ExternalUrl { get; set; }

        public string FileName { get; set; }

        #endregion Public Properties

        #region Public Methods

        public async Task<bool> ReadExternalFile(string extUrl = null, string fileName = null)
        {
            if (!string.IsNullOrWhiteSpace(extUrl))
            {
                ExternalUrl = extUrl;
            }
            if (!string.IsNullOrWhiteSpace(fileName))
            {
                FileName = fileName;
            }
            if (string.IsNullOrWhiteSpace(ExternalUrl) || string.IsNullOrWhiteSpace(FileName))
            {
                throw new InvalidOperationException("External URL or File Name not set");
            }
            if (ExternalUrl.StartsWith("ftp", StringComparison.CurrentCulture))
            {
                return await GetFtpData();
            }
            return await GetHttpData();
        }

        #endregion Public Methods

        #region Private Methods

        private async Task<bool> GetFtpData()
        {
            using (var client = new FtpClient(ExternalUrl))
            {
                client.Connect();
                var localPath = Path.Combine(Destination, FileName);
                if (File.Exists(localPath))
                {
                    File.Delete(localPath);
                }
                var downloadResult = await client.DownloadFileAsync(localPath, FileName, true, FtpVerify.None);
                return downloadResult;
            }
        }

        private async Task<bool> GetHttpData()
        {
            using (var client = new HttpClient())
            {
                var localPath = Path.Combine(Destination, FileName);
                using (var response = await client.GetAsync(ExternalUrl))
                {
                    using (var content = response.Content)
                    {
                        var result = await content.ReadAsStringAsync();
                        using (var writer = new StreamWriter(localPath))
                        {
                            var writeResult = writer.WriteAsync(result);
                            writeResult.Wait();
                            return writeResult.IsCompleted;
                        }
                    }
                }
            }
        }

        #endregion Private Methods
    }
}