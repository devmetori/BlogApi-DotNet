using System.Linq.Expressions;
using BlogApi.Models;
using BlogApi.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BlogApi.Repository;

public abstract class Repository<TEntity> : IRepository<TEntity> where TEntity : class
{
    protected readonly BlogDbContext _context;
    protected readonly DbSet<TEntity> _dbSet;

    public Repository(BlogDbContext context)
    {
        _context = context;
        _dbSet = context.Set<TEntity>();
    }

    // Obtener por ID
    public virtual async Task<TEntity> GetByIdAsync(string id)
    {
        return await _dbSet.FindAsync(id);
    }

    // Obtener todos
    public virtual async Task<IEnumerable<TEntity>> GetAllAsync()
    {
        return await _dbSet.ToListAsync();
    }

    // Encontrar mediante expresión lambda
    public virtual async Task<IEnumerable<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate)
    {
        return await _dbSet.Where(predicate).ToListAsync();
    }

    // Crear una nueva entidad
    public virtual async Task AddAsync(TEntity entity)
    {
        await _dbSet.AddAsync(entity);
        await _context.SaveChangesAsync();
    }

    // Actualizar una entidad existente
    public virtual async Task UpdateAsync(TEntity entity)
    {
        _dbSet.Update(entity);
        await _context.SaveChangesAsync();
    }

    // Eliminar una entidad
    public virtual async Task DeleteAsync(string id)
    {
        var entity = await GetByIdAsync(id);
        if (entity != null)
        {
            _dbSet.Remove(entity);
            await _context.SaveChangesAsync();
        }
    }
    
}