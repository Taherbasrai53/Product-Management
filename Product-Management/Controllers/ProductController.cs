using Microsoft.AspNetCore.Mvc;
using Product_Management.Data;
using Product_Management.Models;
using Product_Management.Repositories;

namespace Product_Management.Controllers
{
    [ApiController]
    [Route("api/[Controller]")]
    public class ProductController:Controller
    {
        private ApplicationDbContext _dbContext;
        private IProductsRepository _repo;
        public ProductController(ApplicationDbContext dbContext, IProductsRepository repo)
        {
            _dbContext = dbContext;
            _repo = repo;
        }

        [HttpGet("/SearchProducts")]
        public async Task<ActionResult> searchProducts([FromHeader] int StoreID, [FromHeader] string query)
        {
            try
            {
                var res=await _repo.SearchProducts(StoreID, query);

                return Ok(res);
            }
            catch(Exception ex)
            {
                return Problem(ex.Message);
            }
        }

        [HttpGet("/GetByStore")]
        public async Task<ActionResult> GetByStore([FromHeader] int StoreID)
        {
            try
            {
                var res = await _repo.GetByStore(StoreID);

                return Ok(res);
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
        }
        
        [HttpPost("/AddProducts")]
        public async Task<ActionResult> AddProducts(Product req)
        {
            try
            {
                var res = await _repo.AddProducts(req);

                return Ok(res);
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
        }

        [HttpPut("/UpdateProducts")]
        public async Task<ActionResult> UpdateProducts(Product req)
        {
            try
            {
                var res = await _repo.UpdateProducts(req);

                return Ok(res);
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
        }

        //[HttpPost("/GetByRetailer")]
        //public async Task<ActionResult> GetByRetailer(GetByRetailerRequest req)
        //{
        //    try
        //    {

        //    }
        //    catch (Exception ex)
        //    {
        //        return Problem(ex.Message);
        //    }
        //}
    }
}
