using Microsoft.EntityFrameworkCore;
using Models;
using Models.Model;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace DalImplementation
{
    public class CompanyDetailsDAL : IGenericRepository<CompanyDetail, string>
    {
        private readonly StockReporterContext _context;

        public CompanyDetailsDAL(StockReporterContext context)
        {
            _context = context;
        }

        public void Add(CompanyDetail entity, bool commit = true)
        {
            _context.Add(entity);
            if (commit)
            {
                var result = SaveAsync().Result;
            }
        }

        public void Delete(CompanyDetail entity, bool commit = true)
        {
            _context.Entry(entity).State = EntityState.Deleted;
            //_context.CompanyDetails.Remove(entity);
            if (commit)
            {
                var result = SaveAsync().Result;
            }
        }

        public void Delete(int id, bool commit = true)
        {
            var cd = _context.CompanyDetails.FirstOrDefault(r => r.Id == id);
            if (cd != null)
            {
                Delete(cd, commit);
            }
        }

        public IQueryable<CompanyDetail> GetAll()
        {
            return _context.CompanyDetails;
        }

        public async Task<CompanyDetail> GetAsync(int id)
        {
            var retvalue = await _context.FindAsync<CompanyDetail>(id);
            return retvalue;
        }

        public CompanyDetail Get(string symbol)
        {
            var retvalue = _context.CompanyDetails.FirstOrDefault(r => r.Symbol.Equals(symbol));
            return retvalue;
        }

        public CompanyDetail GetWithoutTracking(string symbol)
        {
            var retvalue = _context.CompanyDetails.AsNoTracking().FirstOrDefault(r => r.Symbol.Equals(symbol));
            return retvalue;
        }

        public async Task<bool> SaveAsync()
        {
            return await _context.SaveChangesAsync() >= 1 ? true : false;
        }

        public void Update(CompanyDetail entity, bool commit = true)
        {
            var record = GetAsync(entity.Id).Result;
            if (record == null)
            {
                return;
            }
            _context.Entry(record).CurrentValues.SetValues(entity);
            //record.SecurityName = entity.SecurityName;
            //record.Symbol = entity.Symbol;
            //record.IsMutualFund = entity.IsMutualFund;
            //record.IsExTrdFund = entity.IsExTrdFund;
            _context.Update(record);
            if (commit)
            {
                var x = SaveAsync().Result;
            }
        }
    }
}