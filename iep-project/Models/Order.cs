using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace iep_project.Models
{
    public class Order
    {
        [DisplayName("Order Id")]
        public Guid Id { get; set; }
        [ScaffoldColumn(false)]
        public string ApplicationUserId { get; set; }
        [Required]
        [ForeignKey("ApplicationUserId")]
        public ApplicationUser ApplicationUser { get; set; }
        [ScaffoldColumn(false)]
        public DateTime Created { get; set; }
        public int Amount { get; set; }
        public double Price { get; set; }
    }
}