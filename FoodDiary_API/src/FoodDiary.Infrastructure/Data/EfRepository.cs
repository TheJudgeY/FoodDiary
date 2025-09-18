using Ardalis.Result;
using FoodDiary.Core.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FoodDiary.Infrastructure.Data;

public class EfRepository<T> : FoodDiary.Core.Interfaces.IRepository<T> where T : class
{
    private readonly AppDbContext _dbContext;
    private readonly DbSet<T> _dbSet;

    public EfRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
        _dbSet = dbContext.Set<T>();
    }

    public async Task<Result<T>> GetByIdAsync(Guid id, params string[] includeProperties)
    {
        try
        {
            var query = _dbSet.AsQueryable();
            
            foreach (var includeProperty in includeProperties)
            {
                query = query.Include(includeProperty);
            }
            
            var entity = await query.FirstOrDefaultAsync(e => EF.Property<Guid>(e, "Id") == id);
            if (entity == null)
                return Result.NotFound();
            return Result.Success(entity);
        }
        catch (Exception ex)
        {
            return Result.Error(ex.Message);
        }
    }

    public async Task<Result<List<T>>> ListAsync(params string[] includeProperties)
    {
        try
        {
            var query = _dbSet.AsQueryable();
            
            foreach (var includeProperty in includeProperties)
            {
                query = query.Include(includeProperty);
            }
            
            var list = await query.ToListAsync();
            return Result.Success(list);
        }
        catch (Exception ex)
        {
            return Result.Error(ex.Message);
        }
    }

    public async Task<Result<T>> AddAsync(T entity)
    {
        try
        {
            await _dbSet.AddAsync(entity);
            await _dbContext.SaveChangesAsync();
            return Result.Success(entity);
        }
        catch (Exception ex)
        {
            return Result.Error(ex.Message);
        }
    }

    public async Task<Result> UpdateAsync(T entity)
    {
        try
        {
            _dbSet.Update(entity);
            await _dbContext.SaveChangesAsync();
            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Error(ex.Message);
        }
    }

    public async Task<Result> DeleteAsync(Guid id)
    {
        try
        {
            var entity = await _dbSet.FindAsync(id);
            if (entity == null)
                return Result.NotFound();
            _dbSet.Remove(entity);
            await _dbContext.SaveChangesAsync();
            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Error(ex.Message);
        }
    }
}
