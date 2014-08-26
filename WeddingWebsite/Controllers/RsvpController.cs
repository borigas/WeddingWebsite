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
    [Authorize] // Don't allow anonymous access to anything but create
    public class RsvpController : BaseController
    {
        [Authorize(Roles = "admin")]
        public ActionResult Index()
        {
            return View(Db.Rsvps.ToList());
        }

        // GET: Rsvp/Details/5
        public ActionResult Details(int? id)
        {
            Rsvp rsvp = FindRsvpAndCheckPermissions(id);
            return View(rsvp);
        }

        [AllowAnonymous]
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
        [AllowAnonymous]
        public ActionResult Create(Rsvp rsvp)
        {
            if (ModelState.IsValid)
            {
                // Save the inital state of the rsvp
                Db.Rsvps.Add(rsvp);
                Db.SaveChanges();

                // Create a dummy user for this rsvp
                string userName = rsvp.Email;
                if (string.IsNullOrWhiteSpace(userName))
                {
                    userName = rsvp.Name.Replace(" ", "");
                }

                var user = new ApplicationUser()
                {
                    Email = rsvp.Email,
                    UserName = userName,
                };
                UserManager.Create(user);
                UserManager.AddToRole(user.Id, "user");

                // Save the user id to the rsvp
                rsvp.UserId = user.Id;

                Db.Entry(rsvp).State = EntityState.Modified;
                Db.SaveChanges();

                // If not an admin, sign them into the new account
                if (!User.IsInRole("admin"))
                {
                    var signInManager = HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
                    signInManager.SignIn(user, isPersistent: true, rememberBrowser: false);
                }

                return RedirectToAction("Detail", new { id = rsvp.Id });
            }

            return View(rsvp);
        }

        // GET: Rsvp/Edit/5
        public ActionResult Edit(int? id)
        {
            Rsvp rsvp = FindRsvpAndCheckPermissions(id);
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
                Rsvp rsvpFromDb = FindRsvpAndCheckPermissions(rsvp.Id);
                // The user id was tampered with
                if (rsvp.UserId != rsvpFromDb.UserId)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                Db.Entry(rsvp).State = EntityState.Modified;
                Db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(rsvp);
        }

        // GET: Rsvp/Delete/5
        public ActionResult Delete(int? id)
        {
            Rsvp rsvp = FindRsvpAndCheckPermissions(id);

            return View(rsvp);
        }

        // POST: Rsvp/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Rsvp rsvp = FindRsvpAndCheckPermissions(id);

            Db.Rsvps.Remove(rsvp);
            Db.SaveChanges();
            return RedirectToAction("Index");
        }

        private Rsvp FindRsvpAndCheckPermissions(int? id)
        {
            if (id == null)
            {
                throw new HttpException((int)HttpStatusCode.BadRequest, "Bad Request");
            }
            Rsvp rsvp = Db.Rsvps.Find(id);
            if (rsvp == null)
            {
                throw new HttpException((int)HttpStatusCode.NotFound, "Not Found");
            }

            CheckRsvpPermissions(rsvp);

            return rsvp;
        }

        private void CheckRsvpPermissions(Rsvp rsvp)
        {
            if (!User.IsInRole("admin") && User.Identity.GetUserId() != rsvp.UserId)
            {
                // This user doesn't have permission to see this RSVP
                throw new HttpException((int)HttpStatusCode.Unauthorized, "Unauthorized");
            }
        }
    }
}
