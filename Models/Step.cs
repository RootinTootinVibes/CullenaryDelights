using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

[PrimaryKey(nameof(RecipeID), nameof(StepOrdinal))]
public class Step
{
    [ForeignKey(nameof(Recipe))]
    public int RecipeID { get; set; }

    
    public int StepOrdinal { get; set; }
    [Required]
    public string Instruction { get; set; }

}