using static Product_Management.Models.Request;

namespace Product_Management.Models
{
    public class Request
    {
        public int ID { get; set; }
        public int ProductID { get; set; }
        public int RetailerID { get; set; }
        public int StoreID { get; set; }
        public int Quantity { get; set; }
        public EStatus status { get; set; } = EStatus.Pending;

        public enum EStatus
        {
            Pending=1,
            Completed=2
        }            
    }
    public class RequestDto
    {        
        public int ProductID { get; set; }        
        public int StoreID { get; set; }
        public int Quantity { get; set; }
        public EStatus status { get; set; } = EStatus.Pending;
                 
    }
}
