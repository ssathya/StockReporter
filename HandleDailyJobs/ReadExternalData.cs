using ListedSecurities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HandleDailyJobs
{
    internal class ReadExternalData
    {
        private readonly Dictionary<string, string> _fileList;

        public ReadExternalData()
        {
            _fileList = MaintainFileList.GetExternalFileList();
        }

        internal Task<bool>[] GetAllExternalData(string destinationFolder)
        {
            var gf = new GetFiles(destinationFolder);
            var retValue = new List<Task<bool>>();
            foreach (var file in _fileList)
            {
                var fileName = file.Key;
                var rv = gf.ReadExternalFile(_fileList[fileName], fileName);
                retValue.Add(rv);
            }
            return retValue.ToArray();
        }
    }
}