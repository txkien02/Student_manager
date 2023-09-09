using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using Web_Student_manager.Filters;


namespace Web_Student_manager.Controllers
{
    [AuthorFilter("User")]
    public class UserController : Controller
    {
        // GET: UserController
        public ActionResult Index()
        {
            return View();
        }

        
    }
}
