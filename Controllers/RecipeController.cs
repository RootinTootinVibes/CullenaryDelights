using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using System.Linq;
using System.Security.Claims;

public class RecipeController : Controller
{
    private readonly ApplicationDbContext _context;

    public RecipeController(ApplicationDbContext context)
    {
        _context = context;
    }

    public IActionResult Index(string search)
    {
        var recipes = _context.Recipes.Include(x => x.Steps)
            .Include(r => r.User).AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            // Allow '*' wildcard from user; convert to SQL %
            var pattern = search.Replace('*', '%');
            if (!pattern.Contains('%'))
                pattern = $"%{pattern}";

            recipes = recipes.Where(r => EF.Functions.ILike(r.Name, pattern));    
        }

        var model = recipes
            .OrderByDescending(r => r.Name)
            .ToList();

        ViewData["CurrentFilter"] = search;

        return View(model);
    }

    [AllowAnonymous]
    public IActionResult Details(int id)
    {
        var recipe = _context.Recipes
            .Include(r => r.User)
            .Include(r => r.Steps)
            .Include(r => r.Ingredients)
                .ThenInclude(ri => ri.Ingredient)
            .FirstOrDefault(r => r.RecipeID == id);

        if (recipe == null)
            return NotFound();

        return View(recipe);
    }

    public IActionResult MyRecipes()
    {
        var user = GetCurrentUser();
        var myRecipes = _context.Recipes
            .Where(r => r.User.UserID == user.UserID)
            .OrderByDescending(r => r.Name).ToList();

        return View(myRecipes);
    }

    [HttpGet]
    public IActionResult Create() => View();

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create(Recipe recipe)
    {
        var user = GetCurrentUser();
        if (!ModelState.IsValid)
        {
            return View(recipe);
        }
        recipe.UserID = user.UserID;
        recipe.TotalTimeMinutes = recipe.PrepTimeMinutes + recipe.CookTimeMinutes;

        _context.Recipes.Add(recipe);
        _context.SaveChanges();

        return View(recipe);
    }

    [HttpGet]
    public IActionResult Edit([FromQuery] int recipeID)
    {
        var user = GetCurrentUser();
        var recipe = _context.Recipes
            .Include(r => r.Steps)
            .Include(r => r.Ingredients)
                .ThenInclude(ri => ri.Ingredient)
            .FirstOrDefault(r => r.RecipeID == recipeID && r.UserID == user.UserID);

        if (recipe == null)
            return Forbid();

        return View(recipe);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Edit(int recipeID, Recipe recipe)
    {
        if (recipeID != recipe.RecipeID)
            return BadRequest();

        var user = GetCurrentUser();
        var existing = _context.Recipes
            .Include(r => r.Steps)
            .Include(r => r.Ingredients)
                .ThenInclude(ri => ri.Ingredient)
            .FirstOrDefault(r => r.RecipeID == recipeID && r.UserID == user.UserID);

        if (existing == null)
            return Forbid();

        if (!ModelState.IsValid)
            return View(recipe);

        existing.Name = recipe.Name;
        existing.PrepTimeMinutes = recipe.PrepTimeMinutes;
        existing.CookTimeMinutes = recipe.CookTimeMinutes;
        existing.TotalTimeMinutes = existing.PrepTimeMinutes + existing.CookTimeMinutes;
        existing.Picture = recipe.Picture;
        existing.Steps = recipe.Steps;
        existing.Ingredients = recipe.Ingredients;

        _context.Recipes.Update(existing);
        _context.SaveChanges();

        return RedirectToAction(nameof(MyRecipes));
    }

    [HttpGet]
    public IActionResult Delete(int id)
    {
        var user = GetCurrentUser();
        var recipe = _context.Recipes
            .FirstOrDefault(r => r.RecipeID == id && r.UserID == user.UserID);

        if (recipe == null)
            return Forbid();

        return View(recipe);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public IActionResult DeleteConfirmed(int id)
    {
        var user = GetCurrentUser();
        var recipe = _context.Recipes
            .FirstOrDefault(r => r.RecipeID == id && r.UserID == user.UserID);

        if (recipe == null)
            Forbid();

        _context.Recipes.Remove(recipe);
        _context.SaveChanges();

        return RedirectToAction(nameof(MyRecipes));
    }

    public IActionResult FullRecipe([FromQuery] int recipeID)
    {
        var recipe = _context.Recipes
            .Include(r => r.User)
            .Include(r => r.Steps)
            .Include(r => r.Ingredients)
                .ThenInclude(ri => ri.Ingredient)
            .FirstOrDefault(r => r.RecipeID == recipeID);

        if (recipe == null)
            return NotFound();

        return View(recipe);
    }

    private User GetCurrentUser()
    {
        var claim = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
        var user = _context.Users.SingleOrDefault(u => u.UserID == claim);
        return user;
    }
}