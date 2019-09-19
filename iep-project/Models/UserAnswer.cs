using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace iep_project.Models
{
    public class UserAnswer
    {
        [Key, Column(Order = 0)]
        public string UserName { get; set; }
        [Key, Column(Order = 1)]
        public Guid AnswerId { get; set; }
    }
}