using Product_Management.Models;

namespace Product_Management.Repositories.Contracts
{
    public interface IRetailersRepository
    {
        public List<RetailerDto> GetRetailers();
        public Response AddRetailer(RetailerDto item);
        public Response DeleteRetailer(int id);
        public Boolean CheckIfExists(int ID, string Name, int flag);
    }
}
