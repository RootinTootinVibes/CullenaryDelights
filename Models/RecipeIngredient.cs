using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

[PrimaryKey(nameof(IngredientID), nameof(RecipeID))]
public class RecipeIngredient
{
    public int RecipeID { get; set; }

    [Required]
    [ForeignKey(nameof(Ingredient))]
    public int IngredientID { get; set; }

    [Required]
    public string Quantity { get; set; }

    public Ingredient Ingredient { get; set; }

}