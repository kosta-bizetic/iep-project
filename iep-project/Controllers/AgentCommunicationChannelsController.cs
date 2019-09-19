using iep_project.Models;
using System;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace iep_project.Controllers
{
    [Authorize(Roles = "Agent")]
    public class AgentCommunicationChannelsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: UserCommunicationChannels
        public ActionResult Index(string Message = "")
        {
            var communicationChannels = db.CommunicationChannels.Include(c => c.ApplicationUser)
                .Where(ch => ch.Open == true);
            var listOfCommunicationChannels = communicationChannels.ToList();
            var channelMember = new bool[listOfCommunicationChannels.Count];
            for (int i = 0; i < listOfCommunicationChannels.Count; i++)
            {
                var channel = listOfCommunicationChannels.ElementAt(i);
                channelMember[i] = db.AgentChannels.Any(ac => ac.AgentUserName == User.Identity.Name && ac.ChannelId == channel.Id);
            }
            ViewBag.ChannelMember = channelMember;
            return View(listOfCommunicationChannels);
        }

        protected bool CurrentUserMemberOfChannel(Guid id)
        {
            return db.AgentChannels.Any(ac => ac.AgentUserName == User.Identity.Name && ac.ChannelId == id);
        }

        // GET: UserCommunicationChannels/Details/5
        public ActionResult Details(Guid id)
        {
            CommunicationChannel communicationChannel = db.CommunicationChannels.Where(ch => ch.Id == id).Include(ch => ch.ApplicationUser).First();
            if (!CurrentUserMemberOfChannel(id))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            if (communicationChannel == null)
            {
                return HttpNotFound();
            }
            ViewBag.Messages = db.CommunicationChannelMessages.Where(ccm => ccm.ChannelId == id)
                .Include(ccm => ccm.ApplicationUser).OrderBy(ccm => ccm.Created).ToArray();
            return View(communicationChannel);
        }

        public ActionResult Join(Guid id)
        {
            CommunicationChannel communicationChannel = db.CommunicationChannels.Where(ch => ch.Id == id).Include(ch => ch.ApplicationUser).First();
            var agentChannel = new AgentChannel()
            {
                AgentUserName = User.Identity.Name,
                ChannelId = communicationChannel.Id
            };
            db.AgentChannels.Add(agentChannel);
            communicationChannel.NumberOfAgents++;
            db.SaveChanges();

            return RedirectToAction("Details", new { id = communicationChannel.Id });
        }

        public ActionResult Leave(Guid id)
        {
            if (CurrentUserMemberOfChannel(id))
            {
                var agentChannel = db.AgentChannels.Where(ac => ac.AgentUserName == User.Identity.Name && ac.ChannelId == id).First();
                CommunicationChannel communicationChannel = db.CommunicationChannels.Where(ch => ch.Id == id).Include(ch => ch.ApplicationUser).First();
                communicationChannel.NumberOfAgents--;
                db.AgentChannels.Remove(agentChannel);
                db.SaveChanges();
            }

            return RedirectToAction("Index");
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
