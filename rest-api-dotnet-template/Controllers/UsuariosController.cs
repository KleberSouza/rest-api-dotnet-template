using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using rest_api_dotnet_template.Services;
using rest_api_dotnet_template.Models;
using Microsoft.AspNetCore.Authorization;
using BC = BCrypt.Net.BCrypt;

namespace rest_api_dotnet_template.Controllers
{
    public class UsuariosController : BaseController<Usuario, IUsuarioService>
    {
        private readonly IMapper _mapper;
        private readonly IUsuarioService _userService;

        public UsuariosController(IUsuarioService service, IMapper mapper) : base(service)
        {
            _mapper = mapper;
            _userService = service;
        }

        [Authorize(Roles = "Administrador")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public override async Task<ActionResult> GetAll(int page = 1, int pageSize = 10)
        {            
            return await base.GetAll(page, pageSize);
        }

        [Authorize]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public override async Task<ActionResult> GetById(int id)
        {
            return await base.GetById(id);
        }

        [Authorize(Roles = "Administrador")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public override async Task<ActionResult> Create([FromBody] Usuario entity)
        {            
            entity.Senha = BC.HashPassword(entity.Senha);
            entity.Perfil = UsuarioPerfil.Administrador;
            return await base.Create(entity);
        }

        [Authorize(Roles = "Administrador")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public override async Task<ActionResult> Update(int id, [FromBody] Usuario entity)
        {
            entity.Senha = BC.HashPassword(entity.Senha);
            return await base.Update(id, entity);
        }

        [Authorize(Roles = "Administrador")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public override async Task<ActionResult> Delete(int id)
        {
            return await base.Delete(id);
        }

        [HttpPost("register")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> Register([FromBody] RegisterDto usuario)
        {
            try
            {
                var entity = _mapper.Map<Usuario>(usuario);
                entity.Senha = BC.HashPassword(entity.Senha);
                entity.Perfil = UsuarioPerfil.Cliente;
                await _userService.RegisterAsync(entity);
                return CreatedAtAction(nameof(GetById), new { id = entity.Id }, entity);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = ex.Message });
            }
        }

        [HttpPost("login")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Login([FromBody] LoginDto usuario)
        {
            try
            {
                string token = await _userService.Authenticate(usuario.Email, usuario.Senha);

                if (token == null)
                {
                    return Unauthorized("E-mail e/ou Senha inválidos.");
                }

                return Ok(new { token });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = ex.Message });
            }
        }
    }
}
