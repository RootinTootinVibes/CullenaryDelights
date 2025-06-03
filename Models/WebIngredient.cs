using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

public class WebIngredient
{
    public int IngredientID { get; set; }

    [Required]
    public string Name { get; set; }
    public string Quantity { get; set; }

}