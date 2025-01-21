using Product_Management.Models;

namespace Product_Management.Services.Contracts
{
    public interface IProductService
    {
        public List<ProductDto> SearchProducts(int StoreId, string query);
        public List<ProductDto> GetByStore(int storeId);
        public Response DeleteProduct(DeleteProductRequest req);
        public Response AddProducts(ProductDto req);
        public Response UpdateProducts(ProductDto req);
    }
}
