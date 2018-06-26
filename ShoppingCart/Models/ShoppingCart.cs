using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ShoppingCart.Models
{
    public class ShoppingCart
    {
        private readonly ApplicationDbContext _shoppingCartDb = new ApplicationDbContext();
        string ShoppingCartId { get; set; }
        public const string CartSessionKey = "CartId";

        public static ShoppingCart GetCart(HttpContextBase context)
        {
            var cart = new ShoppingCart();
            cart.ShoppingCartId = cart.GetCartId(context);
            return cart;
        }

        // Helper method to simplify shopping cart calls
        public static ShoppingCart GetCart(Controller controller)
        {
            return GetCart(controller.HttpContext);
        }

        // We're using HttpContextBase to allow access to cookies.
        public string GetCartId(HttpContextBase context)
        {
            if (context.Session[CartSessionKey] == null)
            {
                if (!string.IsNullOrWhiteSpace(context.User.Identity.Name))
                {
                    context.Session[CartSessionKey] =
                        context.User.Identity.Name;
                }
                else
                {
                    // Generate a new random GUID using System.Guid class
                    Guid tempCartId = Guid.NewGuid();
                    // Send tempCartId back to client as a cookie
                    context.Session[CartSessionKey] = tempCartId.ToString();
                }
            }

            return context.Session[CartSessionKey].ToString();
        }

        public void AddToCart(Product product)
        {
            // Get the matching cart and product instances
            var cartItem = _shoppingCartDb.Carts.SingleOrDefault(
                c => c.CartId == ShoppingCartId
                     && c.ProductId == product.ProductId);

            if (cartItem == null)
            {
                // Create a new cart item if no cart item exists
                cartItem = new Cart
                {
                    ProductId = product.ProductId,
                    CartId = ShoppingCartId,
                    Quantity = 1,
                    DateCreated = DateTime.Now
                };
                _shoppingCartDb.Carts.Add(cartItem);
            }
            else
            {
                // If the item does exist in the cart, 
                // then add one to the quantity
                if (product.Stock > cartItem.Quantity)
                    cartItem.Quantity++;
            }

            // Save changes
            _shoppingCartDb.SaveChanges();
        }

        public int RemoveFromCart(int id)
        {
            // Get the cart
            var cartItem = _shoppingCartDb.Carts.Single(
                cart => cart.CartId == ShoppingCartId
                        && cart.RecordId == id);

            int itemCount = 0;

            if (cartItem != null)
            {
                if (cartItem.Quantity > 1)
                {
                    cartItem.Quantity--;
                    itemCount = cartItem.Quantity;
                }
                else
                {
                    _shoppingCartDb.Carts.Remove(cartItem);
                }

                // Save changes
                _shoppingCartDb.SaveChanges();
            }

            return itemCount;
        }

        public void EmptyCart()
        {
            var cartItems = _shoppingCartDb.Carts.Where(
                cart => cart.CartId == ShoppingCartId);

            foreach (var cartItem in cartItems)
            {
                _shoppingCartDb.Carts.Remove(cartItem);
            }

            // Save changes
            _shoppingCartDb.SaveChanges();
        }

        public List<Cart> GetCartItems()
        {
            return _shoppingCartDb.Carts.Where(
                cart => cart.CartId == ShoppingCartId).ToList();
        }

        public int GetCount()
        {
            // Get the count of each item in the cart and sum them up
            int? count = GetCartItems().Sum(x => x.Quantity);
            // Return 0 if all entries are null
            return count ?? 0;
        }

        public float GetTotal()
        {
            float? total = GetCartItems().Sum(x => x.Quantity * x.Product.Price);
            return total ?? 0;
        }

        public int CreateOrder(Order order)
        {
            float orderTotal = 0;

            var cartItems = GetCartItems();
            // Iterate over the items in the cart, 
            // adding the order details for each
            foreach (var item in cartItems)
            {
                var orderDetail = new OrderDetail
                {
                    ProductId = item.ProductId,
                    OrderId = order.OrderId,
                    UnitPrice = item.Product.Price,
                    Quantity = item.Quantity
                };
                // Set the order total of the shopping cart
                orderTotal += (item.Quantity * item.Product.Price);

                //Reduce stock and update orderable
                item.Product.Stock = item.Product.Stock > 1 ? item.Product.Stock -= item.Quantity : 0;
                item.Product.IsOrderable = item.Product.Stock > 1;

                _shoppingCartDb.OrderDetails.Add(orderDetail);
            }

            // Set the order's total to the orderTotal count
            order.Total = orderTotal;

            _shoppingCartDb.Entry(order).State = EntityState.Modified;
            // Save the order
            _shoppingCartDb.SaveChanges();
            // Empty the shopping cart
            EmptyCart();
            // Return the OrderId as the confirmation number
            return order.OrderId;
        }

        // When a user has logged in, migrate their shopping cart to
        // be associated with their username
        public void MigrateCart(string userName)
        {
            var shoppingCart = _shoppingCartDb.Carts.Where(
                c => c.CartId == ShoppingCartId);

            foreach (Cart item in shoppingCart)
            {
                item.CartId = userName;
            }
            _shoppingCartDb.SaveChanges();
        }
    }
}