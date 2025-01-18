using DomainL.Configuration;
using DomainL.Entities;
using FinalProject.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using ServiceL.Services;
using System.Net.Mail;
using System.Text.RegularExpressions;

namespace FinalProject.Controllers
{
    [AllowAnonymous]
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {

        private readonly UserServices _authService;
        private readonly JWTSettings _jwtSettings;
        private static readonly Regex PasswordRegex = new Regex(
    "^(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])(?=.*?[#?!@$%^&*-]).{8,}$",
    RegexOptions.Compiled);

        public AuthController(UserServices authService, IOptions<JWTSettings> jwtSettings)
        {
            _authService = authService;
            _jwtSettings = jwtSettings.Value;
        }

        [HttpPost("Register")]
        public async Task<IActionResult> Register([FromBody] RegisterDTO registerDTO)
        {
            if (string.IsNullOrWhiteSpace(registerDTO.email) 
                || string.IsNullOrWhiteSpace(registerDTO.userName) 
                || string.IsNullOrWhiteSpace(registerDTO.password)
                )
            {
                return BadRequest("User email, password, and username are required.");
            }

            try
            {
                MailAddress m = new MailAddress(registerDTO.email);
            }
            catch (Exception ex)
            {
                return BadRequest("Error : " + ex.Message);
            }

            var users = await _authService.GetAllUsersAsync();
            if(users.Any(u => u.Email == registerDTO.email))
            {
                return BadRequest("A user with this email already exists");
            }

            if (!PasswordRegex.IsMatch(registerDTO.password))
            {
                return BadRequest("Password must contain at least 8 characters, one uppercase letter, one lowercase letter, one number, and one special character.");
            }

            if (registerDTO.userName.Length < 5)
            {
                return BadRequest("Username must be at least 5 characters");
            }

            if (users.Any(u => u.Username == registerDTO.userName))
            {
                return BadRequest("Username is taken.");
            }

            var user = await _authService.AddUserAsync(new User
            {
                Username = registerDTO.userName,
                Email = registerDTO.email,
                Password = registerDTO.password
            });


            string token = _authService.GenerateJwtToken(user.Id, user.Username, _jwtSettings);

            return Ok(new
            {
                Token = token,
                Message = "User registered successfully",
                user.Id
            });
        }

        [HttpPost("LogIn")]
        public async Task<IActionResult> LogIn([FromBody] LoginDTO loginDTO)
        {
            if (string.IsNullOrWhiteSpace(loginDTO.Email) || string.IsNullOrWhiteSpace(loginDTO.Password))
            {
                return BadRequest(new
                {
                    Erorr = "Email and Password are required"
                });
            }

            try
            {
                var user = await _authService.LogInUser(loginDTO.Email , loginDTO.Password );

                string token = _authService.GenerateJwtToken(user.Id, user.Username, _jwtSettings);

                return Ok(new
                {
                    Token = token,
                    Message = "User logged in successfully",
                    user.Id
                });

            }
            catch (Exception ex) 
            {
                return Unauthorized(ex.Message);
            }


        }
    }
}
