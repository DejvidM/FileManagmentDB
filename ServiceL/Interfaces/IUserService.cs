using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DomainL.Entities;
using Microsoft.AspNetCore.Mvc;
using ServiceL.DTO;

namespace ServiceL.Interfaces
{
    public interface IUserService
    {
        Task<UserDTO?> GetUserByIdAsync(int id);
        Task<IEnumerable<User>> GetAllUsersAsync();
        Task<User> AddUserAsync(User user);
        Task<bool> DeleteUserAsync(int id);
        Task<User> LogInUser(string email, string password);
    }
}
