using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fawry_ECommerce_Project.Models
{
    public class CartDetail
    {
        [Key]
        public int Id { get; set; }
        [ForeignKey("Cart")]
        public int Cart_Id { get; set; }
        public Cart? Cart { get; set; }

        [ForeignKey("Product")]
        public int Product_Id { get; set; }
        public Product? Product { get; set; }

        public int Quantity{ get; set; }
        public double Price { get; set; }

    }
}
