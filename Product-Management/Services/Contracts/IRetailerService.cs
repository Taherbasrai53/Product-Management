using Product_Management.Models;

namespace Product_Management.Services.Contracts
{
    public interface IRetailerService
    {
        public List<RetailerDto> GetRetailers();
        public Response AddRetailer(RetailerDto item);
        public Response DeleteRetailer(int id);
    }
}
