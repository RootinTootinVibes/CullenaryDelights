using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

public class User
{
    public int UserID { get; set; }

    [Required]
    public string UserName { get; set; }

    [Required]
    public string UserEmail { get; set; }

    [Required]
    public string UserPassword { get; set; }

    public DateTimeOffset CreationDate { get; set; }
    
    public ICollection<Recipe> Recipes { get; set; }
}