using Microsoft.AspNetCore.Mvc;
using System.Linq;

public class RecipeController : Controller
{
    private readonly ApplicationDbContext _context;

    public RecipeController(ApplicationDbContext context)
    {
        _context = context;
    }

    public IActionResult Index()
    {
        var recipes = _context.Recipes.ToList();
        return View(recipes);
    }

    public IActionResult Create() => View();

    [HttpPost]
    public IActionResult Create(Recipe recipe)
    {
        if (ModelState.IsValid)
        {
            _context.Recipes.Add(recipe);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        return View(recipe);
    }
}