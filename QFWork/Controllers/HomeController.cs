using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QFWork.Data;

namespace Qualification_Work.Controllers;

[Authorize]
public class HomeController : Controller
{
    private ApplicationDbContext _context;
    public HomeController(ApplicationDbContext context)
    {
        _context = context;
    }

    public IActionResult Index()
    {
        return View();
    }
    [HttpGet]
    public IActionResult Join()
    {
        return View();
    }

    [HttpPost]
    public IActionResult Join(string courseCode)
    {
        return RedirectToAction("Index");
    }
    [HttpGet]
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    public IActionResult Create(string courseName)
    {

        return RedirectToAction("Index");
    }
}
