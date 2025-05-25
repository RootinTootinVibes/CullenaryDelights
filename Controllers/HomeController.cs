using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
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

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
