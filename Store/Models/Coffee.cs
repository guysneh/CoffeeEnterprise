using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CoffeeStore.Models
{
    public class Coffee
    {
        public Guid ID { get; set; }
        public DateTime ProducedAt { get; set; }
        public bool IsDeleted { get; set; }
        public ICollection<OrderCoffee>? OrderCoffees { get; set; }
    }
}
