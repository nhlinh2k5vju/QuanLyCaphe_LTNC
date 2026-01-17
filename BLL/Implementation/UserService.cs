using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BLL.Interface;
using BLL.Validators;
using DAL.Entities;
using DAL.Interface;

namespace BLL.Implementation
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepo;

        public UserService(IUserRepository userRepo)
        {
            _userRepo = userRepo;
        }

        public async Task<User?> LoginAsync(string username, string password)
        {
            UserValidator.ValidateForLogin(username, password);

            return await _userRepo.LoginAsync(username, password);
        }

        public async Task<List<User>> GetAllAsync()
        {
            return await _userRepo.GetAllAsync();
        }

        public async Task AddAsync(User user)
        {
            UserValidator.ValidateForCreate(user);

            await _userRepo.AddAsync(user);
        }

        public async Task UpdateAsync(User user)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));
            await _userRepo.UpdateAsync(user);
        }
    }
}
