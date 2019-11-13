using MovieShop.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace MovieShop.Data
{
    public class GenreRepository : Repository<Genre>, IGenreRepository
    {
        public GenreRepository(MovieShopDbContext dbContext) : base(dbContext)
        {

        }

        public async Task<IEnumerable<Movie>> GetMoviesByGenre(int id)
        {   //include used for get navigation properties
            var movies = await _dbContext.MovieGenres.Where(g => g.GenreId == id).Include(m => m.Movie).ToListAsync();
            return movies.Select(m => m.Movie).ToList();//convert moviegenre type to movie type
        }
    }
}
