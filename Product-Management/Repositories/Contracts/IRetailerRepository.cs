using Product_Management.Models;

namespace Product_Management.Repositories.Contracts
{
    public interface IRetailersRepository
    {
        public List<RetailerDto> GetRetailers();
        public Response AddRetailer(RetailerDto item);
        public Response DeleteRetailer(DeleteRetailerRequest item);
    }
}
