using CoffeeStore.Data;
using CoffeeStore.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CoffeeStore.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class StoreController : ControllerBase
    {
        /// <summary>
        /// The injected CoffeeStore DbContext.
        /// </summary>
        private readonly CoffeeStoreContext coffeeStoreContext;

        /// <summary>
        /// The class contructor. 
        /// </summary>
        /// <param name="coffeeStoreContext">The CoffeeStore DbContext injected service.</param>
        public StoreController(CoffeeStoreContext coffeeStoreContext)
        {
            this.coffeeStoreContext = coffeeStoreContext;
        }

        /// <summary>
        /// Gets the count of the available coffees in the store.
        /// </summary>
        /// <returns>A number represents the amount of available coffees in the store.</returns>
        [HttpGet]
        [Route("Coffees/Count")]
        public int Count()
        {
            return coffeeStoreContext.Coffees.Where(coffee => !coffee.IsDeleted).Count();
        }

        /// <summary>
        /// Gets all orders that made in the store.
        /// </summary>
        /// <param name="customerName">(optional)Filter by customer name. If given, will get only the orders that belong to the given customer name.</param>
        /// <param name="page">The page number.</param>
        /// <param name="size">The page size. How many orders should appear in one page.</param>
        /// <returns>An anonymous object contains the query parameters, the orders array and its count.</returns>
        [HttpGet]
        [Route("Orders")]
        public dynamic GetOrders([FromQuery]string? customerName, int page=0, int size=20)
        {
            if (page<0) 
            {
                page = 0;
            }
            if (size<0) 
            {
                size = 20;
            }

            var query = coffeeStoreContext.Orders;
            if (!string.IsNullOrWhiteSpace(customerName)) 
            {
                query.Where(order => order.CustomerName.Equals(customerName));
            }
            var orders = query.Skip(page * size).Take(size).ToArray();
           
            return new
            {
                page,
                size,
                count = orders.Length,
                orders 
            }; 
        }

        /// <summary>
        /// Get one order by Id.
        /// </summary>
        /// <param name="id">The id of the order to get.</param>
        /// <returns>An anonymous object(if order was found) contain all information of the order and its coffee ids.</returns>
        [HttpGet]
        [Route("Order/{id}")]
        public ActionResult<dynamic> GetOrder([FromRoute] Guid id)
        {
            var result = coffeeStoreContext.Orders.Where(order => order.ID == id).FirstOrDefault();
            if (result == null)
            {
                return NotFound();
            }
            var coffeeIds = coffeeStoreContext.OrderCoffees.Where(oc => oc.OrderID == id).Select(oc => oc.CoffeeID).ToArray();
            var response = new
            {
                result.ID,
                result.CustomerName,
                result.Amount,
                coffeeIds
            };
            return response;
        }

        /// <summary>
        /// Gets all available coffees in the store.
        /// </summary>
        /// <param name="page">The page number.</param>
        /// <param name="size">The page size. How many coffees should appear in one page.</param>
        /// <returns>A Coffee Array.</returns>
        [HttpGet]
        [Route("Coffees")]
        public Coffee[] GetCoffees(int page = 0, int size = 20)
        {
            if (page < 0)
            {
                page = 0;
            }
            if (size < 0)
            {
                size = 20;
            }

            return coffeeStoreContext.Coffees.Where(coffee => !coffee.IsDeleted).Skip(page * size).Take(size).ToArray();
        }

        /// <summary>
        /// Get one coffee by Id.
        /// </summary>
        /// <param name="id">The id of the coffee to get.</param>
        /// <returns>A Coffee object (if found).</returns>
        [HttpGet]
        [Route("Coffee/{id}")]
        public ActionResult<Coffee> GetCoffee([FromRoute]Guid id)
        {
            var result = coffeeStoreContext.Coffees.Where(coffee => coffee.ID == id).FirstOrDefault();
            if (result == null) 
            {
                return NotFound();
            }
            return result;
        }

        /// <summary>
        /// Create new coffee order according to the body inforamtion of the request.
        /// </summary>
        /// <param name="orderRequestBody">The request body (should be a json object that can be deserialized to an OrderRequestBody object).</param>
        /// <returns>The order object if successfully created.</returns>
        [HttpPost]
        [Route("Buy")]
        public ActionResult<Order> Buy([FromBody] OrderRequestBody orderRequestBody)
        {
            var count = Count();
            if (count < orderRequestBody.Amount)
            {
                return Conflict($"The amount in stock is {count} and you ask for {orderRequestBody.Amount}.");
            }

            using var transaction = coffeeStoreContext.Database.BeginTransaction();

            try
            {
                var coffees = coffeeStoreContext.Coffees.Where(coffee => !coffee.IsDeleted).Take(orderRequestBody.Amount);
                var order = new Order
                {
                    ID = Guid.NewGuid(),
                    Amount = orderRequestBody.Amount
                };
                coffeeStoreContext.Orders.Add(order);

                foreach (var coffee in coffees)
                {
                    coffee.IsDeleted = true;
                    var oc = new OrderCoffee() { Coffee = coffee, Order = order };
                    coffeeStoreContext.OrderCoffees.Add(oc);
                }

                coffeeStoreContext.SaveChanges();
                transaction.Commit();
                var result = new
                {
                    order.ID,
                    amount = order.Amount,
                    amountInStockAfterPurchase = count - order.Amount,
                    coffees = coffees.Select(coffee => coffee.ID.ToString()).ToArray()
                };
                return new CreatedResult("coffee store", result);
            }
            catch (Exception)
            {
                return Conflict($"The amount in stock is {Count()} and you ask for {orderRequestBody.Amount}");
            }
        }
    }
}