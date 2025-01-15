using Microsoft.EntityFrameworkCore;
using Product_Management.Data;
using Product_Management.Models;

namespace Product_Management.Repositories
{
    public interface IRetailersRepository
    {
        public Task<List<Retailer>> GetRetailers();
        public Task<Response> AddRetailer(AddRetailerRequest item);
        public Task<Response> DeleteRetailer(DeleteRetailerRequest item);
    }
    public class RetailersRepository:IRetailersRepository
    {
        private ApplicationDbContext _dbContext;
        public RetailersRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<List<Retailer>> GetRetailers()
        {
            try
            {
                List<Retailer> res = new List<Retailer>();
                res= await _dbContext.Retailers.ToListAsync();

                return res;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<Response> AddRetailer(AddRetailerRequest req)
        {
            try
            {
                if (req == null)
                {
                    return new Response(false, "req body is necessary");
                }
                if (String.IsNullOrEmpty(req.Name))
                {
                    return new Response(false, "please enter a valid name");
                }

                var existingRetailer = await _dbContext.Retailers.Where(r => r.Name == req.Name).FirstOrDefaultAsync();

                if (existingRetailer != null)
                {
                    return new Response(false, "Retailer already exists");
                }

                var retailer = new Retailer
                {
                    Name = req.Name,
                };

                await _dbContext.Retailers.AddAsync(retailer);
                await _dbContext.SaveChangesAsync();
                return new Response(true, "Retailer added successfully");
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<Response> DeleteRetailer(DeleteRetailerRequest req)
        {
            try
            {
                var existingRetailer= await _dbContext.Retailers.Where(r=> r.ID==req.Id).FirstOrDefaultAsync();
                if(existingRetailer == null)
                {
                    return new Response(false, "Retailer Doesnt Exist");
                }

                _dbContext.Retailers.Remove(existingRetailer);
                await _dbContext.SaveChangesAsync();

                return new Response(true, "Retailer Deleted Successfully");
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
