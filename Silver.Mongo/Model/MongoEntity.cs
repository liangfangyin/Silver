using System;
using System.Collections.Generic;
using System.Text;

namespace Silver.Mongo.Model
{
    public class MongoEntity
    {
        public object _id { get; set; } = Guid.NewGuid();
    }
}
