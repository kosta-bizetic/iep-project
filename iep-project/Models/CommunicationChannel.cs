using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace iep_project.Models
{
    public class CommunicationChannel
    {
        public Guid Id { get; set; }
        [Required]
        public string Title { get; set; }
        [Required]
        [ScaffoldColumn(false)]
        public DateTime Created { get; set; }

        [ScaffoldColumn(false)]
        public string ApplicationUserId { get; set; }
        [ForeignKey("ApplicationUserId")]
        public ApplicationUser ApplicationUser { get; set; }

        [ScaffoldColumn(false)]
        [Display(Name = "Agents")]
        public int NumberOfAgents { get; set; } = 0;

        [ScaffoldColumn(false)]
        [Required]
        public bool Open { get; set; } = true;
    }
}