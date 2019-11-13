using Microsoft.EntityFrameworkCore;
using MovieShop.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace MovieShop.Data
{
    public class UserRepository:Repository<User>, IUserRepository
    {
        public UserRepository(MovieShopDbContext dbContext) : base(dbContext)
        {

        }

        public async Task<User> GetUserByEmail(string Email)
        {
            return await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == Email);
        }

        public async Task<IEnumerable<Purchase>> GetUserPurchasedMovies(int userId)
        {
            var usermovies = await _dbContext.Purchases.Where(p => p.UserId == userId).Include(p => p.Movie).ToListAsync();
            return usermovies;
        }
    }
}
