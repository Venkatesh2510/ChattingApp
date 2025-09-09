using System;
using System.ComponentModel.DataAnnotations;

namespace API.DTOs;

public class RegisterDTO
{
    [Required]
    public string DisplayName { get; set; } = "";
    [Required]
    public string Email { get; set; } = "";
    [Required]
    [StringLength(8, MinimumLength = 4)]
    public string Password { get; set; } = "";

    [Required] public string Gender { get; set; } = string.Empty;

    [Required] public string City { get; set; } = string.Empty;
    [Required] public string Country { get; set; } = string.Empty;
    [Required] public DateOnly DateOfBirth { get; set; }

}
