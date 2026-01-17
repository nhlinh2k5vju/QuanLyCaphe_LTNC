using System;
using System.Collections.Generic;
using System.Text;

using DAL.Entities;

namespace BLL.Interface
{
    public interface IUserService
    {
        Task<User?> LoginAsync(string username, string password);
        Task<List<User>> GetAllAsync();
        Task AddAsync(User user);
        Task UpdateAsync(User user);
    }
}

