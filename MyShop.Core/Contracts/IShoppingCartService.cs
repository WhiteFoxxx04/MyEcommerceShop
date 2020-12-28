using MyShop.Core.ViewModel;
using System.Collections.Generic;
using System.Web;

namespace MyShop.Core.Contracts
{
    public interface IShoppingCartService
    {
        void AddToCart(HttpContextBase httpContext, string productId);
        List<CartItemViewModel> GetCartItems(HttpContextBase httpContext);
        CartSummaryViewModel GetCartSummary(HttpContextBase httpContext);
        void RemoveFromCart(HttpContextBase httpContext, string itemId);
    }
}