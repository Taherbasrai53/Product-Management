namespace Product_Management.Models
{
    public class Response
    {
        public bool success { get; set; }
        public string message { get; set; }
        public Response(bool success, string message)
        {
            this.success = success;
            this.message = message; 
        }
    }
}
