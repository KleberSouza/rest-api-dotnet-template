using System.ComponentModel.DataAnnotations;

namespace rest_api_dotnet_template.Models
{
    public class LoginDto
    {
        [Required(ErrorMessage = "O email é obrigatório.")]
        [EmailAddress(ErrorMessage = "O email deve ser válido.")]
        [MaxLength(100, ErrorMessage = "O email não pode exceder 100 caracteres.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "A senha é obrigatória.")]
        [MinLength(8, ErrorMessage = "A senha deve ter pelo menos 8 caracteres.")]
        public string Senha { get; set; }
    }
}
