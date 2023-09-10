using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Silver.Cache
{
    /// <summary>
    /// 实体内存库
    /// </summary>
    public class RealMemoryUtil
    {

        private static Dictionary<string, object> shareValues = new Dictionary<string, object>();

        /// <summary>
        /// 注册数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        public static void Register<T>(List<T> list)
        {
            Register<T>(typeof(T).Name, list);
        }

        /// <summary>
        /// 注册数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="keyname"></param>
        /// <param name="list"></param>
        public static void Register<T>(string keyname, List<T> list)
        {
            shareValues[keyname] = list;
        }

        #region 操作

        /// <summary>
        /// 查询
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="expression"></param>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <returns></returns>
        public static (List<T>,int) QueryableAndCount<T>(Expression<Func<T, bool>> expression, int page = 1, int rows = int.MaxValue)
        {
            string typeName = typeof(T).Name;
            return QueryableAndCount<T>(typeName, expression, page, rows);
        }

        /// <summary>
        /// 查询
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="keyname"></param>
        /// <param name="expression"></param>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <returns></returns>
        public static (List<T>, int) QueryableAndCount<T>(string keyname, Expression<Func<T, bool>> expression, int page = 1, int rows = int.MaxValue)
        {
            string typeName = keyname;
            object value = new object();
            if (shareValues.TryGetValue(typeName, out value))
            {
                var list = ((List<T>)value).Where(expression.Compile());
                return (list.Skip((page - 1) * rows).Take(rows).ToList(), list.Count());
            }
            return (new List<T>(), 0);
        }

        /// <summary>
        /// 查询
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="expression"></param>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <returns></returns>
        public static List<T> Queryable<T>(Expression<Func<T, bool>> expression, int page = 1, int rows = int.MaxValue)
        {
            string typeName = typeof(T).Name;
            return Queryable<T>(typeName, expression, page, rows);
        }

        /// <summary>
        /// 查询
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="keyname"></param>
        /// <param name="expression"></param>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <returns></returns>
        public static List<T> Queryable<T>(string keyname, Expression<Func<T, bool>> expression, int page = 1, int rows = int.MaxValue)
        {
            string typeName = keyname;
            object value = new object();
            if (shareValues.TryGetValue(typeName, out value))
            { 
                return ((List<T>)value).Where(expression.Compile()).Skip((page - 1) * rows).Take(rows).ToList();
            }
            return new List<T>();
        }


        /// <summary>
        /// 条数
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="expression"></param>
        /// <returns></returns>
        public static int Count<T>(Expression<Func<T, bool>> expression)
        {
            return Count<T>(typeof(T).Name, expression);
        }

        /// <summary>
        /// 条数
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="keyname"></param>
        /// <param name="expression"></param>
        /// <returns></returns>
        public static int Count<T>(string keyname, Expression<Func<T, bool>> expression)
        {
            string typeName = keyname;
            object value = new object();
            if (shareValues.TryGetValue(typeName, out value))
            {
                return ((List<T>)value).Where(expression.Compile()).Count();
            }
            return 0;
        }

        /// <summary>
        /// 插入数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        public static void Insertable<T>(T value)
        {
            string typeName = typeof(T).Name;
            Insertable<T>(typeName, value);
        }

        /// <summary>
        /// 插入数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="keyname"></param>
        /// <param name="value"></param>
        public static void Insertable<T>(string keyname, T value)
        {
            string typeName = keyname;
            var list = new List<T>();
            object oldvalue = new object();
            if (shareValues.TryGetValue(typeName, out oldvalue))
            {
                list = ((List<T>)oldvalue);
                list.Add(value);
                shareValues[typeName] = list;
            }
            else
            {
                list.Add(value);
                shareValues[typeName] = list;
            }
        }

        /// <summary>
        /// 插入数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        public static void Insertable<T>(List<T> value)
        {
            string typeName = typeof(T).Name;
            Insertable<T>(typeName, value);
        }

        /// <summary>
        /// 插入数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="keyname"></param>
        /// <param name="value"></param>
        public static void Insertable<T>(string keyname, List<T> value)
        {
            string typeName = keyname;
            var list = new List<T>();
            if (shareValues.ContainsKey(typeName) == false)
            {
                list.AddRange(value);
                shareValues[typeName] = list;
            }
            else
            {
                list = (List<T>)shareValues[typeName];
                list.AddRange(value);
                shareValues[typeName] = list;
            }
        }

        /// <summary>
        /// 更新数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        public static void Updateable<T>(T value)
        {
            string typeName = typeof(T).Name;
            Updateable<T>(typeName, value);
        }

        /// <summary>
        /// 更新数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="keyname"></param>
        /// <param name="value"></param>
        public static void Updateable<T>(string keyname, T value)
        {
            string typeName = keyname;
            if (shareValues.ContainsKey(typeName) == false)
            {
                return;
            }
            int i = 0;
            string keyName = "";
            string keyValue = "";
            PropertyInfo[] listPropperty = typeof(T).GetProperties();
            foreach (var item in listPropperty)
            {
                AttributeCollection attributes = TypeDescriptor.GetProperties(item.GetType())[i].Attributes;
                if (((System.ComponentModel.BrowsableAttribute)(attributes[typeof(BrowsableAttribute)])).Browsable != false)
                {
                    keyName = item.Name;
                    keyValue = value.GetType().GetProperty(item.Name).GetValue(value, null).ToString();
                    break;
                }
                i++;
            }
            var list = (List<T>)shareValues[typeName];
            for (int s = 0; s < list.Count; s++)
            {
                string childvalue = list[s].GetType().GetProperty(keyName).GetValue(list[s], null).ToString();
                if (childvalue == keyValue)
                {
                    list[s] = value;
                    break;
                }
            }
        }

        /// <summary>
        /// 主键获取详情
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="id"></param>
        /// <returns></returns>
        public static T GetByInfo<T>(object id)
        {
            return GetByInfo<T>(typeof(T).Name, id);
        }

        /// <summary>
        /// 主键获取详情
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="keyname"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public static T GetByInfo<T>(string keyname, object id)
        {
            int i = 0;
            string keyName = "";
            string typeName = keyname;
            if (shareValues.ContainsKey(typeName) == false)
            {
                return default;
            }
            PropertyInfo[] listPropperty = typeof(T).GetProperties();
            foreach (var item in listPropperty)
            {
                AttributeCollection attributes = TypeDescriptor.GetProperties(item.GetType())[i].Attributes;
                if (((System.ComponentModel.BrowsableAttribute)(attributes[typeof(BrowsableAttribute)])).Browsable != false)
                {
                    keyName = item.Name;
                    break;
                }
                i++;
            }
            int index = -1;
            var list = (List<T>)shareValues[typeName];
            for (int s = 0; s < list.Count; s++)
            {
                string childvalue = list[s].GetType().GetProperty(keyName).GetValue(list[s], null).ToString();
                if (childvalue == id.ToString())
                {
                    index = s;
                    break;
                }
            }
            if (index < 0)
            {
                return default;
            }
            return list[index];
        }

        /// <summary>
        /// 根据查询条件获取详情
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="expression"></param>
        /// <returns></returns>
        public static T GetSingle<T>(Expression<Func<T, bool>> expression)
        {
            return GetSingle<T>(typeof(T).Name, expression);
        }

        /// <summary>
        /// 根据查询条件获取详情
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="keyname"></param>
        /// <param name="expression"></param>
        /// <returns></returns>
        public static T GetSingle<T>(string keyname, Expression<Func<T, bool>> expression)
        {
            if (shareValues.ContainsKey(keyname) == false)
            {
                return default;
            }
            var list_not = RealMemoryUtil.Queryable<T>(expression).ToList();
            if (list_not.Count <= 0)
            {
                return default;
            }
            return ((List<T>)shareValues[keyname]).Where(expression.Compile()).FirstOrDefault();
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="expression"></param>
        public static void Deleteable<T>(Expression<Func<T, bool>> expression)
        {
            string typeName = typeof(T).Name;
            Deleteable<T>(typeName, expression);
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="expression"></param>
        public static void Deleteable<T>(string keyname, Expression<Func<T, bool>> expression)
        {
            string typeName = keyname;
            if (shareValues.ContainsKey(typeName) == false)
            {
                return;
            }
            var list_not = RealMemoryUtil.Queryable<T>(expression).ToList();
            if (list_not.Count <= 0)
            {
                return;
            }
            shareValues[typeName] = ((List<T>)shareValues[typeName]).RemoveAll(t => list_not.Contains(t));
        }

        /// <summary>
        /// 删除key
        /// </summary>
        /// <param name="keyname"></param>
        public static void Removeable(string keyname)
        {
            if (shareValues.ContainsKey(keyname) == false)
            {
                return;
            }
            shareValues.Remove(keyname);
        }

        /// <summary>
        /// 清空列表
        /// </summary>
        public static void RemoveAllable()
        {
            shareValues.Clear();
        }

        #endregion

    }
}
