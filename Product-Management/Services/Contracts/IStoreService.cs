using Product_Management.Models;

namespace Product_Management.Services.Contracts
{
    public interface IStoreService
    {
        public List<StoreDto> GetStores(int RetailerId);
        public Response AddStore(StoreDto req);
        public Response DeleteStore(DeleteStoreModel req);
        public Response SendRequest(RequestDto req);
    }
}
