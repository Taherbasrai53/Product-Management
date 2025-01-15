namespace Product_Management.Models
{
    public class Product
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public double Price { get; set; }
        public int Quantity { get; set; }
        public int StoreId { get; set; }
        public EnumPerformance Performance { get; set; }

        public enum EnumPerformance
        {
            Low=0,
            Medium=1,
            High=2,
        }
    }

    public class DeleteProductRequest
    {
        public int ID { get; set; }
        public int StoreId { get; set; }
    }
}
