using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ProductivityApp.Data;
using ProductivityApp.Data.Models;

namespace ProductivityApp.Services
{
    public interface IUserService
    {
        Task<UserProfile> CreateUserAsync(string username);
        Task<List<UserProfile>> GetUsersAsync();
        Task<UserProfile?> GetUserByIdAsync(int id);
    }

    public class UserService : IUserService
    {
        private readonly ProductivityDbContext _dbContext;

        public UserService(ProductivityDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<UserProfile> CreateUserAsync(string username)
        {
            var user = new UserProfile { UserName = username.Trim(), CreatedAt = DateTime.Now };
            _dbContext.UserProfiles.Add(user);
            await _dbContext.SaveChangesAsync();
            return user;
        }

        public async Task<List<UserProfile>> GetUsersAsync()
        {
            return await _dbContext.UserProfiles.ToListAsync();
        }

        public async Task<UserProfile?> GetUserByIdAsync(int id)
        {
            return await _dbContext.UserProfiles.FindAsync(id);
        }
    }
}
