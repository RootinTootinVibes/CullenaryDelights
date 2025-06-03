using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class WebRecipe
{
    public int RecipeID { get; set; }
    public string Name { get; set; }
    public int PrepTimeMinutes { get; set; }
    public int CookTimeMinutes { get; set; }
    public int TotalTimeMinutes { get; set; }
    public List<Step> Steps { get; set; }
    public List<WebIngredient> Ingredients { get; set; }

}