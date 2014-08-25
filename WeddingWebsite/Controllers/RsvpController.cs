using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using WeddingWebsite.Models;

namespace WeddingWebsite.Controllers
{
    public class RsvpController : Controller
    {
        private WeddingWebsiteContext db = new WeddingWebsiteContext();

        // GET: Rsvps
        public ActionResult Index()
        {
            return View(db.Rsvps.ToList());
        }

        // GET: Rsvps/Details/5
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

        // GET: Rsvps/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Rsvps/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name,Attending,TotalAdults,TotalChildren")] Rsvp rsvp)
        {
            if (ModelState.IsValid)
            {
                db.Rsvps.Add(rsvp);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(rsvp);
        }

        // GET: Rsvps/Edit/5
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

        // POST: Rsvps/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,Attending,TotalAdults,TotalChildren")] Rsvp rsvp)
        {
            if (ModelState.IsValid)
            {
                db.Entry(rsvp).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(rsvp);
        }

        // GET: Rsvps/Delete/5
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

        // POST: Rsvps/Delete/5
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
