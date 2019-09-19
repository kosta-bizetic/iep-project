using iep_project.Models;
using Microsoft.AspNet.SignalR;
using System;
using System.Data.Entity;
using System.Linq;

namespace iep_project.Hubs
{
    public class CommunicationHub : Hub
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public void Hello()
        {
            Clients.All.hello();
        }

        protected bool CurrentUserMemberOfChannel(Guid id)
        {
            CommunicationChannel communicationChannel = db.CommunicationChannels.Where(ch => ch.Id == id).Include(ch => ch.ApplicationUser).First();
            return db.AgentChannels.Any(ac => ac.AgentUserName == Context.User.Identity.Name && ac.ChannelId == id)
                || communicationChannel.ApplicationUser.UserName == Context.User.Identity.Name;
        }

        public void Send(Guid channelId, string message)
        {
            if (message == "") return;

            if (!CurrentUserMemberOfChannel(channelId)) return;

            var user = db.Users.Where(u => u.UserName == Context.User.Identity.Name).First();
            Clients.Group(channelId.ToString()).addNewMessage(user.Name + message);
        }

        public void JoinChannel(Guid channelId)
        {
            if (!CurrentUserMemberOfChannel(channelId)) return;
            Groups.Add(Context.ConnectionId, channelId.ToString());
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}