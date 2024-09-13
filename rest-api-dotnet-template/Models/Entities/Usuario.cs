using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace rest_api_dotnet_template.Models
{
    [Table("Usuarios")]
    public class Usuario: BaseEntity
    {
        [Required(ErrorMessage = "O nome é obrigatório.")]
        [MaxLength(100, ErrorMessage = "O nome não pode exceder 100 caracteres.")]
        public string Nome { get; set; }

        [Required(ErrorMessage = "O email é obrigatório.")]
        [EmailAddress(ErrorMessage = "O email deve ser válido.")]
        [MaxLength(100, ErrorMessage = "O email não pode exceder 100 caracteres.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "A senha é obrigatória.")]
        [MinLength(8, ErrorMessage = "A senha deve ter pelo menos 8 caracteres.")]
        public string Senha { get; set; } 

        [Required(ErrorMessage = "O perfil de usuário é obrigatório.")]
        public UsuarioPerfil Perfil { get; set; }

        public string PerfilDescricao
        {
            get { return Perfil.ToString(); }
        }
    }

    public enum UsuarioPerfil
    {
        [Display(Name = "Administrador")]
        Administrador,
        [Display(Name = "Cliente")]
        Cliente
    }
}
