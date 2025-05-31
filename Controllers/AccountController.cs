using System;
using System.Linq;
using System.Security.Claims;
using BC = BCrypt.Net.BCrypt;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RecipeWebsite.Models;

[Authorize]
public class AccountController : Controller
{
    private readonly ApplicationDbContext _dbContext;
    public AccountController(ApplicationDbContext db) => _dbContext = db;

    private bool IsValidPassword(string pw) =>
        pw.Length >= 8 &&
        pw.Any(char.IsUpper) &&
        pw.Any(char.IsLower) &&
        pw.Any(char.IsDigit);

    [HttpGet]
    public IActionResult Index()
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
        var user = _dbContext.Users.Find(userId);

        var vm = new Account
        {
            Profile = new EditProfile
            {
                Username = user.UserName,
                Email = user.UserEmail
            },
            ChangePassword = new ChangePassword()
        };
        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult ChangePassword(ChangePassword password)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
        var user = _dbContext.Users.Find(userId);

        // check duplicates
        if (!BC.Verify(password.OldPassword, user.UserPassword))
            ModelState.AddModelError(nameof(password.OldPassword), "Current password is incorrect.");

        if (!IsValidPassword(password.NewPassword))
            ModelState.AddModelError(nameof(password.NewPassword),
                "New password must be >= 8 chars, include upper and lowercase and a digit.");

        if (password.NewPassword != password.ConfirmNewPassword)
            ModelState.AddModelError(nameof(password.ConfirmNewPassword), "Passwords must match.");

        if (!ModelState.IsValid)
        {
            var vm = new Account
            {
                Profile = new EditProfile
                {
                    Username = user.UserName,
                    Email = user.UserEmail
                },
                ChangePassword = password
            };
            return View("Index", vm);
        }

        user.UserPassword = BC.HashPassword(password.NewPassword);
        _dbContext.SaveChanges();

        TempData["PasswordSuccess"] = "Password changed.";
        return RedirectToAction(nameof(Index));
    }
}