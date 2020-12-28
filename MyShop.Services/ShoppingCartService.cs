using MyShop.Core.Contracts;
using MyShop.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace MyShop.Services
{
    public class ShoppingCartService
    {
        IRepository<Product> productContext;
        IRepository<ShoppingCart> cartContext;

        //need to add a name for the cookie to identify particualar cookie we want
        public const string CartSessionName = "eCommerceShoppingCart";

        public ShoppingCartService(IRepository<Product> ProductContext, IRepository<ShoppingCart> CartContext)
        {
            this.cartContext = CartContext;
            this.productContext = ProductContext;
        }

        //For reading the shopping th ecart
        //In order to read the cookies we will use HTTP Context
        //we need to read the user cookies from http context looking for the cart id and then attemp to read the cart id from the database
        private ShoppingCart GetCart(HttpContextBase httpContext, bool CreateIfNull)
        {
            HttpCookie cookie = httpContext.Request.Cookies.Get(CartSessionName); //for reading the cookies

            ShoppingCart cart = new ShoppingCart();

            if (cookie != null)
            {
                string cartId = cookie.Value;
                if (!string.IsNullOrEmpty(cartId))
                {
                    cart = cartContext.Find(cartId);
                }
                else
                {
                    if (CreateIfNull)
                    {
                        cart = CreateNewCart(httpContext);
                    }
                }
            }
            else
            {
                if (CreateIfNull)
                {
                    cart = CreateNewCart(httpContext);
                }
            }
            return cart;
        }

        //For creating a new cart
        private ShoppingCart CreateNewCart(HttpContextBase httpContext)
        {
            ShoppingCart cart = new ShoppingCart();
            cartContext.Insert(cart);
            cartContext.Commit();

            HttpCookie cookie = new HttpCookie(CartSessionName);
            cookie.Value = cart.Id;
            cookie.Expires = DateTime.Now.AddDays(1); //cookie expiration
            httpContext.Response.Cookies.Add(cookie);

            return cart;
        }

        //for adding a product to the shopping cart
        public void AddToCart(HttpContextBase httpContext, string productId)
        {
            ShoppingCart cart = GetCart(httpContext, true);
            CartItem item = cart.CartItems.FirstOrDefault(i => i.ProductId == productId);

            if (item == null)
            {
                item = new CartItem()
                {
                    CartId = cart.Id,
                    ProductId = productId,
                    Quantity = 1
                };
                cart.CartItems.Add(item);
            }
            else
            {
                item.Quantity = item.Quantity + 1;
            }
            cartContext.Commit();
        }

        //for removing a product from shopping cart
        public void RemoveFromCart(HttpContextBase httpContext, string itemId)
        {
            ShoppingCart cart = GetCart(httpContext, true);
            CartItem item = cart.CartItems.FirstOrDefault(i => i.Id == itemId);

            if (item != null)
            {
                cart.CartItems.Remove(item);
                cartContext.Commit();
            }
        }
    }
}
