using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.Design.Serialization;
using Microsoft.EntityFrameworkCore;

[PrimaryKey(nameof(CommentID))]
public class CommentAndRating
{
    [Required]
    public int CommentID;

    [Required]
    [ForeignKey(nameof(UserID))]
    public int UserID { get; set; }

    [Required]
    [ForeignKey(nameof(RecipeID))]
    public int RecipeID { get; set; }

    [Range(1, 5)]
    public int Rating { get; set; }

    [Required]
    public string Comment { get; set; }
    public User User { get; set; }
    public Recipe Recipe { get; set; }
}