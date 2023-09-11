using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Student_manager.Controllers
{
    public class NavController : Controller
    {
        // GET: NavController
        public ActionResult Index()
        {
            return View();
        }

        // GET: NavController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: NavController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: NavController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: NavController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: NavController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: NavController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: NavController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
