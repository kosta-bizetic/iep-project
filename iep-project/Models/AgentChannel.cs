using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace iep_project.Models
{
    public class AgentChannel
    {
        [Key, Column(Order = 0)]
        public string AgentUserName { get; set; }
        [Key, Column(Order = 1)]
        public Guid ChannelId { get; set; }
    }
}