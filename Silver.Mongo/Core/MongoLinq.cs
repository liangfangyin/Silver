using MongoDB.Driver;

namespace Silver.Mongo.Core
{
    public class MongoLinq<T>
    {

        public FilterDefinitionBuilder<T> Builder;
        public FilterDefinition<T> Filter;

        public MongoLinq()
        {
            Builder = new FilterDefinitionBuilder<T>();
            Filter = Builders<T>.Filter.Empty;
        }

        public void And(FilterDefinition<T> definition)
        {
            Filter = Filter & Builders<T>.Filter.And(definition);
        }

    }
}
