using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
        var recipes = _context.Recipes.Include(x => x.Steps).ToList();
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

    public IActionResult FullRecipe([FromQuery]int recipeID)
    {
        var recipe =_context.Recipes.Where(x => x.RecipeID == recipeID).Include(x => x.Steps).Single();
        var recipeIngredients = _context.RecipeIngredients.Include(x => x.Ingredient).Where(x => x.RecipeID == recipeID).ToList();
        return View((recipe, recipeIngredients));
    }
}