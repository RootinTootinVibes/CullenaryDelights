using System.ComponentModel.DataAnnotations;

public class ChangePassword
{
    [Required]
    [DataType(DataType.Password)]
    public string OldPassword { get; set; }

    [Required]
    [DataType(DataType.Password)]
    public string NewPassword { get; set; }

    [Required]
    [Compare("NewPassword", ErrorMessage = "Must match new password")]
    [DataType(DataType.Password)]
    public string ConfirmNewPassword { get; set; }
}