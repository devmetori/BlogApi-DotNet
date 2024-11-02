using System.ComponentModel.DataAnnotations;

namespace BlogApi.Shared.DTOs.Auth;

public class SignInDto
{
    [Required(ErrorMessage = "El correo electrónico es obligatorio.")]
    [EmailAddress(ErrorMessage = "El correo electrónico no es válido.")]
    public string email { get; set; }

    [Required(ErrorMessage = "La contraseña es obligatoria.")]
    public string password { get; set; }
}