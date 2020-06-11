using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;

namespace MongoDBConnector
{
    public interface IMongoPersist<T>
    {
        bool Save(T entity);
        bool Delete(ObjectId id, out T deletedEntity);
        IEnumerable<T> List();
        T GetOneById(ObjectId id);
    }
}
