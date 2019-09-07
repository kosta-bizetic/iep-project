using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace iep_project.Models
{
    [Table("Answer")]
    public class Answer
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        [Required]
        public string Text { get; set; }

        [Required]
        public Guid QuestionId { get; set; }
        public Question Question { get; set; }

        public Guid? AnswerId { get; set; }
        [ForeignKey("AnswerId")]
        public Answer ParentAnswer { get; set; }

        [ScaffoldColumn(false)]
        public DateTime Created { get; set; }

        [NotMapped]
        [ScaffoldColumn(false)]
        public int NumberLikes { get; set; }

        [NotMapped]
        [ScaffoldColumn(false)]
        public int NumberDislikes { get; set; }

        [ScaffoldColumn(false)]
        public string ApplicationUserId { get; set; }
        [ForeignKey("ApplicationUserId")]
        public ApplicationUser ApplicationUser { get; set; }

        public virtual ICollection<Answer> ChildAnswers { get; set; }
    }
}