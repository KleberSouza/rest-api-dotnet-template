using rest_api_dotnet_template.Data;
using rest_api_dotnet_template.Models;
using rest_api_dotnet_template.Repositories;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BC = BCrypt.Net.BCrypt;

namespace rest_api_dotnet_template.Services
{
    public interface IUsuarioService : IService<Usuario> {
        Task<Usuario> GetByEmailAsync(string email);

        Task RegisterAsync(Usuario entity);

        Task<string> Authenticate(string email, string password);
    }

    public class UsuarioService : Service<Usuario, ApiDbContext>, IUsuarioService
    {
        private readonly IUsuarioRepository _userRepository;
        private readonly IConfiguration _configuration;

        public UsuarioService(IUsuarioRepository repository, IConfiguration configuration) : base(repository)
        {
            _userRepository = repository;
            _configuration = configuration;
        }

        public async Task<Usuario> GetByEmailAsync(string email)
        {
            try
            {
                return await _userRepository.GetByEmailAsync(email);
            }
            catch (KeyNotFoundException ex)
            {
                throw new KeyNotFoundException($"Entidade do tipo Usuário com o e-mail {email} não encontrada.", ex);
            }
            catch (Exception ex)
            {
                throw new Exception($"Falha ao recuperar a entidade com o e-mail {email}.", ex);
            }
        }

        public virtual async Task RegisterAsync(Usuario entity)
        {
            ValidateEntity(entity);

            try
            {
                entity.Perfil = UsuarioPerfil.Cliente;
                await _repository.AddAsync(entity);
            }
            catch (Exception ex)
            {
                throw new Exception("Ocorreu um erro ao adicionar a entidade.", ex);
            }
        }

        public async Task<string> Authenticate(string email, string password)
        {
            try
            {
                var user = await _userRepository.GetByEmailAsync(email);

                if (user == null || !BC.Verify(password, user.Senha))
                    return null;

                return GenerateJwtToken(user);
            }
            catch (Exception ex)
            {
                throw new Exception("Ocorreu um erro ao tentar autenticar o usuário.", ex);
            }
        }

        private string GenerateJwtToken(Usuario user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"]);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Nome),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Perfil.ToString())
            }),
                Expires = DateTime.UtcNow.AddHours(8),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
                Issuer = _configuration["Jwt:Issuer"],
                Audience = _configuration["Jwt:Audience"]
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
