using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Product_Management.Data;
using Product_Management.Models;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using static Product_Management.Models.Product;

namespace Product_Management.Repositories
{
    
    public interface IProductsRepository
    {
        public Task<List<Product>> SearchProducts(int StoreId, string query);
        public Task<List<Product>> GetByStore(int storeId);
        public Task<Response> DeleteProduct(DeleteProductRequest req);
        public Task<Response> AddProducts(Product req);
        public Task<Response> UpdateProducts(Product req);
    }
    public class ProductsRepository:IProductsRepository
    {
        #region
            private const string PROC_PRODUCTS_SEARCH = "dbo.Proc_Search_Product";
            private const string PROC_PRODUCTS_ADD = "dbo.Proc_Products_Add";
            private const string PROC_PRODUCTS_UPDATE = "Proc_Products_Update";
        #endregion

        private ApplicationDbContext _dbContext;
        private IConfiguration _configuration;
        public ProductsRepository(ApplicationDbContext dbContext, IConfiguration configuration)
        {
            _dbContext = dbContext;
            _configuration = configuration;
        }

        //public async Task<List<Product>> GetProductsByStore(int Id)
        //{
        //    try
        //    {
        //        var resQuery= from products in _dbContext.Products
        //                      where products.StoreId == Id
        //                      select products;

        //        return resQuery.ToList();
        //    }
        //    catch (Exception ex)
        //    {
        //        throw;
        //    }
        //}

        public async Task<List<Product>> SearchProducts(int StoreId, string query)
        {
            SqlConnection sqlConn= null;
            try
            {
                string connectionString = _configuration.GetConnectionString("DefaultConnection");
                sqlConn = new SqlConnection(connectionString);
                sqlConn.Open();
                SqlCommand sqlComm = sqlConn.CreateCommand();

                sqlComm.CommandType = System.Data.CommandType.StoredProcedure;
                sqlComm.CommandText = PROC_PRODUCTS_SEARCH;

                sqlComm.Parameters.AddWithValue("@StoreId", StoreId);
                sqlComm.Parameters.AddWithValue("@Query", query);

                SqlDataReader reader = sqlComm.ExecuteReader();
                List<Product> products = new List<Product>();

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        Product product = new Product();
                        product.ID= reader.IsDBNull(reader.GetOrdinal("ID")) ? 0 : reader.GetInt32(reader.GetOrdinal("ID"));
                        product.Name = reader.IsDBNull(reader.GetOrdinal("Name")) ? "" : reader.GetString(reader.GetOrdinal("Name"));
                        product.Price = reader.IsDBNull(reader.GetOrdinal("Price")) ? 0 : reader.GetDouble(reader.GetOrdinal("Price"));
                        product.Quantity = reader.IsDBNull(reader.GetOrdinal("Quantity")) ? 0 : reader.GetInt32(reader.GetOrdinal("Quantity"));
                        product.StoreId = reader.IsDBNull(reader.GetOrdinal("StoreId")) ? 0 : reader.GetInt32(reader.GetOrdinal("StoreId"));
                        product.Quantity = reader.IsDBNull(reader.GetOrdinal("Quantity")) ? 0 : reader.GetInt32(reader.GetOrdinal("Quantity"));
                        product.Performance = reader.IsDBNull(reader.GetOrdinal("Performance")) ? EnumPerformance.Low : (EnumPerformance)reader.GetInt32(reader.GetOrdinal("Performance"));

                        products.Add(product);
                    }
                }

                return products;
            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                if (sqlConn != null)
                {
                    try
                    { sqlConn.Close(); }
                    catch { }
                }
            }
        }

        public async Task<List<Product>> GetByStore(int storeId)
        {
            try
            {
                var resQuery = from prods in _dbContext.Products
                               where prods.StoreId == storeId
                               select prods;

                var res = await resQuery.ToListAsync();

                return res;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<Response> AddProducts(Product req)
        {
            SqlConnection sqlConn = null;
            try
            {
                string connectionString = _configuration.GetConnectionString("DefaultConnection");
                sqlConn = new SqlConnection(connectionString);
                sqlConn.Open();
                SqlCommand sqlComm = sqlConn.CreateCommand();

                sqlComm.CommandType = System.Data.CommandType.StoredProcedure;
                sqlComm.CommandText = PROC_PRODUCTS_ADD;

                sqlComm.Parameters.AddWithValue("@Name", req.Name);
                sqlComm.Parameters.AddWithValue("@Price", req.Price);
                sqlComm.Parameters.AddWithValue("@Quantity", req.Quantity);
                sqlComm.Parameters.AddWithValue("@StoreId", req.StoreId);

                SqlParameter returnParameter = new SqlParameter();
                returnParameter.Direction = System.Data.ParameterDirection.ReturnValue;
                sqlComm.Parameters.Add(returnParameter);

                sqlComm.ExecuteNonQuery();
                int returnValue = returnParameter.Value != DBNull.Value && returnParameter.Value != null ? (int)returnParameter.Value : -1;

                if (returnValue == -1)
                {
                    return new Response(false, "Select a valid Store");
                }
                else if (returnValue == -2)
                {
                    return new Response(false, "Add a valid Product");
                }
                else if (returnValue == -3)
                {
                    return new Response(false, "Product with this name already exists");
                }

                return new Response(true, "Product added successfully");
            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                if (sqlConn != null)
                {
                    try
                    { sqlConn.Close(); }
                    catch { }
                }
            }
        }

        public async Task<Response> UpdateProducts(Product req)
        {
            SqlConnection sqlConn = null;
            try
            {
                string connectionString = _configuration.GetConnectionString("DefaultConnection");
                sqlConn = new SqlConnection(connectionString);
                sqlConn.Open();
                SqlCommand sqlComm = sqlConn.CreateCommand();

                sqlComm.CommandType = System.Data.CommandType.StoredProcedure;
                sqlComm.CommandText = PROC_PRODUCTS_UPDATE;

                sqlComm.Parameters.AddWithValue("@ID", req.ID);
                sqlComm.Parameters.AddWithValue("@Name", req.Name);
                sqlComm.Parameters.AddWithValue("@Price", req.Price);
                sqlComm.Parameters.AddWithValue("@Quantity", req.Quantity);
                sqlComm.Parameters.AddWithValue("@StoreId", req.StoreId);
                sqlComm.Parameters.AddWithValue("@Performance", req.Performance);

                SqlParameter returnParameter = new SqlParameter();
                returnParameter.Direction = System.Data.ParameterDirection.ReturnValue;
                sqlComm.Parameters.Add(returnParameter);

                sqlComm.ExecuteNonQuery();
                int returnValue = returnParameter.Value != DBNull.Value && returnParameter.Value != null ? (int)returnParameter.Value : -1;

                if (returnValue == -1)
                {
                    return new Response(false, "Add a valid Product");
                }                
                else if (returnValue == -2)
                {
                    return new Response(false, "Product with this name already exists");
                }

                return new Response(true, "Product updated successfully");
            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                if (sqlConn != null)
                {
                    try
                    { sqlConn.Close(); }
                    catch { }
                }
            }
        }

        public async Task<Response> DeleteProduct(DeleteProductRequest req)
        {
            try
            {
                var existingRetailer = await _dbContext.Products.Where(r => r.ID == req.ID && r.StoreId==req.StoreId).FirstOrDefaultAsync();
                if (existingRetailer == null)
                {
                    return new Response(false, "Product Doesnt Exist");
                }

                _dbContext.Products.Remove(existingRetailer);
                await _dbContext.SaveChangesAsync();

                return new Response(true, "Product Deleted Successfully");
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
