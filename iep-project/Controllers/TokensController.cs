using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using iep_project.Models;
using PayPal.Api;
using Order = iep_project.Models.Order;

namespace iep_project.Controllers
{
    [Authorize(Roles ="User")]
    public class TokensController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Tokens
        public ActionResult Index(int result = 0)
        {
            var orders = db.Orders.Include(o => o.ApplicationUser).Where(o => o.ApplicationUser.UserName == User.Identity.Name)
                .OrderByDescending(o => o.Created);
            var user = db.Users.Where(u => u.UserName == User.Identity.Name).First();
            ViewBag.TokenCount = user.NumberOfTokens;
            ViewBag.Offers = db.Offers.ToArray();
            ViewBag.Result = result;
            return View(orders.ToList());
        }

        public ActionResult ExecutePayment()
        {
            var user = db.Users.Where(u => u.UserName == User.Identity.Name).First();
            APIContext apiContext = PayPalConfiguration.ApiContext;
            string payerId = Request.Params["PayerID"];
            var guid = Request.Params["guid"];

            var paymentId = Session[guid] as string;

            if (paymentId != null)
            {
                var paymentExecution = new PaymentExecution() { payer_id = payerId };
                var payment = new Payment() { id = paymentId };

                var executedPayment = payment.Execute(apiContext, paymentExecution);
                Session.Remove(guid);

                int amount = int.Parse(executedPayment.transactions[0].item_list.items[0].quantity);
                double price = amount * double.Parse(executedPayment.transactions[0].item_list.items[0].price);

                Order order = new Order()
                {
                    Id = Guid.NewGuid(),
                    ApplicationUserId = user.Id,
                    Created = DateTime.Now,
                    Amount = amount,
                    Price = price,
                };

                user.NumberOfTokens += amount;

                db.Orders.Add(order);
                db.SaveChanges();
            }
            return RedirectToAction("Index", new { result = 2 });
        }

        public ActionResult Buy(int Id)
        {
            Offer offer = db.Offers.Find(Id);
            APIContext apiContext = PayPalConfiguration.ApiContext;

            if (offer == null)
            {
                return HttpNotFound();
            }

            var payer = new Payer() { payment_method = "paypal" };

            var guid = Convert.ToString((new Random()).Next(100000));
            var redirUrls = new RedirectUrls()
            {
                cancel_url = this.Url.Action("Index", "Tokens", new { result = 1 }, this.Request.Url.Scheme),
                return_url = this.Url.Action("ExecutePayment", "Tokens", new { result = 2, guid = guid }, this.Request.Url.Scheme)
            };

            var itemList = new ItemList()
            {
                items = new List<Item>()
                  {
                    new Item()
                    {
                      name = offer.Name + " Tokens Offer",
                      currency = "USD",
                      price = (offer.Price / offer.Amount).ToString(),
                      quantity = offer.Amount.ToString(),
                      sku = "sku"
                    }
                  }
            };

            var details = new Details()
            {
                tax = "0",
                shipping = "0",
                subtotal = offer.Price.ToString()
            };

            var amount = new Amount()
            {
                currency = "USD",
                total = offer.Price.ToString(), // Total must be equal to sum of shipping, tax and subtotal.
                details = details
            };

            var transactionList = new List<Transaction>();

            transactionList.Add(new Transaction()
            {
                description = "Transaction description.",
                invoice_number = new Random().Next(999999).ToString(),
                amount = amount,
                item_list = itemList
            });

            var payment = new Payment()
            {
                intent = "sale",
                payer = payer,
                redirect_urls = redirUrls,
                transactions = transactionList
            };

            var createdPayment = payment.Create(apiContext);

            Session.Add(guid, createdPayment.id);

            var links = createdPayment.links.GetEnumerator();
            while (links.MoveNext())
            {
                var link = links.Current;
                if (link.rel.ToLower().Trim().Equals("approval_url"))
                {
                    return Redirect(link.href);
                }
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
