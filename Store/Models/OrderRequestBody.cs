namespace CoffeeStore.Models
{
    public class OrderRequestBody
    {
        public int Amount { get; set; }
        public string CustomerName { get; set; } = string.Empty;
    }
}
