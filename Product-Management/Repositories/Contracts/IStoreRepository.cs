using Product_Management.Models;

namespace Product_Management.Repositories.Contracts
{
    public interface IStoresRepository
    {
        public List<StoreDto> GetStores(int RetailerId);
        public Response AddStore(StoreDto req);
        public Response DeleteStore(DeleteStoreModel req);
        public Response SendRequest(RequestDto req);
    }
}
