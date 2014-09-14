using Microsoft.AspNet.Mvc;

namespace WeddingWebsite.Controllers
{
    public class PhotosController : Controller
    {
        // GET: Photos
        public ActionResult Index()
        {
            return View();
        }
    }
}