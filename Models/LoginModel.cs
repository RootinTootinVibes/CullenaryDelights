using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

public class LoginModel
{
    [Required]
    [Display(Name = "Name")]
    [FromForm(Name = "userName")]
     public string Name { get; set; }

    [Required]
    [DataType(DataType.Password)]
    [Display(Name = "Password")]
    [FromForm(Name = "userPassword")]
    public string Password { get; set; }

    [Display(Name = "Remember me?")]
    public bool RememberMe { get; set; }
}