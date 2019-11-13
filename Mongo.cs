using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB;
using MongoDB.Driver;
using MongoDB.Bson;

namespace MongoDBConnector
{
    public static class Mongo
    {
        private static MongoClient Client { get; set; }
        private static IMongoDatabase Db { get; set; }

        public static void Initialize(JsonConfigurator configurator)
        {
            try
            {
                Client = new MongoClient(configurator.ConnectionString);
                Db = Client.GetDatabase(configurator.DatabaseName);
                _ = Db.ListCollectionNames();
            }
            catch (Exception)
            {
                throw;
            }
        }

        private static IMongoCollection<T> GetCollection<T>() where T : Entity
        {
            return Db.GetCollection<T>(typeof(T).Name.NormalizeCollectionName());
        }

        public static IAsyncCursor<T> Get<T>(FilterDefinition<T> filter = null) where T : Entity
        {
            var collection = Db.GetCollection<T>(typeof(T).Name.NormalizeCollectionName());

            if (filter == null)
                return collection.FindAsync(FilterDefinition<T>.Empty).Result;
            else
                return collection.FindAsync(filter).Result;
        }

        public static T GetOne<T>(FilterDefinition<T> filter = null) where T : Entity
        {
            var collection = Db.GetCollection<T>(typeof(T).Name.NormalizeCollectionName());

            if (filter == null)
                return collection.FindAsync(FilterDefinition<T>.Empty).Result.SingleOrDefault();
            else
                return collection.FindAsync(filter).Result.SingleOrDefault();
        }

        public static T GetOneById<T>(ObjectId id) where T : Entity
        {
            var collection = Db.GetCollection<T>(typeof(T).Name.NormalizeCollectionName());

            return collection.FindAsync(Builders<T>.Filter.Eq(x => x.Id, id)).Result.SingleOrDefault();
        }

        public static IAsyncCursor<T> Get<T>(ObjectId id) where T : Entity
        {
            var collection = Db.GetCollection<T>(typeof(T).Name.NormalizeCollectionName());

            var filter = Builders<T>.Filter.Eq(x => x.Id, id);
            return collection.FindAsync(filter).Result;
        }

        public static long GetCount<T>(FilterDefinition<T> filter = null) where T : Entity
        {
            var collection = Db.GetCollection<T>(typeof(T).Name.NormalizeCollectionName());

            if (filter == null)
                return collection.CountDocumentsAsync(FilterDefinition<T>.Empty).Result;
            else
                return collection.CountDocumentsAsync(filter).Result;
        }

        public static bool Exists<T>(ObjectId id) where T : Entity
        {
            var collection = Db.GetCollection<T>(typeof(T).Name.NormalizeCollectionName());

            var filter = Builders<T>.Filter.Eq(x => x.Id, id);
            return collection.FindAsync(filter).Result.Any();
        }

        public static bool InsertOne<T>(T entity) where T : Entity
        {
            try
            {
                entity.CreatedDate = DateTime.Now;
                entity.ModifiedDate = DateTime.Now;
                GetCollection<T>().InsertOneAsync(entity).Wait();

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static void InsertMany<T>(params T[] entities) where T : Entity
        {
            foreach (var entity in entities)
            {
                entity.ModifiedDate = DateTime.Now;
                entity.CreatedDate = DateTime.Now;
            }

            GetCollection<T>().InsertManyAsync(entities).Wait();
        }

        public static void DeleteOne<T>(FilterDefinition<T> filter) where T : Entity
        {
            GetCollection<T>().DeleteOneAsync(filter).Wait();
        }

        public static bool DeleteOne<T>(T entity) where T : Entity
        {
            try
            {
                var filter = Builders<T>.Filter.Eq(x => x.Id, entity.Id);
                GetCollection<T>().DeleteOneAsync(filter).Wait();

                return !GetCollection<T>().FindAsync(filter).Result.Any();
            }
            catch (Exception)
            {

                return false;
            }
        }

        public static bool DeleteMany<T>(FilterDefinition<T> filter) where T : Entity
        {
            GetCollection<T>().DeleteManyAsync(filter).Wait();

            return !GetCollection<T>().FindAsync(filter).Result.Any();
        }

        public static bool DeleteMany<T>(IEnumerable<T> entities) where T : Entity
        {
            var filter = Builders<T>.Filter.In(x => x, entities);
            GetCollection<T>().DeleteManyAsync(filter).Wait();

            return !GetCollection<T>().FindAsync(filter).Result.Any();
        }

        public static void UpdateOne<T>(FilterDefinition<T> filter, T entity) where T : Entity
        {
            entity.ModifiedDate = DateTime.Now;
            var options = new UpdateOptions { IsUpsert = true };
            GetCollection<T>().ReplaceOneAsync(filter, entity, options).Wait();
        }

        public static void UpdateOne<T>(T entity) where T : Entity
        {
            entity.ModifiedDate = DateTime.Now;
            var options = new UpdateOptions { IsUpsert = true };
            var filter = Builders<T>.Filter.Eq(x => x.Id, entity.Id);
            GetCollection<T>().ReplaceOneAsync(filter, entity, options).Wait();
        }

        public static bool UpdateField<T>(FilterDefinition<T> filter, UpdateDefinition<T> update) where T : Entity
        {
            return GetCollection<T>().UpdateOneAsync(filter, update).Result.IsAcknowledged;
        }
    }
}
