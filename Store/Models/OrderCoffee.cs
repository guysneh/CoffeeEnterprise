namespace CoffeeStore.Models
{
    public class OrderCoffee
    {
        public Guid OrderID { get; set; }
        public Order? Order { get; set; }

        public Guid CoffeeID { get; set; }
        public Coffee? Coffee { get; set; }
    }
}
