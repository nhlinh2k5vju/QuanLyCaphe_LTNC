using System;
using System.Collections.Generic;
using System.Text;

using DAL.Entities;

namespace DAL.Interface
{
    public interface IUserRepository
    {
        Task<User?> LoginAsync(string username, string passwordHash);
        Task<User?> GetByIdAsync(int userId);
        Task<List<User>> GetAllAsync();
        Task AddAsync(User user);
        Task UpdateAsync(User user);
    }
}

