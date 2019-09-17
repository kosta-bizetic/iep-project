using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace iep_project.Models
{
    [Table("Question")]
    public class Question
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        [Required]
        public string Title { get; set; }
        [Required]
        public string Text { get; set; }
        [Required]
        public int CategoryId { get; set; }
        
        public Category Category { get; set; }
        [ScaffoldColumn(false)]
        public string ImagePath { get; set; }
        [NotMapped]
        public HttpPostedFileBase ImageFile { get; set; }
        [Required]
        [ScaffoldColumn(false)]
        public DateTime Created { get; set; }
        [ScaffoldColumn(false)]
        public DateTime? LastLocked { get; set; }
        [Required]
        public Boolean Locked { get; set; } = false;

        [ScaffoldColumn(false)]
        [Display(Name = "Replies")]
        public int NumberOfAnswers { get; set; } = 0;

        [ScaffoldColumn(false)]
        public string ApplicationUserId { get; set; }
        [ForeignKey("ApplicationUserId")]
        public ApplicationUser ApplicationUser { get; set; }

    }
}