using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using BlogApi.Data.Context;
using BlogApi.Data.Repository.Interfaces;
using BlogApi.Shared.Models;
using BlogApi.Shared.Enums;

namespace BlogApi.Data.Repository;

public abstract class Repository<TEntity>(BlogDbContext context) : IRepository<TEntity>
    where TEntity : class
{
    public virtual async Task<Result<TEntity>> GetByIdAsync(string Id)
    {
        try
        {
            var data = await context.Set<TEntity>().FindAsync(Id);
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
            var data = await context.Set<TEntity>().ToListAsync();
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
            var data = await context.Set<TEntity>().Where(predicate).ToListAsync();
            return Result<IEnumerable<TEntity>>.Success(data);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return Result<IEnumerable<TEntity>>.Failure(ERROR_CODE.UNKNOWN_ERROR, "Error al obtener los registros");
        }
    }

    public virtual async Task<Result<TEntity>> FindFirstAsync(Expression<Func<TEntity, bool>> predicate)
    {
        try
        {
            var data = await context.Set<TEntity>().FirstOrDefaultAsync(predicate);
            return data == null
                ? Result<TEntity>.Failure(ERROR_CODE.RECORD_NOT_FOUND, "No se encontró el registro")
                : Result<TEntity>.Success(data);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return Result<TEntity>.Failure(ERROR_CODE.UNKNOWN_ERROR, "Error al obtener el registro");
        }
    }

    public virtual async Task<Result<TEntity>> AddAsync(TEntity entity)
    {
        try
        {
            var result = await context.Set<TEntity>().AddAsync(entity);
            var changeResult = await context.SaveChangesAsync();
            return changeResult == 0
                ? Result<TEntity>.Failure(ERROR_CODE.UNKNOWN_ERROR, "Error al crear el registro")
                : Result<TEntity>.Success(result.Entity);
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
            var data = context.Set<TEntity>().Update(entity);
            await context.SaveChangesAsync();
            return Result<TEntity>.Success(data.Entity);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return Result<TEntity>.Failure(ERROR_CODE.UNKNOWN_ERROR, "Error al actualizar el registro");
        }
    }

    public virtual async Task<Result<TEntity>> DeleteAsync(TEntity entity)
    {
        try
        {
            var data = context.Set<TEntity>().Remove(entity);
            var countChange = await context.SaveChangesAsync();
            if (countChange == 0) throw new Exception("Error al eliminar el registro");
            return Result<TEntity>.Success(data.Entity);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return Result<TEntity>.Failure(ERROR_CODE.UNKNOWN_ERROR, "Error al eliminar el registro");
        }
    }


    public virtual async Task<Result<TEntity>> AddOrUpdateAsync(Expression<Func<TEntity, bool>> predicate,
        TEntity entity)
    {
        try
        {
            var result = await FindFirstAsync(predicate);
            if (!result.IsSuccess) return await AddAsync(entity);

            context.Entry(result.Data).CurrentValues.SetValues(entity);
            var changes = await context.SaveChangesAsync();
            if (changes == 0) throw new Exception("Error al agregar o actualizar el registro");
            return Result<TEntity>.Success(result.Data);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return Result<TEntity>.Failure(ERROR_CODE.UNKNOWN_ERROR, "Error al agregar o actualizar el registro");
        }
    }
}