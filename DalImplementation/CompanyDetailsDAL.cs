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
        private StockReporterContext _context;

        public CompanyDetailsDAL(StockReporterContext context)
        {
            _context = context;
        }

        public void Add(CompanyDetail entity)
        {
            _context.Add(entity);
            var result = SaveAsync();
        }

        public void Delete(CompanyDetail entity)
        {
            _context.Entry(entity).State = EntityState.Deleted;
            //_context.CompanyDetails.Remove(entity);
            var result = SaveAsync();
        }

        public void Delete(int id)
        {
            var cd = _context.CompanyDetails.FirstOrDefault(r => r.Id == id);
            if (cd != null)
            {
                Delete(cd);
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

        public async Task<bool> SaveAsync()
        {
            return await _context.SaveChangesAsync() >= 1 ? true : false;
        }

        public void Update(CompanyDetail entity)
        {
            var record = GetAsync(entity.Id).Result;
            if (record == null)
            {
                return;
            }
            record = entity;
            _context.Update(record);
            var x = SaveAsync().Result;
        }
    }
}