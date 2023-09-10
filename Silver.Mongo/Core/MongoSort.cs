using MongoDB.Driver;
using System;
using System.Linq.Expressions;

namespace Silver.Mongo.Core
{
    public static class MongoSort
    {

        public static SortDefinition<T> CreateSort<T>()
        {
            return null;
        }

        /// <summary>
        /// 正序
        /// </summary>
        /// <param name="field"></param>
        public static SortDefinition<T> Asc<T>(this SortDefinition<T> definition, Expression<Func<T, object>> field)
        {
            SortDefinition<T> Sorts;
            if (definition == null)
            {
                Sorts = Builders<T>.Sort.Ascending(field);
            }
            else
            {
                Sorts = Builders<T>.Sort.Combine(definition, Builders<T>.Sort.Ascending(field));
            }
            return Sorts;
        }

        /// <summary>
        /// 倒序
        /// </summary>
        /// <param name="field"></param>
        public static SortDefinition<T> Desc<T>(this SortDefinition<T> definition, Expression<Func<T, object>> field)
        {
            if (definition == null)
            {
                definition = Builders<T>.Sort.Descending(field);
            }
            else
            {
                definition = Builders<T>.Sort.Combine(definition, Builders<T>.Sort.Descending(field));
            }
            return definition;
        }

    }
}
