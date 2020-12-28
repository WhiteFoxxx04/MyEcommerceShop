using MyShop.Core.Contracts;
using MyShop.Core.Models;
using MyShop.Core.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace MyShop.Services
{
    public class ShoppingCartService :  IShoppingCartService
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

        //For getting the product from the shopping cart
        public List<CartItemViewModel> GetCartItems(HttpContextBase httpContext)
        {
            ShoppingCart cart = GetCart(httpContext, false);

            if (cart != null)
            {
                //querying using linq to get the items from both the table using join shopping cart table with product table
                var results = (from c in cart.CartItems
                               join p in productContext.Collection() on c.ProductId equals p.Id
                               select new CartItemViewModel()
                               {
                                   Id = c.Id,
                                   Quantity = c.Quantity,
                                   ProductName = p.Name,
                                   Image = p.Image,
                                   Price = p.Price
                               }).ToList();
                return results;
            }
            else
            {
                return new List<CartItemViewModel>();
            }
        }

        public CartSummaryViewModel GetCartSummary(HttpContextBase httpContext)
        {
            ShoppingCart cart = GetCart(httpContext, false);
            CartSummaryViewModel model = new CartSummaryViewModel(0, 0);
            if (cart != null)
            {
                int? cartCount = (from item in cart.CartItems
                                  select item.Quantity).Sum();

                decimal? cartTotal = (from item in cart.CartItems
                                      join p in productContext.Collection() on item.ProductId equals p.Id
                                      select item.Quantity * p.Price).Sum();

                model.CartCount = cartCount ?? 0;
                model.CartTotal = cartTotal ?? decimal.Zero;

                return model;
            }
            else
            {
                return model;
            }
        }
    }
}
