using System.Linq.Expressions;
using System.Reflection;
using System.Security.Principal;
using rest_api_dotnet_template.Models;
using Microsoft.EntityFrameworkCore;

namespace rest_api_dotnet_template.Repositories
{
    public interface IRepository<TEntity> where TEntity : BaseEntity
    {
        Task<PagedResultDto<TEntity>> GetAllAsync(int page = 1, int pageSize = 10,
                                  Expression<Func<TEntity, bool>> whereClause = null,
                                  params Expression<Func<TEntity, object>>[] includes);
        Task<TEntity> GetByIdAsync(int id, Expression<Func<TEntity, bool>> whereClause = null,
                                  params Expression<Func<TEntity, object>>[] includes);
        Task AddAsync(TEntity entity);
        Task UpdateAsync(TEntity entity);
        Task DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
    }

    public class Repository<TEntity, TContext> : IRepository<TEntity>
            where TEntity : BaseEntity
            where TContext : DbContext
    {
        protected readonly TContext _context;

        public Repository(TContext context)
        {
            _context = context;
        }

        public virtual async Task<PagedResultDto<TEntity>> GetAllAsync(int page = 1, int pageSize = 10,
                                               Expression<Func<TEntity, bool>> whereClause = null,
                                               params Expression<Func<TEntity, object>>[] includes)
        {
            try
            {
                var query = _context.Set<TEntity>().AsQueryable();

                if (whereClause != null)
                {
                    query = query.Where(whereClause);
                }

                foreach (var include in includes)
                {
                    query = query.Include(include);
                }

                var totalCount = await query.CountAsync();
                var entities = await query
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                return new PagedResultDto<TEntity>
                {
                    Items = entities,
                    CurrentPage = page,
                    PageSize = pageSize,
                    TotalCount = totalCount
                };
            }
            catch (Exception ex)
            {
                throw new Exception("Falha ao recuperar os dados.", ex);
            }
        }

        public virtual async Task<TEntity> GetByIdAsync(int id, Expression<Func<TEntity, bool>> whereClause = null,
                                  params Expression<Func<TEntity, object>>[] includes)
        {
            if (id <= 0)
                throw new ArgumentException("O ID deve ser maior que 0.");

            try
            {
                var query = _context.Set<TEntity>().AsQueryable();

                if (whereClause != null)
                {
                    query = query.Where(whereClause);
                }

                foreach (var include in includes)
                {
                    query = query.Include(include);
                }

                var entity = await query.FirstOrDefaultAsync(e => e.Id == id)
                    ?? throw new KeyNotFoundException($"Entidade do tipo {typeof(TEntity).Name} com ID {id} não encontrada.");

                return entity;
            }
            catch (KeyNotFoundException ex)
            {
                throw new KeyNotFoundException(ex.Message, ex);
            }
            catch (Exception ex)
            {
                throw new Exception($"Falha ao recuperar a entidade com ID {id}.", ex);
            }
        }

        public virtual async Task AddAsync(TEntity entity)
        {
            try
            {
                await _context.Set<TEntity>().AddAsync(entity);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Falha ao adicionar a entidade.", ex);
            }
        }

        public virtual async Task UpdateAsync(TEntity entity)
        {
            try
            {
                _context.Set<TEntity>().Update(entity);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Falha ao atualizar a entidade com ID {entity.Id}.", ex);
            }
        }

        public virtual async Task DeleteAsync(int id)
        {
            try
            {
                var entity = await GetByIdAsync(id);
                _context.Set<TEntity>().Remove(entity);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Falha ao excluir a entidade com ID {id}.", ex);
            }
        }

        public virtual async Task<bool> ExistsAsync(int id)
        {
            if (id <= 0)
                return false;

            try
            {
                return await _context.Set<TEntity>().AnyAsync(e => e.Id == id);
            }
            catch (Exception ex)
            {
                throw new Exception($"Falha ao verificar a existência da entidade com ID {id}.", ex);
            }
        }
    }
}
