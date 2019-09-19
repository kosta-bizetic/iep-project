using iep_project.Models;
using System;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;

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
