using Product_Management.Models;
using Product_Management.Repositories;
using Product_Management.Repositories.Contracts;
using Product_Management.Services.Contracts;

namespace Product_Management.Services.Implementations
{
    public class ProductService : IProductService
    {
        IProductsRepository _repo;
        IStoresRepository _storeRepo;
        // read about Ninject bindings

        public ProductService(IProductsRepository repo, IStoresRepository storeRepo)
        {
            _repo = repo;
            _storeRepo = storeRepo;
        }
        public Response AddProducts(ProductDto req)
        {
            if (String.IsNullOrEmpty(req.Name))
            {
                return new Response(false, "Please enter a valid name for the product");
            }

            var checkStore = _storeRepo.CheckIfExists(req.StoreId, "", 0, 2);
            if(checkStore)
            {
                return new Response(false, "Store Doesnt Exist");
            }

            var checkProduct = _repo.CheckIfExists(req.ID, req.Name, req.StoreId, 3);
            if (checkProduct)
            {
                return new Response(false, "Product with given name already exists");
            }

            return _repo.AddProducts(req);
        }

        public Response DeleteProduct(int id, int storeID)
        {
            var checkProduct = _repo.CheckIfExists(id, "", storeID, 2);
            if (checkProduct)
            {
                return new Response(false, "Product doesnt exist");
            }

            return _repo.DeleteProduct(id, storeID);
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
            if(String.IsNullOrEmpty(req.Name))
            {
                return new Response(false, "Please enter a valid name for the product");
            }
            var checkProduct = _repo.CheckIfExists(req.ID, req.Name, req.StoreId, 1);
            if (checkProduct)
            {
                return new Response(false, "Product with given name already exists");
            }

            return _repo.UpdateProducts(req);
        }
    }
}
