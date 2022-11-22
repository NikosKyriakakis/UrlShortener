using MongoDB.Driver;
using Repository.Pattern.Generic;
using System.Linq.Expressions;

namespace Repository.Pattern.MongoDB
{
    public class MongoRepository<TEntity> : IRepository<TEntity> where TEntity : IEntity
    {
        private readonly IMongoCollection<TEntity> _database;
        private readonly FilterDefinitionBuilder<TEntity> _filterBuilder = Builders<TEntity>.Filter;

        public MongoRepository(IMongoDatabase database, string collectionName)
        {
            _database = database.GetCollection<TEntity>(collectionName);
        }

        public async Task<IReadOnlyCollection<TEntity>> GetAllAsync()
        {
            return await _database.Find(_filterBuilder.Empty).ToListAsync();
        }

        public async Task<IReadOnlyCollection<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>> filter)
        {
            return await _database.Find(filter).ToListAsync();
        }

        public async Task<TEntity> GetByIdAsync(Guid id)
        {
            FilterDefinition<TEntity> filter = _filterBuilder.Eq(entity => entity.Id, id);

            return await _database.Find(filter).FirstOrDefaultAsync();
        }

        public async Task<TEntity> GetByIdAsync(Guid id, Expression<Func<TEntity, bool>> filter)
        {
            return await _database.Find(filter).FirstOrDefaultAsync();
        }

        public async Task PostAsync(TEntity entity)
        {
            if (entity is null)
                throw new ArgumentNullException(nameof(entity));

            await _database.InsertOneAsync(entity);
        }

        public async Task UpdateAsync(TEntity entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            FilterDefinition<TEntity> filter = _filterBuilder.Eq(e => e.Id, entity.Id);
            await _database.ReplaceOneAsync(filter, entity);
        }

        public async Task DeleteAsync(Guid id)
        {
            FilterDefinition<TEntity> filter = _filterBuilder.Eq(e => e.Id, id);

            await _database.DeleteOneAsync(filter);
        }
    }
}
