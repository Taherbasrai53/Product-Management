using Product_Management.Models;

namespace Product_Management.Repositories.Contracts
{
    public interface IProductsRepository // get rid of tasks and make the methods synchronous 
    {
        public List<ProductDto> SearchProducts(int storeId, string query);
        public List<ProductDto> GetByStore(int storeId);
        public Response DeleteProduct(DeleteProductRequest req);
        public Response AddProducts(ProductDto req);
        public Response UpdateProducts(ProductDto req);
    }
}
