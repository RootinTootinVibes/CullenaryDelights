using System.ComponentModel.DataAnnotations;

public class RegisterUser
{
    [Required]
    public string UserName { get; set; }

    [Required]
    [EmailAddress]
    public string UserEmail { get; set; }

    [Required]
    [DataType(DataType.Password)]
    public string Password { get; set; }

    [Required]
    [Compare("Password", ErrorMessage = "Passwords must match.")]
    [DataType(DataType.Password)]
    public string ConfirmPassword { get; set; }
}