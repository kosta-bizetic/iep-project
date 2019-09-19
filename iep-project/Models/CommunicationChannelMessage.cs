using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace iep_project.Models
{
    public class CommunicationChannelMessage
    {
        public Guid Id { get; set; }

        [ScaffoldColumn(false)]
        public string ApplicationUserId { get; set; }
        [ForeignKey("ApplicationUserId")]
        public ApplicationUser ApplicationUser { get; set; }

        public Guid ChannelId { get; set; }
        [ForeignKey("ChannelId")]
        public CommunicationChannel CommunicationChannel { get; set; }

        public DateTime Created { get; set; }
        public string Message { get; set; }
    }
}