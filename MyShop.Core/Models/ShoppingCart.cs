using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyShop.Core.Models
{
    public class ShoppingCart : BaseEntity
    {
        //Virtual keyword because of lazyloading so we can get all the items with the id
        public virtual ICollection<CartItem> CartItems { get; set; }

        public ShoppingCart()
        {
            this.CartItems = new List<CartItem>();
        }
    }
}
