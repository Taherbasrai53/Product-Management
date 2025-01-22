using Product_Management.Models;

namespace Product_Management.Repositories.Contracts
{
    public interface IStoresRepository
    {
        public List<StoreDto> GetStores(int RetailerId);
        public Response AddStore(StoreDto req);
        public Response DeleteStore(int id);
        public Response SendRequest(RequestDto req);
        public Boolean CheckIfExists(int ID, string Name, int RetailerID, int flag);
    }
}
