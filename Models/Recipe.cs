using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class Recipe
{
    public int RecipeID { get; set; }

    [Required]
    public string Name { get; set; }
    public int PrepTimeMinutes { get; set; }
    public int CookTimeMinutes { get; set; }
    public int TotalTimeMinutes { get; set; }
    public byte[]? Picture { get; set; }
    public int UserID { get; set; }
    [ForeignKey("UserID")]
    public User User { get; set; }
    public ICollection<Step> Steps { get; set; }
    public ICollection<RecipeIngredient> Ingredients { get; set; }

}