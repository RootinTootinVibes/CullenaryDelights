using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

public class Ingredient
{
    public int IngredientID { get; set; }

    [Required]
    public string Name { get; set; }

    [Required]
    public string FoodGroup { get; set; }
}