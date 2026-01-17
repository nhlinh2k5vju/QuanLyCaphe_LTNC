using System;
using System.Collections.Generic;
using System.Text;

using DAL.Entities;
using DAL.Interface;
using Microsoft.EntityFrameworkCore;

namespace DAL.Implementation
{
    public class UserRepository : IUserRepository
    {
        private readonly CoffeDbContext _context;

        public UserRepository(CoffeDbContext context)
        {
            _context = context;
        }

        public async Task<User?> LoginAsync(string username, string passwordHash)
        {
            return await _context.Users
                .FirstOrDefaultAsync(u =>
                    u.Username == username &&
                    u.PasswordHash == passwordHash &&
                    u.Status);
        }

        public async Task<User?> GetByIdAsync(int userId)
        {
            return await _context.Users.FindAsync(userId);
        }

        public async Task<List<User>> GetAllAsync()
        {
            return await _context.Users.ToListAsync();
        }

        public async Task AddAsync(User user)
        {
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(User user)
        {
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
        }
    }
}

