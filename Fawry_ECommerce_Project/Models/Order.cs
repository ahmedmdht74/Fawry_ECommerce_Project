using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fawry_ECommerce_Project.Models
{
    public class Order
    {
        [Key]
        public int Id { get; set; }
        [ForeignKey("ApplicationUser")]
        public string User_Id { get; set; }
        public ApplicationUser? ApplicationUser { get; set; }
        public string Status { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public DateTime? ShippingDate { get; set; }
    }
}
