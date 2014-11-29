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
            var photosPath = Server.MapPath(@"~/Content/EngagementPhotos");
            DirectoryInfo photosDirectory = new DirectoryInfo(photosPath);

            var photoFiles = photosDirectory.EnumerateFiles("*.jpg")
                // Randomly, but repeatably, sort the files
                .OrderBy(fi => fi.Name.GetHashCode());

            // Convert to a virtual path
            string absoluteRoot = Server.MapPath("~");
            var photoPaths = photoFiles.Select(fi => fi.FullName.Replace(absoluteRoot, "")).ToArray();

            return View(photoPaths);
        }
    }
}