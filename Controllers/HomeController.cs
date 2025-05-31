using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RecipeWebsite.Models;
using BCrypt.Net;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;

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
        return View();
    }

    [HttpGet("Home/Login")]
    public IActionResult LoginGet()
    {
        return View("Login");
    }

    // [HttpPost("Home/Login")]
    // public IActionResult LoginPost([FromForm] string userName, [FromForm] string userPassword)
    // {
    //     var user = _dbContext.Users.Where(x => x.UserName == userName).SingleOrDefault();
    //     if (user != null && userPassword == user.UserPassword)
    //     {
    //         return Redirect("/");
    //     }
    //     else
    //     {
    //         return View("Login");
    //     }
    // }

    [HttpGet]
    public IActionResult Register() => View();

    [HttpPost]
    public IActionResult Register(RegisterUser ru)
    {
        if (!ModelState.IsValid)
            return View(ru);

        // Check for existing User/Email
        if (_dbContext.Users.Any(u => u.UserName == ru.UserName))
            ModelState.AddModelError("", "Username already in use");
        if (_dbContext.Users.Any(u => u.UserEmail == ru.UserEmail))
            ModelState.AddModelError("", "Email already registered.");

        // Enforce complexity: >= 8 chars, upper, lower, digit
        if (!IsValidPassword(ru.Password))
            ModelState.AddModelError("",
                "Password must be at least 8 characters, " +
                "include uppercase, lowercase, and a digit.");

        if (!ModelState.IsValid)
            return View(ru);

        // All good -> hash & save
        var user = new User
        {
            UserName = ru.UserName,
            UserEmail = ru.UserEmail,
            UserPassword = BCrypt.Net.BCrypt.HashPassword(ru.Password),
            CreationDate = DateTimeOffset.UtcNow
        };
        _dbContext.Users.Add(user);
        _dbContext.SaveChanges();

        return RedirectToAction("Login");

    }

    [HttpGet]
    public IActionResult ChangePassword() => View();

    [HttpPost]
    public IActionResult ChangePassword(ChangePassword cp)
    {
        if (!ModelState.IsValid)
            return View(cp);

        // Retrieve current user
        if (!User.Identity.IsAuthenticated)
            return Challenge();
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
        var user = _dbContext.Users.SingleOrDefault(u => u.UserID == userId);
        if (user == null
            || !BCrypt.Net.BCrypt.Verify(cp.OldPassword, user.UserPassword))
        {
            ModelState.AddModelError("", "Current password is incorrect.");
            return View(cp);
        }

        if (!IsValidPassword(cp.NewPassword))
        {
            ModelState.AddModelError("",
                "New password must be at least 8 chars, " +
                "include uppercase, lowercase, and a digit.");
            return View(cp);
        }

        user.UserPassword = BCrypt.Net.BCrypt.HashPassword(cp.NewPassword);
        _dbContext.SaveChanges();

        return RedirectToAction("Index", "Home");

    }

    [HttpPost("Home/Login")]
    public async Task<IActionResult> Login(LoginModel lm)
    {
        if (!ModelState.IsValid) return View(lm);

        var user = _dbContext.Users
                    .SingleOrDefault(u => u.UserName == lm.Name);

        if (user == null || lm.Password != user.UserPassword)
        {
            ModelState.AddModelError("", "Invalid credentials.");
            return View(lm);
        }

        // Create claims
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.UserID.ToString()),
            new Claim(ClaimTypes.Name, user.UserName)
        };

        // var claims = new List<Claim>
        // {
        //     new Claim(ClaimTypes.Name, user.Email),
        //     new Claim("FullName", user.FullName),
        //     new Claim(ClaimTypes.Role, "Administrator"),
        // };

        // Create identity and principal
        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var principal = new ClaimsPrincipal(identity);

        //Sign in
        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            principal,
            new AuthenticationProperties
            {
                IsPersistent = lm.RememberMe,
                ExpiresUtc = DateTimeOffset.UtcNow.AddHours(2)
            }
        );

        return RedirectToAction("Index", "Home");
    }

    [HttpPost]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Index", "Home");
    }

    private bool IsValidPassword(string pw)
    {
        return pw.Length >= 8
            && pw.Any(char.IsUpper)
            && pw.Any(char.IsLower)
            && pw.Any(char.IsDigit);
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
