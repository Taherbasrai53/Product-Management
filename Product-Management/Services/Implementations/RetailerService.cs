using Product_Management.Models;
using Product_Management.Repositories;
using Product_Management.Repositories.Contracts;
using Product_Management.Services.Contracts;
using static Product_Management.Models.Product;

namespace Product_Management.Services.Implementations
{

    public class RetailerService : IRetailerService
    {
        IRetailersRepository _repo;
        public RetailerService(IRetailersRepository repo)
        {
            _repo = repo;
        }

        public Response AddRetailer(RetailerDto req)
        {
            if (req == null)
            {
                return new Response(false, "req body is necessary");
            }
            if (string.IsNullOrEmpty(req.Name))
            {
                return new Response(false, "please enter a valid name");
            }
            //var retailer = new Retailer
            //{
            //    Name = req.Name,

            //};
            return _repo.AddRetailer(req);
        }

        public Response DeleteRetailer(DeleteRetailerRequest item)
        {
            return _repo.DeleteRetailer(item);
        }

        public List<RetailerDto> GetRetailers()
        {
            return _repo.GetRetailers();
        }
    }
}
