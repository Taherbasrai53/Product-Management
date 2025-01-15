using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Product_Management.Data;
using Product_Management.Models;


namespace Product_Management.Repositories
{
    public interface IStoresRepository
    {
        public Task<List<Store>> GetStores(int RetailerId);
        public Task<Response> AddStore(AddStoreModel req);
        public Task<Response> DeleteStore(DeleteStoreModel req);
        public Task<Response> SendRequest(Request req);
    }
    public class StoresRepository:IStoresRepository
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

        public async Task<List<Store>> GetStores(int RetailerId)
        {
            try
            {
                var resQuery = from stores in _dbContext.Stores
                               where stores.RetailerID == RetailerId
                               select stores;

                var res = await resQuery.ToListAsync();

                return res;
            }
            catch(Exception ex)
            {
                throw;
            }
        }

        public async Task<Response> AddStore(AddStoreModel req)
        {
            SqlConnection sqlConn=null;
            try
            {
                string connectionString = _configuration.GetConnectionString("DefaultConnection");
                sqlConn = new SqlConnection(connectionString);
                sqlConn.Open();
                SqlCommand sqlComm = sqlConn.CreateCommand();

                sqlComm.CommandType = System.Data.CommandType.StoredProcedure;
                sqlComm.CommandText = PROC_STORE_SAVE;

                sqlComm.Parameters.AddWithValue("@RetailerId", req.RetailerID);
                sqlComm.Parameters.AddWithValue("@Name", req.Name);
                sqlComm.Parameters.AddWithValue("@Franchisee", req.Franchisee);

                SqlParameter returnParameter = new SqlParameter();
                returnParameter.Direction = System.Data.ParameterDirection.ReturnValue;
                sqlComm.Parameters.Add(returnParameter);

                sqlComm.ExecuteNonQuery();
                int returnValue = returnParameter.Value != DBNull.Value && returnParameter.Value != null ? (int)returnParameter.Value : -1;

                if(returnValue==-1)
                {
                    return new Response(false, "Select a valid Retailer");
                }
                else if(returnValue==-2)
                {
                    return new Response(false, "Store already exists");
                }

                return new Response(true, "Store Added successfully");
            }
            catch(Exception ex)
            {
                throw;
            }
            finally
            {
                if(sqlConn!=null)
                {
                    try
                    {
                        sqlConn.Close();
                    }
                    catch { }
                }
            }
        }

        public async Task<Response> DeleteStore(DeleteStoreModel req)
        {
            try
            {
                var existingStore = await _dbContext.Stores.Where(r => r.ID == req.ID).FirstOrDefaultAsync();
                if (existingStore == null)
                {
                    return new Response(false, "Retailer Doesnt Exist");
                }

                _dbContext.Stores.Remove(existingStore);
                await _dbContext.SaveChangesAsync();

                return new Response(true, "Store Deleted Successfully");
            }
            catch(Exception ex)
            {
                throw;
            }
        }

        public async Task<Response> SendRequest(Request req)
        {
            SqlConnection sqlConn = null;
            try
            {
                string connectionString = _configuration.GetConnectionString("DefaultConnection");
                sqlConn = new SqlConnection(connectionString);
                sqlConn.Open();
                SqlCommand sqlComm = sqlConn.CreateCommand();

                sqlComm.CommandType = System.Data.CommandType.StoredProcedure;
                sqlComm.CommandText = PROC_STORE_REQUEST;

                sqlComm.Parameters.AddWithValue("@ProductID", req.ProductID);
                sqlComm.Parameters.AddWithValue("@StoreID", req.StoreID);
                sqlComm.Parameters.AddWithValue("@Quantity", req.Quantity);

                SqlParameter returnParameter = new SqlParameter();
                returnParameter.Direction = System.Data.ParameterDirection.ReturnValue;
                sqlComm.Parameters.Add(returnParameter);

                sqlComm.ExecuteNonQuery();
                int returnValue = returnParameter.Value != DBNull.Value && returnParameter.Value != null ? (int)returnParameter.Value : -1;

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
    }
}
