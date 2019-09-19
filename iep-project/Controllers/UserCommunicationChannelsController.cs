using iep_project.Hubs;
using iep_project.Models;
using Microsoft.AspNet.SignalR;
using System;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using AuthorizeAttribute = System.Web.Mvc.AuthorizeAttribute;

namespace iep_project.Controllers
{
    [Authorize(Roles = "User")]
    public class UserCommunicationChannelsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: UserCommunicationChannels
        public ActionResult Index(string Message = "")
        {
            var communicationChannels = db.CommunicationChannels.Include(c => c.ApplicationUser)
                .Where(ch => ch.ApplicationUser.UserName == User.Identity.Name);
            ViewBag.Price = db.Prices.First().Cost;
            ViewBag.Message = Message;
            return View(communicationChannels.ToList());
        }

        // GET: UserCommunicationChannels/Details/5
        public ActionResult Details(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CommunicationChannel communicationChannel = db.CommunicationChannels.Where(ch => ch.Id == id).Include(ch => ch.ApplicationUser).First();
            if (communicationChannel.ApplicationUser.UserName != User.Identity.Name)
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

        // GET: UserCommunicationChannels/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: UserCommunicationChannels/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Title")] CommunicationChannel communicationChannel)
        {
            var user = db.Users.Where(u => u.UserName == User.Identity.Name).First();
            communicationChannel.ApplicationUserId = user.Id;
            communicationChannel.Created = DateTime.Now;

            int cost = db.Prices.First().Cost;

            if (user.NumberOfTokens < cost)
            {
                return RedirectToAction("Index", new { Message = "You do not have enough tokens to create a Communication Channel!" });
            }

            if (ModelState.IsValid)
            {
                communicationChannel.Id = Guid.NewGuid();
                user.NumberOfTokens -= cost;
                db.CommunicationChannels.Add(communicationChannel);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(communicationChannel);
        }

        public ActionResult Close(Guid id)
        {
            var communicationChannel = db.CommunicationChannels.Find(id);
            if (communicationChannel == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var user = db.Users.Where(u => u.UserName == User.Identity.Name).First();
            if (communicationChannel.ApplicationUserId != user.Id)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            communicationChannel.Open = false;
            communicationChannel.NumberOfAgents = 0;
            var AgentChannels = db.AgentChannels.Where(ac => ac.ChannelId == id);
            db.AgentChannels.RemoveRange(AgentChannels);
            db.SaveChanges();

            var context = GlobalHost.ConnectionManager.GetHubContext<CommunicationHub>();
            var commHub = new CommunicationHub();

            context.Clients.Group(id.ToString()).addNewMessage("This channel closed.");

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
