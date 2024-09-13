using rest_api_dotnet_template.Models;
using rest_api_dotnet_template.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace rest_api_dotnet_template.Services
{
    public interface IService<TEntity>
            where TEntity : BaseEntity
    {
        Task<PagedResultDto<TEntity>> GetAllAsync(int page, int pageSize,
                                  Expression<Func<TEntity, bool>> whereClause = null,
                                  params Expression<Func<TEntity, object>>[] includes);
        Task<TEntity> GetByIdAsync(int id,
                                   Expression<Func<TEntity, bool>> whereClause = null,
                                   params Expression<Func<TEntity, object>>[] includes);
        Task AddAsync(TEntity entity);
        Task UpdateAsync(TEntity entity);
        Task DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
    }

    public class Service<TEntity, TContext> : IService<TEntity>
            where TEntity : BaseEntity
            where TContext : DbContext
    {
        protected readonly IRepository<TEntity> _repository;

        public Service(IRepository<TEntity> repository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository), "Repositório não pode ser nulo.");
        }

        public virtual async Task<PagedResultDto<TEntity>> GetAllAsync(int page = 1, int pageSize = 10,
                                                      Expression<Func<TEntity, bool>> whereClause = null,
                                                      params Expression<Func<TEntity, object>>[] includes)
        {
            ValidatePaginationParameters(page, pageSize);

            try
            {
                return await _repository.GetAllAsync(page, pageSize, whereClause, includes);
            }
            catch (Exception ex)
            {
                throw new Exception("Ocorreu um erro ao recuperar as entidades.", ex);
            }
        }

        public virtual async Task<TEntity> GetByIdAsync(int id,
                                                        Expression<Func<TEntity, bool>> whereClause = null,
                                                        params Expression<Func<TEntity, object>>[] includes)
        {
            ValidateId(id);

            try
            {
                return await _repository.GetByIdAsync(id, whereClause, includes);
            }
            catch (KeyNotFoundException ex)
            {
                throw new KeyNotFoundException($"Entidade do tipo {typeof(TEntity).Name} com ID {id} não encontrada.", ex);
            }
            catch (Exception ex)
            {
                throw new Exception($"Ocorreu um erro ao recuperar a entidade com ID {id}.", ex);
            }
        }

        public virtual async Task AddAsync(TEntity entity)
        {
            ValidateEntity(entity);

            try
            {
                await _repository.AddAsync(entity);
            }
            catch (Exception ex)
            {
                throw new Exception("Ocorreu um erro ao adicionar a entidade.", ex);
            }
        }

        public virtual async Task UpdateAsync(TEntity entity)
        {
            ValidateEntity(entity);

            try
            {
                if (await _repository.ExistsAsync(entity.Id))
                {
                    await _repository.UpdateAsync(entity);
                }
                else
                {
                    throw new KeyNotFoundException($"Entidade do tipo {typeof(TEntity).Name} com ID {entity.Id} não encontrada.");
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Ocorreu um erro ao atualizar a entidade com ID {entity.Id}.", ex);
            }
        }

        public virtual async Task DeleteAsync(int id)
        {
            ValidateId(id);

            try
            {
                if (await _repository.ExistsAsync(id))
                {
                    await _repository.DeleteAsync(id);
                }
                else
                {
                    throw new KeyNotFoundException($"Entidade do tipo {typeof(TEntity).Name} com ID {id} não encontrada.");
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Ocorreu um erro ao excluir a entidade com ID {id}.", ex);
            }
        }

        public async Task<bool> ExistsAsync(int id)
        {
            ValidateId(id);

            try
            {
                return await _repository.ExistsAsync(id);
            }
            catch (Exception ex)
            {
                throw new Exception($"Ocorreu um erro ao verificar a existência da entidade com ID {id}.", ex);
            }
        }

        public void ValidateId(int id)
        {
            if (id <= 0)
                throw new ArgumentException("O ID deve ser maior que 0.");
        }

        public void ValidateEntity(TEntity entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity), "A entidade não pode ser nula.");
        }

        public void ValidatePaginationParameters(int page, int pageSize)
        {
            if (page < 1)
                throw new ArgumentException("O número da página deve ser maior ou igual a 1.");

            if (pageSize < 1)
                throw new ArgumentException("O tamanho da página deve ser maior ou igual a 1.");
        }

        public void ValidateFieldsToUpdate(Dictionary<string, object> fieldsToUpdate)
        {
            if (fieldsToUpdate == null || fieldsToUpdate.Count == 0)
                throw new ArgumentException("O dicionário 'fieldsToUpdate' não pode ser nulo ou vazio.");
        }
    }
}
