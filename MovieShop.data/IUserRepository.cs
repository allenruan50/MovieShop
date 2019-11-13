﻿using MovieShop.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MovieShop.Data
{
    public interface IUserRepository:IRepository<User>
    {
        Task<User> GetUserByEmail(string Email);
        Task<IEnumerable<Purchase>> GetUserPurchasedMovies(int UserId);
    }
}
