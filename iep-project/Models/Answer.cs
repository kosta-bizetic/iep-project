using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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

        [ScaffoldColumn(false)]
        public int NumberLikes { get; set; } = 0;

        [ScaffoldColumn(false)]
        public string ApplicationUserId { get; set; }
        [ForeignKey("ApplicationUserId")]
        public ApplicationUser ApplicationUser { get; set; }

        public virtual ICollection<Answer> ChildAnswers { get; set; }
    }
}