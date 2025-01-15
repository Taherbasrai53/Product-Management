namespace Product_Management.Models
{
    public class Retailer
    {
        public int ID { get; set; }
        public string Name { get; set; }        
    }

    public class GetByRetailerRequest
    {
        public int RetailerId { get; set; }
    }
    public class AddRetailerRequest
    {
        public string Name { get; set; }
    }
    public class DeleteRetailerRequest
    {
        public int Id { get; set; }
    }
}
