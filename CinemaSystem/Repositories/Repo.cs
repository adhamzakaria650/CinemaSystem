using CinemaSystem.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Collections;
using System.Linq.Expressions;

namespace CinemaSystem.Repositories
{
    public class Repo<T> : IRepo<T> where T : class
    {
        ApplicationDbContext _context; //= new ApplicationDbContext();
        public DbSet<T> _dbset;
        public Repo(ApplicationDbContext context)
        {
            _context = context;
            _dbset = _context.Set<T>();
        }
        public async Task<EntityEntry<T>> AddAsync(T Entity)
        {
            return await _dbset.AddAsync(Entity);
        }
        public EntityEntry<T> Update(T Entity)
        {
            return _dbset.Update(Entity);
        }
        public EntityEntry<T> Remove(T Entity)
        {
            return _dbset.Remove(Entity);
        }
        public async Task<int> CommitAsync()
        {
            return await _context.SaveChangesAsync();
        }
        public  async Task<T> GetOneAsync(
           Expression<Func<T, bool>>? Filter = null,
           Expression<Func<T, object>>[]? Includes = null,
           bool isTracked = true
           )
        {
            var Entities = _dbset.AsQueryable();
            if(Filter!=null)
                Entities = Entities.Where(Filter);
            if (Includes != null)
            {
                foreach(var include in Includes)
                {
                    Entities = Entities.Include(include);
                }
            }
            if(isTracked == false)
                Entities = Entities.AsNoTracking();
            return await Entities.FirstOrDefaultAsync();
        }
        public IEnumerable<T> GetAll(
            Expression<Func<T, bool>>? Filter = null,
            Expression<Func<T, object>>[]? Includes = null,
            bool isTracked = true,
            int Skip = -1,
            int Take = -1
           )
        {
            var Entities = _dbset.AsQueryable();
            if (Filter != null)
                Entities = Entities.Where(Filter);
            if (Includes != null)
            {
                foreach (var include in Includes)
                {
                    Entities = Entities.Include(include);
                }
            }
            if (isTracked == false)
                Entities = Entities.AsNoTracking();
           
            if(Skip>=0&& Take >= 1)
            {
                Entities = Entities.Skip(Skip).Take(Take);
            }
            
            return Entities.AsEnumerable();
        }
    }
}