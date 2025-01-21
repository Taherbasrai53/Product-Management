using Product_Management.Models;
using Product_Management.Repositories;
using Product_Management.Repositories.Contracts;
using Product_Management.Services.Contracts;

namespace Product_Management.Services.Implementations
{

    public class StoreService : IStoreService
    {
        private readonly IStoresRepository _repo;
        public StoreService(IStoresRepository repo)
        {
            _repo = repo;
        }

        public Response AddStore(StoreDto req)
        {
            return _repo.AddStore(req);
        }

        public Response DeleteStore(DeleteStoreModel req)
        {
            return _repo.DeleteStore(req);
        }

        public List<StoreDto> GetStores(int RetailerId)
        {
            return _repo.GetStores(RetailerId);
        }

        public Response SendRequest(RequestDto req)
        {
            return _repo.SendRequest(req);
        }
    }
}
