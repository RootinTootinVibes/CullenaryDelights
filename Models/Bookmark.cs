using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

[PrimaryKey(nameof(UserID), nameof(RecipeID))]
public class Bookmark
{
    public int UserID { get; set; }

    [Required]
    public int RecipeID { get; set; }

    [Required]
    public DateTimeOffset CreationDate { get; set; }

}