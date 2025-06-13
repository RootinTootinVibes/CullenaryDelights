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

        ViewData["CurrentFiler"] = search;

        var recipes = _context.Recipes.Include(x => x.Steps)
            .Include(r => r.User)
            .Include(r => r.Ingredients)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            // Allow '*' wildcard from user; convert to SQL %
            var pattern = search.Replace('*', '%');
            if (!pattern.Contains('%'))
                pattern = $"%{pattern}";

            recipes = recipes.Where(r =>
                EF.Functions.ILike(r.Name, pattern) ||
                r.Ingredients.Any(ri =>
                    EF.Functions.ILike(ri.Ingredient.Name, pattern)));    
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
            .Include(r => r.CommentsAndRatings)
                .ThenInclude(cr => cr.User)
            .FirstOrDefault(r => r.RecipeID == id);

        if (recipe == null)
            return NotFound();

        return View(recipe);
    }

    [HttpPost]
    [Authorize]
    [ValidateAntiForgeryToken]
    public IActionResult AddComment(int recipeID, int rating, string comment)
    {
        //Validating comment
        if (rating < 1 || rating > 5)
            ModelState.AddModelError("Rating", "Rating must be between 1 & 5");

        if (string.IsNullOrWhiteSpace(comment))
            ModelState.AddModelError("Comment", "Comment cannot be empty");

        if (!ModelState.IsValid)
        {
            //get the recipe exactly as in GET
            var recipeToShow = _context.Recipes
                .Include(r => r.User)
                .Include(r => r.Steps)
                .Include(r => r.Ingredients)
                    .ThenInclude(ri => ri.Ingredient)
                .Include(r => r.CommentsAndRatings)
                    .ThenInclude(cr => cr.User)
                .FirstOrDefault(r => r.RecipeID == recipeID);

            if (recipeToShow == null)
                return NotFound();

            return View("Details", recipeToShow);
        };

        //Creating and saving new Comment and Rating
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        int userId = int.Parse(userIdClaim);

        var newComment = new CommentAndRating
        {
            UserID = userId,
            RecipeID = recipeID,
            Rating = rating,
            Comment = comment
        };

        _context.CommentsAndRatings.Add(newComment);
        _context.SaveChanges();

        //Recalculating and updating recipe's overall rating
        var allRatings = _context.CommentsAndRatings
            .Where(cr => cr.RecipeID == recipeID)
            .Select(cr => cr.Rating)
            .ToList();

        double average = allRatings.Any()
            ? allRatings.Average()
            : 0.0;

        var recipeToUpdate = _context.Recipes.First(r => r.RecipeID == recipeID);
        recipeToUpdate.OverallRating = average;
        _context.SaveChanges();

        return RedirectToAction("FullRecipe", new { recipeID = recipeID });
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
    public IActionResult Create(WebRecipe recipe)
    {
        var user = GetCurrentUser();
        if (!ModelState.IsValid)
        {
            return View(recipe);
        }
        Recipe dbRecipe = new Recipe
        {
            Name = recipe.Name,
            PrepTimeMinutes = recipe.PrepTimeMinutes,
            CookTimeMinutes = recipe.CookTimeMinutes,
            TotalTimeMinutes = recipe.PrepTimeMinutes + recipe.CookTimeMinutes,
            UserID = user.UserID,
            RecipeID = recipe.RecipeID,
            CreatedAt = DateTime.Now
        };

        _context.Recipes.Add(dbRecipe);
        var ingredients = recipe.Ingredients.ConvertAll(x => new Ingredient
        {
            Name = x.Name,
            FoodGroup = ""
        });
        _context.Ingredients.AddRange(ingredients);
        _context.SaveChanges();

        int ordinalId = 0;
        foreach (var step in recipe.Steps)
        {
            step.RecipeID = dbRecipe.RecipeID;
            step.StepOrdinal = ordinalId++;
        }

        int n = 0;
        var recipeIngredients = ingredients.ConvertAll(x => new RecipeIngredient
        {
            RecipeID = dbRecipe.RecipeID,
            IngredientID = x.IngredientID,
            Quantity = recipe.Ingredients[n++].Quantity,

        });
        _context.RecipeIngredients.AddRange(recipeIngredients);
        _context.Steps.AddRange(recipe.Steps);
        _context.SaveChanges();

        return View(dbRecipe);
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
    public IActionResult Edit(int recipeID, WebRecipe recipe)
    {
        if (recipeID != recipe.RecipeID)
            return BadRequest();

        var user = GetCurrentUser();
        var existing = _context.Recipes
            .Include(r => r.User)
            .Include(r => r.Steps)
            .Include(r => r.Ingredients)
                .ThenInclude(ri => ri.Ingredient)
            .FirstOrDefault(r => r.RecipeID == recipeID && r.UserID == user.UserID);

        if (existing == null)
            return Forbid();

        if (!ModelState.IsValid)
            return View(recipe);


        var ingredients = recipe.Ingredients.ConvertAll(x => new Ingredient
        {
            Name = x.Name,
            FoodGroup = ""
        });
        _context.Ingredients.AddRange(ingredients);
        _context.SaveChanges();
        existing.Name = recipe.Name;
        existing.PrepTimeMinutes = recipe.PrepTimeMinutes;
        existing.CookTimeMinutes = recipe.CookTimeMinutes;
        existing.TotalTimeMinutes = existing.PrepTimeMinutes + existing.CookTimeMinutes;
        _context.Steps.RemoveRange(existing.Steps);
        //_context.SaveChanges();
        int ordinalId = 0;
        foreach (var step in recipe.Steps)
        {
            step.RecipeID = existing.RecipeID;
            step.StepOrdinal = ordinalId++;
        }
        _context.Steps.AddRange(recipe.Steps);
        _context.RecipeIngredients.RemoveRange(existing.Ingredients);
        int n = 0;
        var recipeIngredients = ingredients.ConvertAll(x => new RecipeIngredient
        {
            RecipeID = existing.RecipeID,
            IngredientID = x.IngredientID,
            Quantity = recipe.Ingredients[n++].Quantity,

        });
        _context.RecipeIngredients.AddRange(recipeIngredients);

        _context.SaveChanges();

        return RedirectToAction(nameof(MyRecipes));
    }

    [HttpGet]
    public IActionResult Delete(int recipeID)
    {
        var user = GetCurrentUser();
        var recipe = _context.Recipes
            .FirstOrDefault(r => r.RecipeID == recipeID && r.UserID == user.UserID);

        if (recipe == null)
            return Forbid();

        return View(recipe);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public IActionResult DeleteConfirmed(int recipeID)
    {
        var user = GetCurrentUser();
        var recipe = _context.Recipes
            .FirstOrDefault(r => r.RecipeID == recipeID && r.UserID == user.UserID);

        if (recipe == null)
            Forbid();

        _context.RecipeIngredients.Where(x => x.RecipeID == recipeID).ExecuteDelete();
        _context.Steps.Where(x => x.RecipeID == recipeID).ExecuteDelete();
        _context.Recipes.Remove(recipe);
        _context.SaveChanges();

        return RedirectToAction(nameof(MyRecipes));
    }

    public IActionResult FullRecipe([FromQuery] int recipeID)
    {
        var recipe = _context.Recipes
            .Include(r => r.User)
            .Include(r => r.Steps)
            .Include(r => r.CommentsAndRatings)
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