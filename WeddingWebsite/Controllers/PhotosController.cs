using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WeddingWebsite.Models;

namespace WeddingWebsite.Controllers
{
    public class PhotosController : Controller
    {
        private static string[] _model = null;
        private static object _picturesModelLock = new object();
        // GET: Photos
        public ActionResult Index()
        {
            if (_model == null)
            {
                lock (_picturesModelLock)
                {
                    if (_model == null)
                    {
                        string webRoot = Server.MapPath("~");
                        string photosRoot = Server.MapPath(@"~/Content/EngagementPhotos/");

                        _model = CreatePhotosModel(webRoot, photosRoot);
                    }
                }
            }

            return View(_model);
        }

        private static string[] CreatePhotosModel(string webRoot, string photosRoot)
        {
            string[] photosSizeFolders = new string[] { "Small", "Medium", "Large", "Full" };
            string searchFolder = photosSizeFolders.First();
            string photosPath = photosRoot + searchFolder;
            DirectoryInfo photosDirectory = new DirectoryInfo(photosPath);


            var photoFiles = photosDirectory.EnumerateFiles("*.jpg")
                // Randomly, but repeatably, sort the files
                .OrderBy(fi => fi.Name.GetHashCode());

            return photoFiles.Select(fi => fi.Name).ToArray();
        }
    }
}