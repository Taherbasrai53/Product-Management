using Microsoft.AspNetCore.Mvc;
using Product_Management.Models;
using Product_Management.Repositories;

namespace Product_Management.Controllers
{
    [ApiController]
    [Route("api/[Controller]")]
    public class StoreController:Controller
    {
        private IStoresRepository _repo;
        public StoreController(IStoresRepository repo)
        {
            _repo = repo;
        }

        [HttpGet("/GetStores")]
        public async Task<ActionResult> GetStores([FromHeader] int RetailerId)
        {
            try
            {
                var res = await _repo.GetStores(RetailerId);
                return Ok(res);
            }
            catch(Exception ex)
            {
                return Problem(ex.Message);
            }
        }

        [HttpPost("/AddStore")]
        public async Task<ActionResult> AddStore(AddStoreModel req)
        {
            try
            {
                var res = await _repo.AddStore(req);
                if(!res.success)
                {
                    return BadRequest(res);
                }

                return Ok(res);
            }
            catch(Exception ex)
            {
                return Problem(ex.Message);
            }
        }

        [HttpPost("/DeleteStore")]
        public async Task<ActionResult> DeleteStore(DeleteStoreModel req)
        {
            try
            {
                var res = await _repo.DeleteStore(req);

                if(!res.success)
                {
                    return BadRequest(res);
                }

                return Ok(res);
            }
            catch(Exception ex)
            {
                return Problem(ex.Message);
            }
        }

        [HttpPost("/AddRequest")]
        public async Task<ActionResult> AddRequest(Request req)
        {
            try
            {
                var res = await _repo.SendRequest(req);

                if (!res.success)
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
