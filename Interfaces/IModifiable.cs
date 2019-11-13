using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MongoDBConnector
{
    public interface IModifiable
    {
        bool Save();
        bool Delete(out Entity deletedEntity);
    }
}
