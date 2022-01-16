using System;

namespace CoffeeFactory.Models
{
    /// <summary>
    /// Reperesnts a Coffee.
    /// </summary>
    public class Coffee
    {
        /// <summary>
        /// The ID of the coffee.
        /// </summary>
        public Guid Id { get; }

        /// <summary>
        /// The DateTime on which the Coffee was produced.
        /// </summary>
        public DateTime ProducedAt { get; }

        /// <summary>
        /// The constructor of the class.
        /// </summary>
        public Coffee() 
        {
            Id = Guid.NewGuid();
            ProducedAt = DateTime.Now;
        }
    }
}
