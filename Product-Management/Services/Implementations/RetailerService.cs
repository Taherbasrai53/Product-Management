using Product_Management.Models;
using Product_Management.Repositories;
using Product_Management.Repositories.Contracts;
using Product_Management.Services.Contracts;
using static Product_Management.Models.Product;

namespace Product_Management.Services.Implementations
{

    public class RetailerService : IRetailerService
    {
        private readonly IRetailersRepository _repo;
        private HashSet<int> Ids;
        private HashSet<string> Names;
        public RetailerService(IRetailersRepository repo)
        {
            _repo = repo;
            Ids = new HashSet<int>();
            Names = new HashSet<string>();
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
            var checkRetailer = _repo.CheckIfExists(0, req.Name, 1);
            if(checkRetailer)
            {
                return new Response(false, "The retailer already exists");
            }
            
            return _repo.AddRetailer(req);
        }

        public Response DeleteRetailer(int id)
        {
            var checkRetailer = _repo.CheckIfExists(id, "", 2);
            if (checkRetailer)
            {
                return new Response(false, "The retailer doesnt exist");
            }

            return _repo.DeleteRetailer(id);
        }

        public List<RetailerDto> GetRetailers()
        {
            return _repo.GetRetailers();
        }
    }
}
