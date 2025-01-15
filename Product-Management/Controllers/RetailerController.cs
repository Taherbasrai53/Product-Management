using Microsoft.AspNetCore.Mvc;
using Product_Management.Data;
using Product_Management.Models;
using Product_Management.Repositories;

namespace Product_Management.Controllers
{
    [ApiController]
    [Route("api/[Controller]")]
    public class RetailerController:Controller
    {
        //private ApplicationDbContext _dbContext;
        private IRetailersRepository _repo;
        public RetailerController(IRetailersRepository repo)
        {
            _repo = repo;
        }

        [HttpGet("/GetRetailers")]
        public async Task<ActionResult> GetRetailers()
        {
            try
            {
                var res= await _repo.GetRetailers();
                return Ok(res);
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
        }

        [HttpPost("/AddRetailer")]
        public async Task<ActionResult> AddRetailer(AddRetailerRequest req)
        {
            try
            {
                var res= await _repo.AddRetailer(req);
                if(!res.success)
                {
                    return BadRequest(res);
                }

                return Ok(res);
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
        }
        [HttpPost("/DeleteRetailer")]
        public async Task<ActionResult> DeleteRetailer(DeleteRetailerRequest req)
        {
            try
            {
                var res= await _repo.DeleteRetailer(req);
                if(!res.success)
                {
                    return BadRequest(req);
                }

                return Ok(res);
            }
            catch (Exception ex)
            {
                return Problem(ex.Message); 
            }
        }
    }
}
