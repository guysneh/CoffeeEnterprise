using System.ComponentModel.DataAnnotations;

namespace CoffeeStore.Models
{
    public class Order
    {
        public Guid ID { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 3)]
        public string CustomerName { get; set; } = String.Empty;

        [Required]
        public int Amount { get; set; }

        public ICollection<OrderCoffee>? OrderCoffees { get; set; }
    }
}
