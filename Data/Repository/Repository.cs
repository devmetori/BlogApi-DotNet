using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using BlogApi.Data.Context;
using BlogApi.Data.Repository.Interfaces;
using BlogApi.Shared.Models;
using BlogApi.Shared.Enums;

namespace BlogApi.Data.Repository;

public abstract class Repository<TEntity> : IRepository<TEntity> where TEntity : class
{
    protected readonly BlogDbContext _context;
    protected readonly DbSet<TEntity> _dbSet;

    public Repository(BlogDbContext context)
    {
        _context = context;
        _dbSet = context.Set<TEntity>();
    }

    public virtual async Task<Result<TEntity>> GetByIdAsync(string id)
    {
        try
        {
            var data = await _dbSet.FindAsync(id);
            if (data == null) Result<TEntity>.Failure(ERROR_CODE.RECORD_NOT_FOUND, "Registro no encontrado");
            return Result<TEntity>.Success(data);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return Result<TEntity>.Failure(ERROR_CODE.UNKNOWN_ERROR, "Error al obtener el registro");
        }
    }

   
    public virtual async Task<Result<IEnumerable<TEntity>>> GetAllAsync()
    {
        try
        {
            var data = await _dbSet.ToListAsync();
            if (data == null)
                Result<IEnumerable<TEntity>>.Failure(ERROR_CODE.RECORD_NOT_FOUND, "No se encontraron registros");
            return Result<IEnumerable<TEntity>>.Success(data);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return Result<IEnumerable<TEntity>>.Failure(ERROR_CODE.UNKNOWN_ERROR, "Error al obtener los registros");
        }
    }

    public virtual async Task<Result<IEnumerable<TEntity>>> FindAsync(Expression<Func<TEntity, bool>> predicate)
    {
        try
        {
            var data = await _dbSet.Where(predicate).ToListAsync();
            if (data == null)
                Result<IEnumerable<TEntity>>.Failure(ERROR_CODE.RECORD_NOT_FOUND, "No se encontraron registros");
            return Result<IEnumerable<TEntity>>.Success(data);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return Result<IEnumerable<TEntity>>.Failure(ERROR_CODE.UNKNOWN_ERROR, "Error al obtener los registros");
        }
    }

    public virtual async Task<Result<TEntity>> AddAsync(TEntity entity)
    {
        try
        {
            var data = await _dbSet.AddAsync(entity);
            await _context.SaveChangesAsync();
            if (data == null) Result<TEntity>.Failure(ERROR_CODE.GENERAL_API_ERROR, "Error al crear el registro");
            return Result<TEntity>.Success(data.Entity);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return Result<TEntity>.Failure(ERROR_CODE.UNKNOWN_ERROR, "Error al crear el registro");
        }
    }

    public virtual async Task<Result<TEntity>> UpdateAsync(TEntity entity)
    {
        try
        {
            var data = _dbSet.Update(entity);
            await _context.SaveChangesAsync();
            if (data == null) Result<TEntity>.Failure(ERROR_CODE.GENERAL_API_ERROR, "Error al actualizar el registro");
            return Result<TEntity>.Success(data.Entity);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return Result<TEntity>.Failure(ERROR_CODE.UNKNOWN_ERROR, "Error al actualizar el registro");
        }
    }
    
    public virtual async Task<Result<TEntity>> DeleteAsync(string id)
    {
        try
        {
            var result = await GetByIdAsync(id);
            if (!result.IsSuccess) return result;
            var data = _dbSet.Remove(result.Data);
            await _context.SaveChangesAsync();
            if (data == null) Result<TEntity>.Failure(ERROR_CODE.GENERAL_API_ERROR, "Error al eliminar el registro");
            return Result<TEntity>.Success(data.Entity);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return Result<TEntity>.Failure(ERROR_CODE.UNKNOWN_ERROR, "Error al eliminar el registro");
        }
    }
}