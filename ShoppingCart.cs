using System;
using System.Collections.Generic;
using System.Text;

namespace Sabio.Models.Domain
{
    public class ShoppingCart
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public string SpecialRequests { get; set; }
        public decimal ItemCost { get; set; }
        public string VendorName { get; set; }
        public int VendorId { get; set; }
        public List<Files> ProductImages { get; set; }
        public List<CartItems> Items { get; set; }
        public int CreatedBy { get; set; }
        public int ModifiedBy { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateModified { get; set; }
        
    }
}
