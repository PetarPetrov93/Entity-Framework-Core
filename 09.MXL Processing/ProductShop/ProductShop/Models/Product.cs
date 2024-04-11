namespace ProductShop.Models
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations.Schema;

    public class Product
    {
        public Product()
        {
            this.CategoryProducts = new List<CategoryProduct>();
        }

        public int Id { get; set; }

        public string Name { get; set; } = null!;

        public decimal Price { get; set; }

        public int SellerId { get; set; }

        [InverseProperty(nameof(User.ProductsSold))]
        public virtual User Seller { get; set; } = null!;

        public int? BuyerId { get; set; }
        [InverseProperty(nameof(User.ProductsBought))]
        public virtual User? Buyer { get; set; }

        public virtual ICollection<CategoryProduct> CategoryProducts { get; set; }
    }
}