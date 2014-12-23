using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WeddingWebsite.Controllers
{
    public class PhotosController : Controller
    {
        // GET: Photos
        public ActionResult Index()
        {
            var photosPath = Server.MapPath(@"~/Content/EngagementPhotos/Full");
            DirectoryInfo photosDirectory = new DirectoryInfo(photosPath);

            var photoFiles = photosDirectory.EnumerateFiles("*.jpg")
                // Randomly, but repeatably, sort the files
                .OrderBy(fi => fi.Name.GetHashCode());

            var photoPaths = photoFiles.Select(fi => fi.Name);

            return View(photoPaths);
        }
    }
}