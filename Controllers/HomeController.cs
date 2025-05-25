using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RecipeWebsite.Models;

namespace RecipeWebsite.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly ApplicationDbContext _dbContext;

    public HomeController(ILogger<HomeController> logger, ApplicationDbContext dbContext)
    {
        _logger = logger;
        _dbContext = dbContext;
    }

    public async Task<IActionResult> Index()
    {
        // var user = new User
        // {
        //     UserName = "Kelly",
        //     UserPassword = "Brown",
        //     UserEmail = "kellybrown@utah.com",
        //     CreationDate = DateTimeOffset.UtcNow
        // };
        // // _dbContext.Users.Add(user);
        // await _dbContext.SaveChangesAsync();
        return View();
    }

    [HttpGet("Home/Login")]
    public IActionResult LoginGet()
    {
        return View("Login");
    }

    [HttpPost("Home/Login")]
    public IActionResult LoginPost([FromForm] string userName, [FromForm] string userPassword)
    {
        var user = _dbContext.Users.Where(x => x.UserName == userName).SingleOrDefault();
        if (user != null && userPassword == user.UserPassword)
        {
            return Redirect("/");
        }
        else
        {
            return View("Login");
        }
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
