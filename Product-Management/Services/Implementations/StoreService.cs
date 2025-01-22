using Product_Management.Models;
using Product_Management.Repositories;
using Product_Management.Repositories.Contracts;
using Product_Management.Services.Contracts;

namespace Product_Management.Services.Implementations
{

    public class StoreService : IStoreService
    {
        private readonly IStoresRepository _repo;
        private readonly IProductsRepository _prodRepo;
        public StoreService(IStoresRepository repo, IProductsRepository prodRepo)
        {
            _repo = repo;
            _prodRepo = prodRepo;
        }

        public Response AddStore(StoreDto req)
        {
            var check = _repo.CheckIfExists(req.ID, req.Name, req.RetailerID, 1);
            if (check)
            {
                return new Response(false, "Store with given name already exists");
            }

            return _repo.AddStore(req);
        }

        public Response DeleteStore(int id)
        {

            var check = _repo.CheckIfExists(id, "", 0, 2);
            if(check)
            {
                return new Response(false, "Store Doesnt exist");
            }

            var res= _repo.DeleteStore(id);
            return res;
        }

        public List<StoreDto> GetStores(int RetailerId)
        {
            return _repo.GetStores(RetailerId);
        }

        public Response SendRequest(RequestDto req)
        {
            var checkStore = _repo.CheckIfExists(req.StoreID, "", 0, 2);
            if (checkStore)
            {
                return new Response(false, "Store Doesnt exist");
            }
            var productCheck = _prodRepo.CheckIfExists(req.ProductID, "", req.StoreID, 2);
            if(productCheck)
            {
                return new Response(false, "Product Doesnt exist");
            }

            return _repo.SendRequest(req);
        }
    }
}
