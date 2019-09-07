using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using iep_project.Models;

namespace iep_project.Controllers
{
    public class QuestionsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        protected Question getQuestionById(Guid QuestionId)
        {
            return db.Questions.Include(q => q.ApplicationUser).Where(q => q.Id == QuestionId).First();
        }

        // GET: Questions
        public ActionResult Index()
        {
            var questions = db.Questions.Include(q => q.Category).Include(q => q.ApplicationUser);
            return View(questions.ToList());
        }

        // GET: Questions/Details/5
        public ActionResult Details(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Question question = db.Questions.Include(q => q.Category).Include(q => q.ApplicationUser).Where(q => q.Id == id).Single();
            ViewBag.Answers = db.Answers.Where(anwser => anwser.QuestionId == question.Id).Include(a => a.ParentAnswer).Include(a => a.ApplicationUser);
            if (question == null)
            {
                return HttpNotFound();
            }
            return View(question);
        }

        [Authorize(Roles = "User")]
        // GET: Questions/Create
        public ActionResult Create()
        {
            ViewBag.CategoryId = new SelectList(db.Categories, "Id", "CategoryName");
            return View();
        }

        // POST: Questions/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "User")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Title,Text,CategoryId,ImageFile")] Question question)
        {
            question.Created = DateTime.Now;
            question.ApplicationUserId = db.Users.Where(u => u.UserName == User.Identity.Name).First().Id;

            string fileName = "";
            if (question.ImageFile != null)
            {
                fileName = Path.GetFileNameWithoutExtension(question.ImageFile.FileName);
                string extension = Path.GetExtension(question.ImageFile.FileName);
                fileName = fileName + DateTime.Now.ToString("yymmssfff") + extension;
                question.ImagePath = @"~\Images\" + fileName;
                fileName = Path.Combine(Server.MapPath("~/Images/"), fileName);
            }
            
            if (ModelState.IsValid)
            {
                question.Id = Guid.NewGuid();
                db.Questions.Add(question);
                db.SaveChanges();
                if (question.ImageFile != null)
                {
                    question.ImageFile.SaveAs(fileName);
                }
                return RedirectToAction("Index");
            }

            ViewBag.CategoryId = new SelectList(db.Categories, "Id", "CategoryName", question.CategoryId);
            return View(question);
        }

        // GET: Questions/Edit/5
        public ActionResult Edit(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Question question = db.Questions.Find(id);
            if (question == null)
            {
                return HttpNotFound();
            }
            ViewBag.CategoryId = new SelectList(db.Categories, "Id", "CategoryName", question.CategoryId);
            return View(question);
        }

        // POST: Questions/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Title,Text,CategoryId")] Question question)
        {
            if (ModelState.IsValid)
            {
                db.Entry(question).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.CategoryId = new SelectList(db.Categories, "Id", "CategoryName", question.CategoryId);
            return View(question);
        }

        // GET: Questions/Delete/5
        public ActionResult Delete(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Question question = db.Questions.Find(id);
            if (question == null)
            {
                return HttpNotFound();
            }
            return View(question);
        }

        // POST: Questions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(Guid id)
        {
            Question question = db.Questions.Include(q => q.ApplicationUser).Where(q => q.Id == id).First();
            if (User.Identity.Name == question.ApplicationUser.UserName || User.IsInRole("Admin"))
            {
                db.Questions.Remove(question);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            
            return HttpNotFound();
        }

        [Authorize]
        // GET: Questions/Create
        public ActionResult CreateAnswer(Guid QuestionId, Guid? AnswerId = null)
        {
            ViewBag.QuestionId = QuestionId;
            ViewBag.AnswerId = AnswerId;
            return View();
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateAnswer([Bind(Include = "QuestionId,Text,AnswerId")] Answer answer)
        {
            answer.Created = DateTime.Now;
            answer.ApplicationUserId = db.Users.Where(u => u.UserName == User.Identity.Name).First().Id;
            Guid id = Guid.NewGuid();
            
            if (ModelState.IsValid)
            {
                answer.Id = id;
                db.Answers.Add(answer);
                db.SaveChanges();
                return RedirectToAction("Details", new { Id = answer.QuestionId});
            }

            return RedirectToAction("Details", new { Id = answer.QuestionId });
        }

        [Authorize]
        public ActionResult Lock(Guid QuestionId)
        {
            Question question = this.getQuestionById(QuestionId);
            if (User.Identity.Name == question.ApplicationUser.UserName && question.Locked == false)
            {
                question.Locked = true;
                db.SaveChanges();
                return RedirectToAction("Details", new { Id = QuestionId });
            }
            return HttpNotFound();
        }

        [Authorize(Roles ="Agent, Admin")]
        public ActionResult Unlock(Guid QuestionId)
        {
            Question question = this.getQuestionById(QuestionId);
            if (question.Locked == true)
            {
                question.Locked = false;
                db.SaveChanges();
                return RedirectToAction("Details", new { Id = QuestionId });
            }
            return HttpNotFound();
        }

        [Authorize]
        public ActionResult DeleteAnswer(Guid? AnswerId)
        {
            if (AnswerId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Answer answer = db.Answers.Where(a => a.Id == AnswerId).Include(a => a.ApplicationUser).First();
            if (answer == null)
            {
                return HttpNotFound();
            }
            return View(answer);
        }

        [Authorize]
        [HttpPost, ActionName("DeleteAnswer")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteAnswerConfirmed(Guid AnswerId)
        {
            Answer answer = db.Answers.Include(a => a.ApplicationUser).Where(a => a.Id == AnswerId).First();
            if (User.Identity.Name == answer.ApplicationUser.UserName || User.IsInRole("Admin"))
            {
                db.Answers.Remove(answer);
                db.SaveChanges();
                return RedirectToAction("Details", new { Id = answer.QuestionId });
            }
            return HttpNotFound();
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
