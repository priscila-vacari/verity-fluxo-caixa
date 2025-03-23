using FluxoCaixa.Infra.Context;
using FluxoCaixa.Infra.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;

namespace FluxoCaixa.Infra.Repositories
{
    public class Repository<T>(AppDbContext context, ILogger<Repository<T>> logger) : IRepository<T> where T : class
    {
        private readonly AppDbContext _context = context;
        private readonly ILogger<Repository<T>> _logger = logger;
        private readonly DbSet<T> _dbSet = context.Set<T>();

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            _logger.LogInformation("Pesquisando todos os registros.");
            return await _dbSet.ToListAsync();
        }

        [ExcludeFromCodeCoverage]
        public async Task<T?> GetByIdAsync(int id)
        {
            _logger.LogInformation("Pesquisando o registro de id {id}.", id);
            return await _dbSet.FindAsync(id);
        }

        public async Task<T?> GetByKeysAsync(params object[] keys)
        {
            return await _dbSet.FindAsync(keys);
        }

        [ExcludeFromCodeCoverage]
        public async Task<IEnumerable<T>> GetWhereAsync(Expression<Func<T, bool>> predicate)
        {
            return await _dbSet.Where(predicate).ToListAsync();
        }

        public async Task AddAsync(T entity)
        {
            _logger.LogInformation("Inserindo o registro.");
            await _dbSet.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        [ExcludeFromCodeCoverage]
        public async Task UpdateAsync(T entity)
        {
            _logger.LogInformation("Atualizando o registro.");
            _dbSet.Update(entity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(params object[] keys)
        {
            _logger.LogInformation("Excluindo o registro id {keys}.", keys);
            var entity = await GetByKeysAsync(keys);
            if (entity != null)
            {
                _dbSet.Remove(entity);
                await _context.SaveChangesAsync();
            }
        }
    }
}
