using Microsoft.AspNetCore.Mvc;
using Product_Management.Data;
using Product_Management.Models;
using Product_Management.Repositories;
using Product_Management.Services;
using Product_Management.Services.Contracts;

namespace Product_Management.Controllers
{
    [ApiController]
    [Route("api/[Controller]")]
    public class RetailerController:Controller
    {
        //private ApplicationDbContext _dbContext;
        private readonly IRetailerService _service;
        public RetailerController(IRetailerService service)
        {
            _service = service;
        }

        [HttpGet("/GetRetailers")]
        public ActionResult GetRetailers()
        {
            try
            {
                var res= _service.GetRetailers();
                return Ok(res);
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
        }

        [HttpPost("/AddRetailer")]
        public ActionResult AddRetailer(RetailerDto req)
        {
            try
            {
                var res= _service.AddRetailer(req);
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
        [HttpDelete("/DeleteRetailer")]
        public ActionResult DeleteRetailer(int id)
        {
            try
            {
                var res= _service.DeleteRetailer(id);
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
    }
}
