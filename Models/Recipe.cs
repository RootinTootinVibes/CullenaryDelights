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
    public DateTime CreatedAt { get; set; }
    public double? OverallRating { get; set; }

    [ForeignKey("UserID")]
    public User User { get; set; }
    public List<Step> Steps { get; set; }
    public List<RecipeIngredient> Ingredients { get; set; }

    public List<CommentAndRating> CommentsAndRatings { get; set; }
        = new List<CommentAndRating>();

}