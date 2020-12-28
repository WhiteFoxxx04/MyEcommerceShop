using MyShop.Core.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MyShop.UI.Controllers
{
    public class ShoppingCartController : Controller
    {
        IShoppingCartService ShoppingCartService;

        public ShoppingCartController(IShoppingCartService shoppingCartService)
        {
            this.ShoppingCartService = shoppingCartService;
        }


        // GET: ShoppingCart
        public ActionResult Index()
        {
            var model = ShoppingCartService.GetCartItems(HttpContext);
            return View(model);
        }

        public ActionResult AddToCart(string Id)
        {
            ShoppingCartService.AddToCart(HttpContext, Id);
            return RedirectToAction("Index");
        }

        public ActionResult RemoveFromCart(string Id)
        {
            ShoppingCartService.RemoveFromCart(HttpContext, Id);
            return RedirectToAction("Index");
        }

        //Partial view for shopping cart summary
        public PartialViewResult ShoppingCartSummary()
        {
            var cartSummary = ShoppingCartService.GetCartSummary(HttpContext);
            return PartialView(cartSummary);
        }
    }
}