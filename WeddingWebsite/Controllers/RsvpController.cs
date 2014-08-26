using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.AspNet.Identity;
using WeddingWebsite.Models;

namespace WeddingWebsite.Controllers
{
    public class RsvpController : Controller
    {
        private WeddingWebsiteContext db = new WeddingWebsiteContext();

        // GET: Rsvp
        [Authorize(Roles = "admin")]
        public ActionResult Index()
        {
            return View(db.Rsvps.ToList());
        }

        // GET: Rsvp/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Rsvp rsvp = db.Rsvps.Find(id);
            if (rsvp == null)
            {
                return HttpNotFound();
            }
            return View(rsvp);
        }

        // GET: Rsvp/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Rsvp/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Rsvp rsvp)
        {
            if (ModelState.IsValid)
            {
                // Save the inital state of the rsvp
                db.Rsvps.Add(rsvp);
                db.SaveChanges();

                // Create a dummy user for this rsvp
                string userName = rsvp.Email;
                if (string.IsNullOrWhiteSpace(userName))
                {
                    userName = rsvp.Name.Replace(" ", "");
                }

                var userManager = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
                var user = new ApplicationUser()
                {
                    Email = rsvp.Email,
                    UserName = userName,
                };
                userManager.Create(user);
                userManager.AddToRole(user.Id, "user");

                // Save the user id to the rsvp
                rsvp.UserId = user.Id;

                db.Entry(rsvp).State = EntityState.Modified;
                db.SaveChanges();

                // If not an admin, sign them into the new account
                if (!User.IsInRole("admin"))
                {
                    var signInManager = HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
                    signInManager.SignIn(user, isPersistent: true, rememberBrowser: false);
                }

                return RedirectToAction("Index");
            }

            return View(rsvp);
        }

        // GET: Rsvp/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Rsvp rsvp = db.Rsvps.Find(id);
            if (rsvp == null)
            {
                return HttpNotFound();
            }
            return View(rsvp);
        }

        // POST: Rsvp/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Rsvp rsvp)
        {
            if (ModelState.IsValid)
            {
                db.Entry(rsvp).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(rsvp);
        }

        // GET: Rsvp/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Rsvp rsvp = db.Rsvps.Find(id);
            if (rsvp == null)
            {
                return HttpNotFound();
            }
            return View(rsvp);
        }

        // POST: Rsvp/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Rsvp rsvp = db.Rsvps.Find(id);
            db.Rsvps.Remove(rsvp);
            db.SaveChanges();
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
