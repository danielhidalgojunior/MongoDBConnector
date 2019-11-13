using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MongoDBConnector
{
    public interface IReadable
    {
        IEnumerable<Entity> List();
        Entity GetOneById(ObjectId id);
    }
}
