using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

public class ChangePassword
{
    [Required]
    [DataType(DataType.Password)]
    [FromForm(Name = "OldPassword")]
    public string OldPassword { get; set; }

    [Required]
    [DataType(DataType.Password)]
    [FromForm(Name = "NewPassword")]
    public string NewPassword { get; set; }

    [Required]
    [Compare("NewPassword", ErrorMessage = "Must match new password")]
    [DataType(DataType.Password)]
    [FromForm(Name = "ConfirmNewPassword")]
    public string ConfirmNewPassword { get; set; }
}