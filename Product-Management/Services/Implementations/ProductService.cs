using Product_Management.Models;
using Product_Management.Repositories;
using Product_Management.Repositories.Contracts;
using Product_Management.Services.Contracts;

namespace Product_Management.Services.Implementations
{
    public class ProductService : IProductService
    {
        IProductsRepository _repo;
        // read about Ninject bindings

        public ProductService(IProductsRepository repo)
        {
            _repo = repo;
        }
        public Response AddProducts(ProductDto req)
        {
            return _repo.AddProducts(req);
        }

        public Response DeleteProduct(DeleteProductRequest req)
        {
            return _repo.DeleteProduct(req);
        }

        public List<ProductDto> GetByStore(int storeId)
        {
            return _repo.GetByStore(storeId);
        }

        public List<ProductDto> SearchProducts(int StoreId, string query)
        {
            return _repo.SearchProducts(StoreId, query);
        }

        public Response UpdateProducts(ProductDto req)
        {
            return _repo.UpdateProducts(req);
        }
    }
}
