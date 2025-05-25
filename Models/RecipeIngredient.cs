using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

[PrimaryKey(nameof(IngredientID), nameof(RecipeID))]
public class RecipeIngredient
{
    public int RecipeID { get; set; }

    [Required]
    public int IngredientID { get; set; }

    [Required]
    public string Quantity { get; set; }

}