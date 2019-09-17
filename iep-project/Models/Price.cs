using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace iep_project.Models
{
    public class Price
    {
        public int Id { get; set; }
        [DisplayName("Cost of opening a communication channel")]
        public int Cost { get; set; }
    }
}