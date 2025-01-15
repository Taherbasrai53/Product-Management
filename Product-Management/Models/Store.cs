namespace Product_Management.Models
{
    public class Store
    {
        public int ID { get; set; }
        public int RetailerID { get; set; }
        public string Name { get; set; }
        public string Franchisee { get; set; }
    }
    public class AddStoreModel
    {
        public int RetailerID { get; set; }
        public string Name { get; set; }
        public string Franchisee { get; set; }
    }
    public class DeleteStoreModel
    {
        public int ID { get; set; }
    }
}
