using DomainL.Configuration;
using DomainL.Entities;
using FinalProject.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using ServiceL.Services;
using System.Net.Mail;

namespace FinalProject.Controllers
{
    [AllowAnonymous]
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {

        private readonly UserServices _authService;
        private readonly JWTSettings _jwtSettings;

        public AuthController(UserServices authService, IOptions<JWTSettings> jwtSettings)
        {
            _authService = authService;
            _jwtSettings = jwtSettings.Value;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDTO registerDTO)
        {
            if (string.IsNullOrWhiteSpace(registerDTO.email) 
                || string.IsNullOrWhiteSpace(registerDTO.userName) 
                || string.IsNullOrWhiteSpace(registerDTO.password)
                )
            {
                return BadRequest("User email, password, and username are required.");
            }

            //try
            //{
            //    MailAddress m = new MailAddress(registerDTO.email);
            //}
            //catch (FormatException)
            //{
            //    return BadRequest("Invalid email!");
            //}

            var user = await _authService.AddUserAsync(new User
            {
                Username = registerDTO.userName,
                Email = registerDTO.email,
                Password = registerDTO.password
            });


            string token = _authService.GenerateJwtToken( user.Id, user.Username, _jwtSettings);

            return Ok(new
            {
                Token = token,
                Message = "User registered successfully"
            });
        }

        [HttpPost("LogIn")]
        public async Task<IActionResult> LogIn([FromBody] LoginDTO loginDTO)
        {
            if (string.IsNullOrWhiteSpace(loginDTO.Email) || string.IsNullOrWhiteSpace(loginDTO.Password))
            {
                return BadRequest("Email and password are required");
            }

            try
            {
                var user = await _authService.LogInUser(loginDTO.Email , loginDTO.Password );

                string token = _authService.GenerateJwtToken(user.Id, user.Username, _jwtSettings);

                return Ok(new
                {
                    Token = token,
                    Message = "User logged in successfully"
                });

            }
            catch (Exception ex) 
            {
                return Unauthorized(ex.Message);
            }


        }
    }
}
