using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Product_Management.Data;
using Product_Management.Models;
using Product_Management.Repositories.Contracts;

namespace Product_Management.Repositories.Implementations
{

    public class RetailersRepository : IRetailersRepository
    {
        private ApplicationDbContext _dbContext;
        public RetailersRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public List<RetailerDto> GetRetailers()
        {
            try
            {
                List<RetailerDto> res = new List<RetailerDto>();
                res = _dbContext.Retailers
                    .Select(p => new RetailerDto
                    {
                        ID = p.ID,
                        Name = p.Name
                    })
                    .ToList();

                return res;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Response AddRetailer(RetailerDto req)
        {
            try
            {

                var retailer = new Retailer
                {
                    Name = req.Name,
                };

                _dbContext.Retailers.Add(retailer);
                _dbContext.SaveChanges();
                return new Response(true, "Retailer added successfully");
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Response DeleteRetailer(int id)
        {
            try
            {
                var existingRetailer = _dbContext.Retailers.Where(r => r.ID == id).FirstOrDefault();
                

                _dbContext.Retailers.Remove(existingRetailer);
                _dbContext.SaveChanges();

                return new Response(true, "Retailer Deleted Successfully");
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Boolean CheckIfExists(int ID, string Name, int flag)
        {
            if (flag == 1)
            {
                // add check
                var existingRetailer = _dbContext.Retailers.Where(r => r.Name == Name).FirstOrDefault();

                if (existingRetailer != null)
                {
                    return true;
                }
            }
            else if (flag == 2)
            {
                // delete check
                var existingRetailer = _dbContext.Retailers.Where(r => r.ID == ID).FirstOrDefault();
                if (existingRetailer == null)
                {
                    return true;
                }
            }            

            return false;
        }
    }

    public class RetailersADORepository : IRetailersRepository
    {
        IConfiguration _configuration;
        public RetailersADORepository(IConfiguration config)
        {
            _configuration = config;
        }
        public Response AddRetailer(RetailerDto req)
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
                    

                    Insert into [ProductManagemnt].[dbo].Retailers (Name) values(@Name)
                    select 0 as ReturnValue;
                        return;
                    ";

                sqlComm.Parameters.AddWithValue("@Name", req.Name);


                SqlParameter returnParameter = new SqlParameter();
                returnParameter.Direction = System.Data.ParameterDirection.ReturnValue;
                sqlComm.Parameters.Add(returnParameter);

                SqlDataReader reader = sqlComm.ExecuteReader();
                if (reader.Read())
                {
                    int returnValue = reader.GetInt32(reader.GetOrdinal("ReturnValue"));

                    if (returnValue == -1)
                    {
                        return new Response(false, "Retailer already exists");
                    }

                    return new Response(true, "Retailer added successfully");
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

        public Response DeleteRetailer(int id)
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
    

    Delete from [ProductManagemnt].[dbo].Retailers where ID=@Id;

    select 0 as ReturnValue;
    ";

                sqlComm.Parameters.AddWithValue("@Id", id);

                SqlDataReader reader = sqlComm.ExecuteReader();
                if (reader.Read())
                {
                    int returnValue = reader.GetInt32(reader.GetOrdinal("ReturnValue"));

                    if (returnValue == -1)
                    {
                        return new Response(false, "Retailer doesn't exist");
                    }

                    return new Response(true, "Retailer deleted successfully");
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

        public List<RetailerDto> GetRetailers()
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
Select * from [ProductManagemnt].[dbo].Retailers
";


                List<RetailerDto> retailers = new List<RetailerDto>();

                SqlDataReader reader = sqlComm.ExecuteReader();

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        RetailerDto ret = new RetailerDto();
                        ret.ID = reader.IsDBNull(reader.GetOrdinal("ID")) ? 0 : reader.GetInt32(reader.GetOrdinal("ID"));
                        ret.Name = reader.IsDBNull(reader.GetOrdinal("Name")) ? "" : reader.GetString(reader.GetOrdinal("Name"));


                        retailers.Add(ret);
                    }
                }

                return retailers;
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

        public Boolean CheckIfExists(int ID, string Name, int flag)
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
                    if exists(select * from [ProductManagemnt].[dbo].Retailers where Name=@Name)
                    begin
                        select -1 as ReturnValue;
                        return;
                    end
                end
                else if @Flag=2
                begin
                    if not exists(select * from [ProductManagemnt].[dbo].Retailers where ID=@Id)
                    begin
                        select -1 as ReturnValue;
                        return;
                    end
                end

                select 0 as ReturnValue;
                return;
                ";

                sqlComm.Parameters.AddWithValue("@StoreID", ID);                
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
