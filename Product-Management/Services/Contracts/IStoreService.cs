using Product_Management.Models;

namespace Product_Management.Services.Contracts
{
    public interface IStoreService
    {
        public List<StoreDto> GetStores(int RetailerId);
        public Response AddStore(StoreDto req);
        public Response DeleteStore(int id);
        public Response SendRequest(RequestDto req);
    }
}
