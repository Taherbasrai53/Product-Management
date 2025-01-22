using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Product_Management.Data;
using Product_Management.Models;
using Product_Management.Repositories.Contracts;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using static Product_Management.Models.Product;
using static Product_Management.Models.Request;


namespace Product_Management.Repositories
{
    public class StoresRepository : IStoresRepository
    {
        private ApplicationDbContext _dbContext;
        private readonly IConfiguration _configuration;

        #region
        const string PROC_STORE_SAVE = "dbo.Proc_Stores_Save";
        const string PROC_STORE_REQUEST = "dbo.Proc_Stores_Request";
        #endregion
        public StoresRepository(ApplicationDbContext dbContext, IConfiguration configuration)
        {
            _dbContext = dbContext;
            _configuration = configuration;
        }

        public List<StoreDto> GetStores(int RetailerId)
        {
            try
            {
                var resQuery = from stores in _dbContext.Stores
                               where stores.RetailerID == RetailerId
                               select new StoreDto
                               {
                                   ID = stores.ID,
                                   Name = stores.Name,
                                   Franchisee = stores.Franchisee
                               };

                var res = resQuery.ToList();

                return res;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Boolean CheckIfExists(int ID, string Name, int RetailerID, int flag)
        {
            if (flag == 1)
            {
                //add validation
                var existingStore = _dbContext.Stores.Where(r => r.Name == Name && r.RetailerID == RetailerID).FirstOrDefault();

                if (existingStore != null)
                {
                    return true;
                }

                return false;
            }
            else if (flag == 2)
            {
                //delete validation
                var existingStore = _dbContext.Stores.Where(r => r.ID == ID).FirstOrDefault();
                if (existingStore == null)
                {
                    return true;
                }

                return false;
            }
            
            return false;
        }

        public Response AddStore(StoreDto req)
        {
            //var existingStore = _dbContext.Stores.Where(r => r.Name == req.Name && r.RetailerID==req.RetailerID).FirstOrDefault();

            //if (existingStore != null)
            //{
            //    return new Response(false, "Store already exists");
            //}

            var store = new Store
            {
                RetailerID = req.RetailerID,
                Name = req.Name,
                Franchisee = req.Franchisee,
            };

            _dbContext.Stores.Add(store);
            _dbContext.SaveChanges();
            return new Response(true, "Store added successfully");
        }

        public Response DeleteStore(int id)
        {
            try
            {
                var existingStore = _dbContext.Stores.Where(r => r.ID == id).FirstOrDefault();
                //if (existingStore == null)
                //{
                //    return new Response(false, "Retailer Doesnt Exist");
                //}

                _dbContext.Stores.Remove(existingStore);
                _dbContext.SaveChanges();

                return new Response(true, "Store Deleted Successfully");
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Response SendRequest(RequestDto req)
        {
            try
            {
                int retailerId = 0;
                //var existingProduct= _dbContext.Products.Where(p=> p.ID==req.ProductID).FirstOrDefault();
                //if(existingProduct == null)
                //{
                //    return new Response(false, "Product Doesnt Exist");
                //}

                var existingStore = _dbContext.Stores.Where(r => r.ID == req.StoreID).FirstOrDefault();
                //if (existingstore == null)
                //{
                //    return new response(false, "store doesnt exist");
                //}

                retailerId = existingStore.RetailerID;

                var request = new Request
                {
                    ProductID = req.ProductID,
                    RetailerID = retailerId,
                    StoreID = req.StoreID,
                    Quantity = req.Quantity,
                    status = EStatus.Pending
                };

                _dbContext.Requests.Add(request);
                _dbContext.SaveChanges();

                return new Response(true, "REquest Added Successfully");
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }

    public class StoresADORepository : IStoresRepository
    {
        private readonly IConfiguration _configuration;


        public StoresADORepository(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public Response AddStore(StoreDto req)
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

	INSERT INTO [ProductManagemnt].[dbo].Stores (RetailerID, Name, Franchisee) VALUES (@RetailerId, @Name, @Franchisee)

	select 0 as ReturnValue;
    return;
";

                sqlComm.Parameters.AddWithValue("@RetailerId", req.RetailerID);
                sqlComm.Parameters.AddWithValue("@Name", req.Name);
                sqlComm.Parameters.AddWithValue("@Franchisee", req.Franchisee);

                SqlParameter returnParameter = new SqlParameter();
                returnParameter.Direction = System.Data.ParameterDirection.ReturnValue;
                sqlComm.Parameters.Add(returnParameter);

                SqlDataReader reader = sqlComm.ExecuteReader();
                if (reader.Read())
                {
                    int returnValue = reader.GetInt32(reader.GetOrdinal("ReturnValue"));

                    if (returnValue == -1)
                    {
                        return new Response(false, "Select a valid Retailer");
                    }
                    else if (returnValue == -2)
                    {
                        return new Response(false, "Store already exists");
                    }

                    return new Response(true, "Store Added successfully");
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
                    {
                        sqlConn.Close();
                    }
                    catch { }
                }
            }
        }

        public Response DeleteStore(int id)
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


Delete from [ProductManagemnt].[dbo].Stores were ID=@ID;
select 0 as ReturnValue;
        return;
";

                sqlComm.Parameters.AddWithValue("@ID", id);

                SqlParameter returnParameter = new SqlParameter();
                returnParameter.Direction = System.Data.ParameterDirection.ReturnValue;
                sqlComm.Parameters.Add(returnParameter);

                SqlDataReader reader = sqlComm.ExecuteReader();
                if (reader.Read())
                {
                    int returnValue = reader.GetInt32(reader.GetOrdinal("ReturnValue"));

                    if (returnValue == -1)
                    {
                        return new Response(false, "Select a valid Store");
                    }


                    return new Response(true, "Store Deleted successfully");
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
                    {
                        sqlConn.Close();
                    }
                    catch { }
                }
            }
        }

        public List<StoreDto> GetStores(int RetailerId)
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
Select * from [ProductManagemnt].[dbo].Stores
where RetailerID=@RetailerId
";

                sqlComm.Parameters.AddWithValue("@RetailerId", RetailerId);

                SqlDataReader reader = sqlComm.ExecuteReader();
                List<StoreDto> stores = new List<StoreDto>();

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        StoreDto store = new StoreDto();
                        store.ID = reader.IsDBNull(reader.GetOrdinal("ID")) ? 0 : reader.GetInt32(reader.GetOrdinal("ID"));
                        store.Name = reader.IsDBNull(reader.GetOrdinal("Name")) ? "" : reader.GetString(reader.GetOrdinal("Name"));
                        store.Franchisee = reader.IsDBNull(reader.GetOrdinal("Franchisee")) ? "" : reader.GetString(reader.GetOrdinal("Franchisee"));

                        stores.Add(store);
                    }
                }

                return stores;
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

        public Response SendRequest(RequestDto req)
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
declare @RetailerID int;


	select @RetailerID=RetailerID from [ProductManagemnt].[dbo].Stores where ID=@StoreID

	Insert into [ProductManagemnt].[dbo].Requests (ProductID, StoreID, Quantity, status, RetailerID) values (@ProductID, @StoreID, @Quantity, 1, @RetailerID)

	select 0 as ReturnValue;
    return;
";

                sqlComm.Parameters.AddWithValue("@ProductID", req.ProductID);
                sqlComm.Parameters.AddWithValue("@StoreID", req.StoreID);
                sqlComm.Parameters.AddWithValue("@Quantity", req.Quantity);

                SqlParameter returnParameter = new SqlParameter();
                returnParameter.Direction = System.Data.ParameterDirection.ReturnValue;
                sqlComm.Parameters.Add(returnParameter);

                SqlDataReader reader = sqlComm.ExecuteReader();
                if (reader.Read())
                {
                    int returnValue = reader.GetInt32(reader.GetOrdinal("ReturnValue"));

                    if (returnValue == -1)
                    {
                        return new Response(false, "Select a valid Product");
                    }
                    else if (returnValue == -2)
                    {
                        return new Response(false, "Select a valid Store");
                    }

                    return new Response(true, "Request Added successfully");
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
                    {
                        sqlConn.Close();
                    }
                    catch { }
                }
            }
        }

        public Boolean CheckIfExists(int ID, string Name, int RetailerID, int flag)
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
                    if(exists(Select * from [ProductManagemnt].[dbo].Stores where Name= @Name and RetailerID=@RetailerId))
	                begin
		                select -1 as ReturnValue;
                        return;
	                end
                end
                else if @Flag=2
                begin
                    if not exists (Select * from [ProductManagemnt].[dbo].Stores where ID=@StoreID)
	                begin
		                select -1 as ReturnValue;
                        return;
	                end
                end

                select 0 as ReturnValue;
                return;
                ";

                sqlComm.Parameters.AddWithValue("@StoreID", ID);
                sqlComm.Parameters.AddWithValue("@RetailerID", RetailerID);
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
