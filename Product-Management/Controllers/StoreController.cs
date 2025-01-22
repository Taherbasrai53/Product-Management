using Microsoft.AspNetCore.Mvc;
using Product_Management.Models;
using Product_Management.Repositories;
using Product_Management.Services;
using Product_Management.Services.Contracts;

namespace Product_Management.Controllers
{
    [ApiController]
    [Route("api/[Controller]")]
    public class StoreController:Controller
    {
        private IStoreService _service;
        public StoreController(IStoreService service)
        {
            _service = service;
        }

        [HttpGet("/GetStores")]
        public ActionResult GetStores([FromHeader] int retailerId)
        {
            try
            {
                var res = _service.GetStores(retailerId);
                return Ok(res);
            }
            catch(Exception ex)
            {
                return Problem(ex.Message);
            }
        }

        [HttpPost("/AddStore")]
        public ActionResult AddStore(StoreDto req)
        {
            try
            {
                var res = _service.AddStore(req);
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

        [HttpDelete("/DeleteStore")]
        public ActionResult DeleteStore([FromHeader] int id)
        {
            try
            {
                var res = _service.DeleteStore(id);

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
        public ActionResult AddRequest(RequestDto req)
        {
            try
            {
                var res = _service.SendRequest(req);

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
