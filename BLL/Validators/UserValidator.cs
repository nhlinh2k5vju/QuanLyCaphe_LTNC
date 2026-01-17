using System;
using System.Collections.Generic;
using System.Text;

using BLL.Exceptions;
using DAL.Entities;

namespace BLL.Validators
{
    public static class UserValidator
    {
        public static void ValidateForLogin(string username, string password)
        {
            if (string.IsNullOrWhiteSpace(username))
                throw new BusinessException("Tên đăng nhập không được để trống.");

            if (string.IsNullOrWhiteSpace(password))
                throw new BusinessException("Mật khẩu không được để trống.");
        }

        public static void ValidateForCreate(User user)
        {
            if (string.IsNullOrWhiteSpace(user.Username))
                throw new BusinessException("Username không hợp lệ.");

            if (user.PasswordHash.Length < 6)
                throw new BusinessException("Mật khẩu phải có ít nhất 6 ký tự.");

            if (string.IsNullOrWhiteSpace(user.FullName))
                throw new BusinessException("Họ tên không được để trống.");
        }
    }
}

