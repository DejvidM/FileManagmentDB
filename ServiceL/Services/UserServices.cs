using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using DomainL.Entities;
using DataAccessL.Repositories;
using ServiceL.Interfaces;
using DataAccessL.Interfaces;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using DomainL.Configuration;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using ServiceL.DTO;

namespace ServiceL.Services
{
    public class UserServices : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHashCompare _passwordHashCompare;
        private readonly IFoldersRepository _folderRepository;

        public UserServices(IUserRepository userRepository, IPasswordHashCompare passwordHashCompare, IFoldersRepository folderRepository)
        {
            _userRepository = userRepository;
            _passwordHashCompare = passwordHashCompare;
            _folderRepository = folderRepository;
        }

        public async Task<IEnumerable<User>> GetAllUsersAsync()
        {
            return await _userRepository.GetAllAsync();
        }

        public async Task<User> AddUserAsync(User user)
        {
            user.Password = _passwordHashCompare.HashPasswordAsync(user.Password);

            await _userRepository.AddAsync(user);
            return user;
        }

        public async Task UpdateUserAsync(User user)
        {
            await _userRepository.UpdateAsync(user);
        }

        public async Task<bool> DeleteUserAsync(int id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null)
            {
                return false;
            }

            var response = await _userRepository.DeleteAsync(user);
            return response;
        }
        public async Task<UserDTO> GetUserByIdAsync(int id)
        {
            var user = await _userRepository.GetByIdAsync(id);

            if (user == null) 
            {
                return null;
            }

            var userFolders = await _folderRepository.GetAllUserFoldersFiles(id);

            return new UserDTO
            {
                Id = user.Id,
                Username = user.Username,
                Folders = userFolders
            };

        }
        public async Task<User> LogInUser(string email, string password)
        {
            var user = await _userRepository.FindByEmail(email);

            if (user == null) 
            {
                throw new Exception("User not found");
            }
            var match = _passwordHashCompare.ComparePasswords(password, user);
            if (match)
            {
                return user;
            }
            else
            {
                throw new Exception("Passwords do not match");
            }
        }

        public string GenerateJwtToken(int userId, string userName, JWTSettings jwtSettings)
        {

            string userID = userId.ToString();

            var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, userID),
            new Claim(JwtRegisteredClaimNames.UniqueName, userName),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.SecretKey));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
               issuer: jwtSettings.Issuer,
               audience: jwtSettings.Audience,
               claims: claims,
               expires: DateTime.UtcNow.AddMinutes(jwtSettings.TokenLifetime),
               signingCredentials: credentials
            );


            return new JwtSecurityTokenHandler().WriteToken(token);
            }
        }
}
