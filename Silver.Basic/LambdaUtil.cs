using Silver.Basic.Model.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Silver.Basic
{
    public static class LambdaUtil
    {

        /// <summary>
        /// 返回符合条件的一条
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public static T FirstOrDefaultValue<T>(this List<T> list, Func<T, bool> predicate = null)
        {
            if (list == null)
            {
                return default(T);
            }
            if (predicate == null)
            {
                return list.FirstOrDefault();
            }
            return list.Where(predicate).FirstOrDefault();
        }

        /// <summary>
        /// 返回符合条件的一条,无值new返回
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public static T SingleOrDefaultValue<T>(this List<T> list, Func<T, bool> predicate = null) where T : class, new()
        {
            var infoDefault = new T();
            if (list == null)
            {
                return infoDefault;
            }
            if (list.Count <= 0)
            {
                return infoDefault;
            }
            if (predicate == null)
            {
                infoDefault = list.FirstOrDefault();
            }
            infoDefault = list.Where(predicate).FirstOrDefault();
            if (infoDefault == null)
            {
                infoDefault = new T();
            }
            return infoDefault;
        }

        /// <summary>
        /// 分页查询
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="queryable"></param>
        /// <param name="page"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        public static IQueryable<T> TakePage<T>(this IQueryable<T> queryable,   int page=1, int size = 15)
        { 
            return queryable.TakeOrderByPage<T>(page, size);
        }

        /// <summary>
        /// 分页查询
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="queryable"></param>
        /// <param name="page"></param>
        /// <param name="size"></param>
        /// <param name="orderBy"></param>
        /// <returns></returns>
        public static IQueryable<T> TakeOrderByPage<T>(this IQueryable<T> queryable, int page=1, int size = 15, Expression<Func<T, Dictionary<object, QueryOrderBy>>> orderBy = null)
        {
            if (page <= 0)
            {
                page = 1;
            }
            return Queryable.Take(Queryable.Skip(queryable.GetIQueryableOrderBy(orderBy.GetExpressionToDic()), (page - 1) * size), size);
        }


        /// <summary>
        /// 解析多字段排序
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="queryable"></param>
        /// <param name="orderBySelector">string=排序的字段,bool=true降序/false升序</param>
        /// <returns></returns>
        public static IQueryable<TEntity> GetIQueryableOrderBy<TEntity>(this IQueryable<TEntity> queryable, Dictionary<string, QueryOrderBy> orderBySelector)
        {
            string[] orderByKeys = orderBySelector.Select(x => x.Key).ToArray();
            if (orderByKeys == null || orderByKeys.Length == 0) return queryable;

            IOrderedQueryable<TEntity> queryableOrderBy = null;
            //  string orderByKey = orderByKeys[^1];
            string orderByKey = orderByKeys[orderByKeys.Length - 1];
            queryableOrderBy = orderBySelector[orderByKey] == QueryOrderBy.Desc
                ? queryableOrderBy = queryable.OrderByDescending(orderByKey.GetExpression<TEntity>())
                : queryable.OrderBy(orderByKey.GetExpression<TEntity>());

            for (int i = orderByKeys.Length - 2; i >= 0; i--)
            {
                queryableOrderBy = orderBySelector[orderByKeys[i]] == QueryOrderBy.Desc
                    ? queryableOrderBy.ThenByDescending(orderByKeys[i].GetExpression<TEntity>())
                    : queryableOrderBy.ThenBy(orderByKeys[i].GetExpression<TEntity>());
            }
            return queryableOrderBy;
        }

        private static ParameterExpression GetExpressionParameter(this Type type)
        {
            return Expression.Parameter(type, "p");
        }


        /// <summary>
        /// 创建lambda表达式：p=>false
        /// 在已知TKey字段类型时,如动态排序OrderBy(x=>x.ID)会用到此功能,返回的就是x=>x.ID
        /// Expression<Func<Out_Scheduling, DateTime>> expression = x => x.CreateDate;指定了类型
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        private static Expression<Func<T, TKey>> GetExpression<T, TKey>(this string propertyName, ParameterExpression parameter)
        {
            if (typeof(TKey).Name == "Object")
                return Expression.Lambda<Func<T, TKey>>(Expression.Convert(Expression.Property(parameter, propertyName), typeof(object)), parameter);
            return Expression.Lambda<Func<T, TKey>>(Expression.Property(parameter, propertyName), parameter);
        }
        /// <summary>
        /// 创建lambda表达式：p=>false
        /// object不能确认字段类型(datetime,int,string),如动态排序OrderBy(x=>x.ID)会用到此功能,返回的就是x=>x.ID
        /// Expression<Func<Out_Scheduling, object>> expression = x => x.CreateDate;任意类型的字段
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        private static Expression<Func<T, object>> GetExpression<T>(this string propertyName)
        {
            return propertyName.GetExpression<T, object>(typeof(T).GetExpressionParameter());
        }


        /// <summary>
        /// 表达式转换成KeyValList(主要用于多字段排序，并且多个字段的排序规则不一样)
        /// 如有多个字段进行排序,参数格式为
        ///  Expression<Func<Out_Scheduling, Dictionary<object, QueryOrderBy>>> orderBy = x => new Dictionary<object, QueryOrderBy>() {
        ///            { x.ID, QueryOrderBy.Desc },
        ///           { x.DestWarehouseName, QueryOrderBy.Asc }
        ///      };
        ///      返回的是new Dictionary<object, QueryOrderBy>(){{}}key为排序字段，QueryOrderBy为排序方式
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="expression"></param>
        /// <returns></returns>
        private static Dictionary<string, QueryOrderBy> GetExpressionToDic<T>(this Expression<Func<T, Dictionary<object, QueryOrderBy>>> expression)
        {
            return expression.GetExpressionToPair().Reverse().ToList().ToDictionary(x => x.Key, x => x.Value);
        }

        /// <summary>
        /// 表达式转换成KeyValList(主要用于多字段排序，并且多个字段的排序规则不一样)
        /// 如有多个字段进行排序,参数格式为
        ///  Expression<Func<Out_Scheduling, Dictionary<object, bool>>> orderBy = x => new Dictionary<object, bool>() {
        ///            { x.ID, true },
        ///           { x.DestWarehouseName, true }
        ///      };
        ///      返回的是new Dictionary<object, bool>(){{}}key为排序字段，bool为升降序
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="expression"></param>
        /// <returns></returns>
        private static IEnumerable<KeyValuePair<string, QueryOrderBy>> GetExpressionToPair<T>(this Expression<Func<T, Dictionary<object, QueryOrderBy>>> expression)
        {

            foreach (var exp in ((ListInitExpression)expression.Body).Initializers)
            {
                yield return new KeyValuePair<string, QueryOrderBy>(
                exp.Arguments[0] is MemberExpression ?
                (exp.Arguments[0] as MemberExpression).Member.Name.ToString()
                : ((exp.Arguments[0] as UnaryExpression).Operand as MemberExpression).Member.Name,
                 (QueryOrderBy)((exp.Arguments[1] as ConstantExpression).Value));
            }
        }

    }
}
