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
        // GET: Photos
        public ActionResult Index()
        {
            string[] photosSizeFolders = new string[] { "Small", "Medium", "Large", "Full" };
            string searchFolder = photosSizeFolders.First();
            var photosPath = Server.MapPath(@"~/Content/EngagementPhotos/" + searchFolder);
            DirectoryInfo photosDirectory = new DirectoryInfo(photosPath);


            var photoFiles = photosDirectory.EnumerateFiles("*.jpg")
                // Randomly, but repeatably, sort the files
                .OrderBy(fi => fi.Name.GetHashCode());

            string absoluteRoot = Server.MapPath("~");
            //var photoPaths = photoFiles.Select(fi => fi.FullName.Replace(absoluteRoot, @"\")).ToArray();

            PicturesModel model = new PicturesModel();
            model.PictureModels = new List<PictureModel>();

            foreach (var photoFile in photoFiles)
            {
                var allSizeFullPaths = photosSizeFolders.Select(size => photoFile.FullName.Replace(searchFolder, size));
                var allSizeMetadatas = allSizeFullPaths.Select(fullPath =>
                {
                    var relativePath = fullPath.Replace(absoluteRoot, @"\");
                    return new PictureMetadata(fullPath, relativePath);
                });

                model.PictureModels.Add(new PictureModel()
                {
                    PictureMetadatas = allSizeMetadatas.ToList()
                });
            }

            return View(model);
        }
    }
}