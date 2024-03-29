﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using MovieShop.Data;
using MovieShop.Entities;

namespace MovieShop.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly ICryptoService _cryptoService;
        public UserService(IUserRepository userRepository, ICryptoService cryptoService)
        {
            _userRepository = userRepository;
            _cryptoService = cryptoService;
        }
        public async Task<User> CreateUser(string email, string password, string firstName, string lastName)
        {
            var dbUser = await _userRepository.GetUserByEmail(email);
            if (dbUser != null)
            {
                return null;
            }
            var salt = _cryptoService.CreateSalt();
            var hashPassword = _cryptoService.HashPassword(password, salt);
            var user = new User
            {
                Email = email,
                FirstName = firstName,
                LastName = lastName,
                HashedPassword = hashPassword,
                Salt = salt
            };
            var createdUser = await _userRepository.AddAsync(user);
            return createdUser;
        }

        public async Task<IEnumerable<Purchase>> GetPurchases(int userId)
        {
            return await _userRepository.GetUserPurchasedMovies(userId);
        }

        public async Task<User> ValidateUser(string email, string password)
        {
            var dbUser = await _userRepository.GetUserByEmail(email);
            var hashPassword = _cryptoService.HashPassword(password, dbUser.Salt);
            if (dbUser == null)
            {
                return null;
            }
            if (hashPassword != dbUser.HashedPassword)
            {
                return null;
            }
            return dbUser;
        }
    }
}
