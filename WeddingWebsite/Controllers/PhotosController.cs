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
        private static PicturesModel _model = null;
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

        private static PicturesModel CreatePhotosModel(string webRoot, string photosRoot)
        {
            string[] photosSizeFolders = new string[] { "Small", "Medium", "Large", "Full" };
            string searchFolder = photosSizeFolders.First();
            string photosPath = photosRoot + searchFolder;
            DirectoryInfo photosDirectory = new DirectoryInfo(photosPath);


            var photoFiles = photosDirectory.EnumerateFiles("*.jpg")
                // Randomly, but repeatably, sort the files
                .OrderBy(fi => fi.Name.GetHashCode());

            //var photoPaths = photoFiles.Select(fi => fi.FullName.Replace(absoluteRoot, @"\")).ToArray();

            PicturesModel model = new PicturesModel();
            model.PictureModels = new List<PictureModel>();

            foreach (var photoFile in photoFiles)
            {
                var allSizeFullPaths = photosSizeFolders.Select(size => photoFile.FullName.Replace(searchFolder, size));
                var allSizeMetadatas = allSizeFullPaths.Select(fullPath =>
                {
                    var relativePath = fullPath.Replace(webRoot, @"\");
                    return new PictureMetadata(fullPath, relativePath);
                });

                model.PictureModels.Add(new PictureModel()
                {
                    PictureMetadatas = allSizeMetadatas.ToList()
                });
            }

            return model;
        }
    }
}