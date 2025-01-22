using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Product_Management.Data;
using Product_Management.Models;
using Product_Management.Repositories.Contracts;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using static Product_Management.Models.Product;

namespace Product_Management.Repositories.Implementations
{


    public class ProductsRepository : IProductsRepository
    {
        #region
        private const string PROC_PRODUCTS_SEARCH = "dbo.Proc_Search_Product";
        private const string PROC_PRODUCTS_ADD = "dbo.Proc_Products_Add";
        private const string PROC_PRODUCTS_UPDATE = "Proc_Products_Update";
        #endregion

        private readonly ApplicationDbContext _dbContext;
        private readonly IConfiguration _configuration;
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

        public List<ProductDto> SearchProducts(int storeId, string query)
        {
            var result = _dbContext.Products
                        .Where(p => p.StoreId == storeId && p.Name.Contains(query))
                        .Select(p => new ProductDto
                        {
                            ID = p.ID,
                            Name = p.Name,
                            Price = p.Price,
                            Quantity = p.Quantity,
                            Performance = p.Performance
                        })
                        .ToList();

            return result;

        }

        public List<ProductDto> GetByStore(int storeId)
        {
            try
            {
                var resQuery = from p in _dbContext.Products
                               where p.StoreId == storeId
                               select new ProductDto
                               {
                                   ID = p.ID,
                                   Name = p.Name,
                                   Price = p.Price,
                                   Quantity = p.Quantity,
                                   Performance = p.Performance
                               };

                var res = resQuery.ToList();

                return res;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Response AddProducts(ProductDto req)
        {
            try
            {                

                var product = new Product
                {
                    Name = req.Name,
                    StoreId = req.StoreId,
                    Price = req.Price,
                    Quantity = req.Quantity,
                    Performance = EnumPerformance.Medium,
                };

                _dbContext.Products.Add(product);
                _dbContext.SaveChanges();
                return new Response(true, "Product added successfully");
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Response UpdateProducts(ProductDto req)
        {
            try
            {                

                var productToUpdate = _dbContext.Products
                                    .Where(p => p.ID == req.ID && p.StoreId == req.StoreId)
                                    .FirstOrDefault();
                

                productToUpdate.Name = req.Name;
                productToUpdate.Quantity = req.Quantity;
                productToUpdate.Price = req.Price;
                productToUpdate.Performance = req.Performance;

                _dbContext.SaveChanges();
                return new Response(true, "Product updated successfully");
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Response DeleteProduct(int id, int storeID)  //read more about tasks 
        {
            try
            {
                var existingRetailer = _dbContext.Products.Where(r => r.ID == id && r.StoreId == storeID).FirstOrDefault();
                

                _dbContext.Products.Remove(existingRetailer);
                _dbContext.SaveChanges(); //async and await.. moment the main tread hits here.. the main thread would go back and the reamining execution might be done by a different thread

                return new Response(true, "Product Deleted Successfully");
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Boolean CheckIfExists(int ID, string Name, int StoreID, int flag)
        {
            if (flag == 1)
            {
                // update check
                var existingProduct = _dbContext.Products.Where(r => r.ID != ID && r.StoreId == StoreID && r.Name == Name).FirstOrDefault();
                if (existingProduct != null)
                {
                    return true;
                }
            }
            else if (flag == 2)
            {
                // delete check
                var existingRetailer = _dbContext.Products.Where(r => r.ID == ID && r.StoreId == StoreID).FirstOrDefault();
                if (existingRetailer == null)
                {
                    return true;
                }
            }
            else if (flag == 3)
            {
                //add check
                var existingProduct = _dbContext.Products.Where(r => r.StoreId == StoreID && r.Name == Name).FirstOrDefault();

                if (existingProduct != null)
                {
                    return true;
                }
            }
            
            return false;
        }
    }

    public class ProductsADORepository : IProductsRepository
    {
        private readonly IConfiguration _configuration;
        public ProductsADORepository(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public Response AddProducts(ProductDto req)
        {
            SqlConnection sqlConn = null;
            try
            {
                string connectionString = _configuration.GetConnectionString("DefaultConnection");
                sqlConn = new SqlConnection(connectionString);
                sqlConn.Open();
                SqlCommand sqlComm = sqlConn.CreateCommand();

                sqlComm.CommandType = System.Data.CommandType.Text;
                sqlComm.CommandText = @"	
	Insert into Products (Name, Price, Quantity, StoreId, Performance) values(@Name, @Price, @Quantity, @StoreId, 1);
        select 0 as ReturnValue;
        return;
";

                sqlComm.Parameters.AddWithValue("@Name", req.Name);
                sqlComm.Parameters.AddWithValue("@Price", req.Price);
                sqlComm.Parameters.AddWithValue("@Quantity", req.Quantity);
                sqlComm.Parameters.AddWithValue("@StoreId", req.StoreId);

                SqlParameter returnParameter = new SqlParameter();
                returnParameter.Direction = System.Data.ParameterDirection.ReturnValue;
                sqlComm.Parameters.Add(returnParameter);

                //int returnValue = returnParameter.Value != DBNull.Value && returnParameter.Value != null ? (int)returnParameter.Value : -1;

                SqlDataReader reader = sqlComm.ExecuteReader();
                if (reader.Read())
                {
                    int returnValue = reader.GetInt32(reader.GetOrdinal("ReturnValue"));

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

                return new Response(false, "Unexpected error occurred");

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

        public Response DeleteProduct(int id, int storeID)
        {
            SqlConnection sqlConn = null;
            try
            {
                string connectionString = _configuration.GetConnectionString("DefaultConnection");
                sqlConn = new SqlConnection(connectionString);
                sqlConn.Open();
                SqlCommand sqlComm = sqlConn.CreateCommand();

                sqlComm.CommandType = System.Data.CommandType.Text;
                sqlComm.CommandText = @"


Delete from Products where ID=@ID;

    select 0 as ReturnValue;
    return;
";

                sqlComm.Parameters.AddWithValue("@ID", id);
                sqlComm.Parameters.AddWithValue("@StoreId", storeID);

                SqlParameter returnParameter = new SqlParameter();
                returnParameter.Direction = System.Data.ParameterDirection.ReturnValue;
                sqlComm.Parameters.Add(returnParameter);


                SqlDataReader reader = sqlComm.ExecuteReader();
                if (reader.Read())
                {
                    int returnValue = reader.GetInt32(reader.GetOrdinal("ReturnValue"));
                    if (returnValue == -1)
                    {
                        return new Response(false, "product doesnt exist");
                    }

                    return new Response(true, "Product deleted successfully");
                }

                return new Response(false, "Unexpected error occurred");
                //return new Response(true, "Product deleted successfully");
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

        public List<ProductDto> GetByStore(int storeId)
        {
            SqlConnection sqlConn = null;
            try
            {
                string connectionString = _configuration.GetConnectionString("DefaultConnection");
                sqlConn = new SqlConnection(connectionString);
                sqlConn.Open();
                SqlCommand sqlComm = sqlConn.CreateCommand();

                sqlComm.CommandType = System.Data.CommandType.Text;
                sqlComm.CommandText = @"
Select * from Products 
		where StoreId=@StoreID;
";

                sqlComm.Parameters.AddWithValue("@StoreId", storeId);


                SqlDataReader reader = sqlComm.ExecuteReader();
                List<ProductDto> products = new List<ProductDto>();

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        ProductDto product = new ProductDto();
                        product.ID = reader.IsDBNull(reader.GetOrdinal("ID")) ? 0 : reader.GetInt32(reader.GetOrdinal("ID"));
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

        public List<ProductDto> SearchProducts(int storeId, string query)
        {
            SqlConnection sqlConn = null;
            try
            {
                string connectionString = _configuration.GetConnectionString("DefaultConnection");
                sqlConn = new SqlConnection(connectionString);
                sqlConn.Open();
                SqlCommand sqlComm = sqlConn.CreateCommand();

                sqlComm.CommandType = System.Data.CommandType.Text;
                sqlComm.CommandText = @"
Select * from Products 
		where StoreId=@StoreID and 
		Name like '%' + @Query + '%';
";

                sqlComm.Parameters.AddWithValue("@StoreId", storeId);
                sqlComm.Parameters.AddWithValue("@Query", query);

                SqlDataReader reader = sqlComm.ExecuteReader();
                List<ProductDto> products = new List<ProductDto>();

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        ProductDto product = new ProductDto();
                        product.ID = reader.IsDBNull(reader.GetOrdinal("ID")) ? 0 : reader.GetInt32(reader.GetOrdinal("ID"));
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

        public Response UpdateProducts(ProductDto req)
        {
            SqlConnection sqlConn = null;
            try
            {
                string connectionString = _configuration.GetConnectionString("DefaultConnection"); // we dont use SPs anymore 
                sqlConn = new SqlConnection(connectionString);
                sqlConn.Open();
                SqlCommand sqlComm = sqlConn.CreateCommand();

                sqlComm.CommandType = System.Data.CommandType.Text;
                sqlComm.CommandText = @"
	Update Products set Name=@Name, Quantity=@Quantity, Price=@Price, Performance=@Performance where ID=@ID and StoreId=@StoreId

	select 0 as ReturnValue;
    return;
";

                sqlComm.Parameters.AddWithValue("@ID", req.ID);
                sqlComm.Parameters.AddWithValue("@Name", req.Name);
                sqlComm.Parameters.AddWithValue("@Price", req.Price);
                sqlComm.Parameters.AddWithValue("@Quantity", req.Quantity);
                sqlComm.Parameters.AddWithValue("@StoreId", req.StoreId);
                sqlComm.Parameters.AddWithValue("@Performance", req.Performance);

                SqlParameter returnParameter = new SqlParameter();
                returnParameter.Direction = System.Data.ParameterDirection.ReturnValue;
                sqlComm.Parameters.Add(returnParameter);


                SqlDataReader reader = sqlComm.ExecuteReader();
                if (reader.Read())
                {
                    int returnValue = reader.GetInt32(reader.GetOrdinal("ReturnValue"));
                    if (returnValue == -1)
                    {
                        return new Response(false, "Add a valid Product");
                    }
                    else if (returnValue == -2)
                    {
                        return new Response(false, "Product with this name already exists");
                    }

                    return new Response(true, "Product Updated successfully");
                }


                return new Response(false, "Unexpected Error occured");

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

        public Boolean CheckIfExists(int ID, string Name, int StoreID, int flag)
        {
            SqlConnection sqlConn = null;
            try
            {
                string connectionString = _configuration.GetConnectionString("DefaultConnection");
                sqlConn = new SqlConnection(connectionString);
                sqlConn.Open();
                SqlCommand sqlComm = sqlConn.CreateCommand();

                sqlComm.CommandType = System.Data.CommandType.Text;

                sqlComm.CommandText = @"
                if @Flag=1
                begin
                    -- for update check
                    if exists(select * from Products where ID<>@ID and Name=@Name and StoreId=@StoreId)
	                begin
		                select -1 as ReturnValue;
                        return;
	                end
                end
                else if @Flag=2
                begin
                    -- for delete check
                    if not exists(Select * from Products where StoreId=@StoreId and ID=@ID)
	                begin
		                select -1 as ReturnValue;
                        return;
	                end
                end
                else if @Flag=3 
                begin
                   
                     -- for add check
                    if exists(select * from Products where Name=@Name and StoreId=@StoreId)
	                begin
		                select -1 as ReturnValue;
                        return;
	                end
                end
                

                select 0 as ReturnValue;
                return;
                ";

                sqlComm.Parameters.AddWithValue("@StoreID", StoreID);
                sqlComm.Parameters.AddWithValue("@ID", ID);
                sqlComm.Parameters.AddWithValue("@Name", Name);
                sqlComm.Parameters.AddWithValue("@Flag", flag);

                SqlParameter returnParameter = new SqlParameter();
                returnParameter.Direction = System.Data.ParameterDirection.ReturnValue;
                sqlComm.Parameters.Add(returnParameter);

                SqlDataReader reader = sqlComm.ExecuteReader();
                if (reader.Read())
                {
                    int returnValue = reader.GetInt32(reader.GetOrdinal("ReturnValue"));

                    if (returnValue == -1)
                    {
                        return true;
                    }

                    return false;
                }

                return false;
            }
            catch { throw; }
            finally
            {
                if (sqlConn != null)
                {
                    try
                    {
                        sqlConn.Close();
                    }
                    catch { }
                }
            }
        }
    }
}
