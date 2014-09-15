using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Web;
using Microsoft.AspNet.Mvc;
using Microsoft.AspNet.Identity;
using System.Security.Principal;
using WeddingWebsite.Models;

namespace WeddingWebsite.Controllers
{
    [Authorize] // Don't allow anonymous access to anything but create
    public class RsvpController : Controller
    {
        public RsvpController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }

        public UserManager<ApplicationUser> UserManager { get; private set; }
        public SignInManager<ApplicationUser> SignInManager { get; private set; }

        private WeddingWebsiteContext _db = null;
        public WeddingWebsiteContext Db
        {
            get
            {
                if (_db == null)
                {
                    _db = new WeddingWebsiteContext();
                }
                return _db;
            }
        }

        [AllowAnonymous]
        public ActionResult Index()
        {
            if (User.Identity.IsAuthenticated)
            {
                List<Rsvp> rsvps;
                if (User.IsInRole("admin"))
                {
                    rsvps = Db.Rsvps.ToList();
                }
                else
                {
                    string userId = User.Identity.GetUserId();
                    rsvps = Db.Rsvps.Where(r => r.UserId == userId).ToList();
                }
                return View(rsvps);
            }

            return RedirectToAction("Create");
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
        public async ActionResult Create(Rsvp rsvp)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    ApplicationUser user = null;
                    if (User.Identity.IsAuthenticated)
                    {
                        rsvp.UserId = User.Identity.GetUserId();
                    }
                    else
                    {
                        // Check if this account exists already
                        string userName = rsvp.Email;
                        string email = rsvp.Email;
                        if (string.IsNullOrWhiteSpace(userName))
                        {
                            userName = rsvp.Name.Replace(" ", "");
                            email = userName + "@DontHaveEmail.com";
                        }
                        user = await UserManager.FindByNameAsync(userName);
                        //user = UserManager.FindByName(userName);
                        if (user == null)
                        {
                            // Create a dummy user for this rsvp
                            ApplicationUser newUser = new ApplicationUser()
                            {
                                Email = email,
                                UserName = userName,
                                Name = rsvp.Name,
                            };
                            IdentityResult result = await UserManager.CreateAsync(newUser, email);
                            //IdentityResult result = UserManager.Create(newUser, email);
                            if (result.Succeeded)
                            {
                                await UserManager.AddToRoleAsync(newUser, "user");
                                //UserManager.AddToRole(newUser.Id, "user");
                                user = newUser;
                            }
                        }

                        if (user != null)
                        {
                            // If not an admin, sign them into the account
                            //var signInManager = Context.GetOwinContext().Get<ApplicationSignInManager>();
                            //signInManager.SignIn(user, isPersistent: true, rememberBrowser: false);
                            await SignInManager.SignInAsync(user, isPersistent: true);

                            // Assign RSVP.UserId
                            // Save the user id to the rsvp
                            rsvp.UserId = user.Id;
                        }
                    }
                }
                catch (Exception)
                { }

                // Save the rsvp
                Db.Rsvps.Add(rsvp);
                Db.SaveChanges();

                return RedirectToAction("Index");
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
                    return new HttpStatusCodeResult((int)HttpStatusCode.BadRequest);
                }

                rsvpFromDb.Name = rsvp.Name;
                rsvpFromDb.Attending = rsvp.Attending;
                rsvpFromDb.TotalAdults = rsvp.TotalAdults;
                rsvpFromDb.TotalChildren = rsvp.TotalChildren;

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
