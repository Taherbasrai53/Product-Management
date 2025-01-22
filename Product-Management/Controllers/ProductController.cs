using Microsoft.AspNetCore.Mvc;
using Product_Management.Data;
using Product_Management.Models;
using Product_Management.Services;
using Product_Management.Services.Contracts;

namespace Product_Management.Controllers
{
    [ApiController]
    [Route("api/[Controller]")]
    public class ProductController:Controller
    {
        private readonly ApplicationDbContext _dbContext; // should be readonly as well (all the class firlds)
        private readonly IProductService _service;
        public ProductController(ApplicationDbContext dbContext, IProductService service)
        {
            _dbContext = dbContext;
            _service = service;
        }

        [HttpGet("/SearchProducts")] //search-products (kebab casing)
        public ActionResult SearchProducts([FromHeader] int storeId, [FromHeader] string query)  //should be Pascal cased and not camel cased
        {
            try
            {
                var res= _service.SearchProducts(storeId, query); // params should be small cased 

                return Ok(res);
            }
            catch(Exception ex)
            {
                return Problem(ex.Message);
            }
        }

        [HttpGet("/GetByStore")]
        public ActionResult GetByStore([FromHeader] int storeId)
        {
            try
            {
                var res =  _service.GetByStore(storeId);

                return Ok(res);
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
        }
        
        [HttpPost("/AddProducts")]
        public ActionResult AddProducts(ProductDto req)
        {
            try
            {
                var res = _service.AddProducts(req);

                return Ok(res);
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
        }

        [HttpPut("/UpdateProducts")]
        public ActionResult UpdateProducts(ProductDto req)
        {
            try
            {
                var res = _service.UpdateProducts(req);

                return Ok(res);
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
        }

        [HttpDelete("/DeleteProduct")]
        public ActionResult DeleteProduct([FromHeader] int id, [FromHeader] int storeId)
        {
            try
            {
                var res = _service.DeleteProduct(id, storeId);

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
