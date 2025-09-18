using Ardalis.Result;

namespace FoodDiary.Core.Interfaces;

public interface IRepository<T> where T : class
{
    Task<Result<T>> GetByIdAsync(Guid id, params string[] includeProperties);
    Task<Result<List<T>>> ListAsync(params string[] includeProperties);
    Task<Result<T>> AddAsync(T entity);
    Task<Result> UpdateAsync(T entity);
    Task<Result> DeleteAsync(Guid id);
} 