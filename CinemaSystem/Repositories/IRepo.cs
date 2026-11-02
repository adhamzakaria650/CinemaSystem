using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Linq.Expressions;

namespace CinemaSystem.Repositories
{
    public interface IRepo<T> where T : class 
    {
        Task<EntityEntry<T>> AddAsync(T Entity);
        EntityEntry<T> Update(T Entity);
        EntityEntry<T> Remove(T Entity);
        Task<int> CommitAsync();
        Task<T> GetOneAsync(
            Expression<Func<T, bool>>? Filter = null,
            Expression<Func<T, object>>[]? Includes =null,
            bool isTracked = false
            );
        IEnumerable<T> GetAll(
            Expression<Func<T, bool>>? Filter = null,
            Expression<Func<T, object>>[]? Includes =null,
            bool isTracked = false,
            int Skip=-1,
            int Take = -1
            );
    }
}
