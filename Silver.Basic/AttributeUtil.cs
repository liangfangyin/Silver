using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Silver.Basic
{
    public static class AttributeUtil
    {

        /// <summary>
        /// 判断当前类型是否可由指定类型派生
        /// </summary>
        public static bool IsDeriveClassFrom<TBaseType>(this Type type, bool canAbstract = false)
        {
            return IsDeriveClassFrom(type, typeof(TBaseType), canAbstract);
        }

        /// <summary>
        /// 判断当前类型是否可由指定类型派生
        /// </summary>
        public static bool IsDeriveClassFrom(this Type type, Type baseType, bool canAbstract = false)
        {
            return type.IsClass && (canAbstract || !type.IsAbstract) && type.IsBaseOn(baseType);
        }

        /// <summary>
        /// 判断类型是否为Nullable类型
        /// </summary>
        /// <param name="type"> 要处理的类型 </param>
        /// <returns> 是返回True，不是返回False </returns>
        public static bool IsNullableType(this Type type)
        {
            return type != null && type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>);
        }

        /// <summary>
        /// 由类型的Nullable类型返回实际类型
        /// </summary>
        /// <param name="type"> 要处理的类型对象 </param>
        /// <returns> </returns>
        public static Type GetNonNullableType(this Type type)
        {
            return IsNullableType(type) ? type.GetGenericArguments()[0] : type;
        }

        /// <summary>
        /// 通过类型转换器获取Nullable类型的基础类型
        /// </summary>
        /// <param name="type"> 要处理的类型对象 </param>
        /// <returns> </returns>
        public static Type GetUnNullableType(this Type type)
        {
            if (IsNullableType(type))
            {
                NullableConverter nullableConverter = new NullableConverter(type);
                return nullableConverter.UnderlyingType;
            }
            return type;
        }

        /// <summary>
        /// 判断类型是否为集合类型
        /// </summary>
        /// <param name="type">要处理的类型</param>
        /// <returns>是返回True，不是返回False</returns>
        public static bool IsEnumerable(this Type type)
        {
            if (type == typeof(string))
            {
                return false;
            }
            return typeof(IEnumerable).IsAssignableFrom(type);
        }

        /// <summary>
        /// 方法是否是异步
        /// </summary>
        public static bool IsAsync(this MethodInfo method)
        {
            return method.ReturnType == typeof(Task)
                || method.ReturnType.IsGenericType && method.ReturnType.GetGenericTypeDefinition() == typeof(Task<>);
        }

        /// <summary>
        /// 返回当前类型是否是指定基类的派生类
        /// </summary>
        /// <param name="type">当前类型</param>
        /// <param name="baseType">要判断的基类型</param>
        /// <returns></returns>
        public static bool IsBaseOn(this Type type, Type baseType)
        {
            if (baseType.IsGenericTypeDefinition)
            {
                return true;
            }
            return baseType.IsAssignableFrom(type);
        }

        /// <summary>
        /// 返回当前类型是否是指定基类的派生类
        /// </summary>
        /// <typeparam name="TBaseType">要判断的基类型</typeparam>
        /// <param name="type">当前类型</param>
        /// <returns></returns>
        public static bool IsBaseOn<TBaseType>(this Type type)
        {
            Type baseType = typeof(TBaseType);
            return type.IsBaseOn(baseType);
        }

        /// <summary>
        /// 返回当前方法信息是否是重写方法
        /// </summary>
        /// <param name="method">要判断的方法信息</param>
        /// <returns>是否是重写方法</returns>
        public static bool IsOverridden(this MethodInfo method)
        {
            return method.GetBaseDefinition().DeclaringType != method.DeclaringType;
        }

        /// <summary>
        /// 返回当前属性信息是否为virtual
        /// </summary>
        public static bool IsVirtual(this PropertyInfo property)
        {
            var accessor = property.GetAccessors().FirstOrDefault();
            if (accessor == null)
            {
                return false;
            }

            return accessor.IsVirtual && !accessor.IsFinal;
        }

        /// <summary>
        /// 获取类型的全名，附带所在类库
        /// </summary>
        public static string GetFullNameWithModule(this Type type)
        {
            return $"{type.FullName},{type.Module.Name.Replace(".dll", "").Replace(".exe", "")}";
        }

        /// <summary>
        /// 获取类型的显示短名称
        /// </summary>
        public static string ShortDisplayName(this Type type)
        {
            return type.DisplayName(false);
        }

        /// <summary>
        /// 获取类型的显示名称
        /// </summary>
        public static string DisplayName(this Type type, bool fullName = true)
        {
            StringBuilder sb = new StringBuilder();
            ProcessType(sb, type, fullName);
            return sb.ToString();
        }

        /// <summary>
        /// 对象转换
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <param name="oleInfo"></param>
        /// <param name="newInfo"></param>
        /// <returns></returns>
        public static T OverPropertys<T, T2>(T oleInfo, T2 newInfo)
        {
            string ObjJson = JsonConvert.SerializeObject(oleInfo);
            T oldIf = JsonConvert.DeserializeObject<T>(ObjJson);
            Type oleType = oldIf.GetType();
            PropertyInfo[] PInfo = typeof(T).GetProperties();
            int i = 0;
            foreach (var arrInfo in PInfo)
            {
                AttributeCollection attributes = TypeDescriptor.GetProperties(oldIf.GetType())[i].Attributes;
                AttributeCollection oldattributes = TypeDescriptor.GetProperties(oleInfo.GetType())[i].Attributes;
                if (((System.ComponentModel.BrowsableAttribute)(attributes[typeof(BrowsableAttribute)])).Browsable != false)
                {
                    try
                    {
                        var NewValue = newInfo.GetType().GetProperty(arrInfo.Name).GetValue(newInfo, null);
                        var OleValue = oldIf.GetType().GetProperty(arrInfo.Name).GetValue(oldIf, null);
                        if ((NewValue != null && NewValue.ToString() != null))
                        {
                            PropertyInfo oinfo = oleType.GetProperty(arrInfo.Name);
                            oinfo.SetValue(oldIf, NewValue, null);
                        }
                    }
                    catch { }
                }
                i++;
            }
            return oldIf;
        }

        public static T OverPropery<T>(T oleInfo)
        {
            int i = 0;
            string ObjJson = JsonConvert.SerializeObject(oleInfo);
            T newIf = JsonConvert.DeserializeObject<T>(ObjJson);
            Type oleType = newIf.GetType();
            PropertyInfo[] PInfo = typeof(T).GetProperties();
            foreach (var arrInfo in PInfo)
            {
                AttributeCollection attributes = TypeDescriptor.GetProperties(newIf.GetType())[i].Attributes;
                if (((System.ComponentModel.BrowsableAttribute)(attributes[typeof(BrowsableAttribute)])).Browsable != false)
                {
                    var OleValue = newIf.GetType().GetProperty(arrInfo.Name).GetValue(newIf, null);
                    if ((OleValue == null || string.IsNullOrEmpty(OleValue.ToString())))
                    {
                        try
                        {
                            PropertyInfo oinfo = oleType.GetProperty(arrInfo.Name);
                            oinfo.SetValue(newIf, "", null);
                        }
                        catch { }
                    }
                }
                i++;
            }
            return newIf;
        }

        #region 私有方法

        private static readonly Dictionary<Type, string> _builtInTypeNames = new Dictionary<Type, string>
        {
            { typeof(bool), "bool" },
            { typeof(byte), "byte" },
            { typeof(char), "char" },
            { typeof(decimal), "decimal" },
            { typeof(double), "double" },
            { typeof(float), "float" },
            { typeof(int), "int" },
            { typeof(long), "long" },
            { typeof(object), "object" },
            { typeof(sbyte), "sbyte" },
            { typeof(short), "short" },
            { typeof(string), "string" },
            { typeof(uint), "uint" },
            { typeof(ulong), "ulong" },
            { typeof(ushort), "ushort" },
            { typeof(void), "void" }
        };

        private static void ProcessType(StringBuilder builder, Type type, bool fullName)
        {
            if (type.IsGenericType)
            {
                var genericArguments = type.GetGenericArguments();
                ProcessGenericType(builder, type, genericArguments, genericArguments.Length, fullName);
            }
            else if (type.IsArray)
            {
                ProcessArrayType(builder, type, fullName);
            }
            else if (_builtInTypeNames.TryGetValue(type, out var builtInName))
            {
                builder.Append(builtInName);
            }
            else if (!type.IsGenericParameter)
            {
                builder.Append(fullName ? type.FullName : type.Name);
            }
        }

        private static void ProcessArrayType(StringBuilder builder, Type type, bool fullName)
        {
            var innerType = type;
            while (innerType.IsArray)
            {
                innerType = innerType.GetElementType();
            }

            ProcessType(builder, innerType, fullName);

            while (type.IsArray)
            {
                builder.Append('[');
                builder.Append(',', type.GetArrayRank() - 1);
                builder.Append(']');
                type = type.GetElementType();
            }
        }

        private static void ProcessGenericType(StringBuilder builder, Type type, Type[] genericArguments, int length, bool fullName)
        {
            var offset = type.IsNested ? type.DeclaringType.GetGenericArguments().Length : 0;

            if (fullName)
            {
                if (type.IsNested)
                {
                    ProcessGenericType(builder, type.DeclaringType, genericArguments, offset, fullName);
                    builder.Append('+');
                }
                else
                {
                    builder.Append(type.Namespace);
                    builder.Append('.');
                }
            }

            var genericPartIndex = type.Name.IndexOf('`');
            if (genericPartIndex <= 0)
            {
                builder.Append(type.Name);
                return;
            }

            builder.Append(type.Name, 0, genericPartIndex);
            builder.Append('<');

            for (var i = offset; i < length; i++)
            {
                ProcessType(builder, genericArguments[i], fullName);
                if (i + 1 == length)
                {
                    continue;
                }

                builder.Append(',');
                if (!genericArguments[i + 1].IsGenericParameter)
                {
                    builder.Append(' ');
                }
            }

            builder.Append('>');
        }

        #endregion

        /// <summary>
        /// 将实体转换成字符串
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <returns></returns>
        public static string GetProperties<T>(T t)
        {
            string tStr = string.Empty;
            if (t == null)
            {
                return tStr;
            }
            PropertyInfo[] properties = t.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public);
            if (properties.Length <= 0)
            {
                return tStr;
            }
            foreach (PropertyInfo item in properties)
            {
                string name = item.Name;
                object value = item.GetValue(t, null);
                if (item.PropertyType.IsValueType || item.PropertyType.Name.StartsWith("String"))
                {
                    tStr += string.Format("{0}:{1},", name, value);
                }
                else
                {
                    GetProperties(value);
                }
            }
            return tStr;
        }

        /// <summary>
        /// 获取实体的属性值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <param name="field"></param>
        /// <returns></returns>
        public static string GetPropertyValue<T>(T t, string field)
        {
            try
            {
                string value = "9";
                if (t == null)
                {
                    return value;
                }
                PropertyInfo[] properties = t.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public);
                if (properties.Length <= 0)
                {
                    return value;
                }
                var property = properties.Where(x => x.Name == field).FirstOrDefault();
                value = property.GetValue(t, null).ToString();
                return value;
            }
            catch (Exception ex)
            {
                return "";
            }
        }

        /// <summary>
        /// 将实体转数据字典
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static Dictionary<string, object> ConvertObjectToDictionary<T>(T obj) where T : class
        {
            var dictionary = new Dictionary<string, object>();
            if (obj == null)
            {
                return dictionary;
            }
            foreach (PropertyInfo property in obj.GetType().GetProperties())
            {
                var value = property.GetValue(obj, null);
                dictionary.Add(property.Name, value);
            }
            return dictionary;
        }

        /// <summary>
        /// 将数据字典转实体
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dict"></param>
        /// <returns></returns>
        public static T ConvertDictionaryToObject<T>(Dictionary<string, object> dict) where T : class, new()
        {
            var obj = new T();
            if (dict == null)
            {
                return obj;
            }
            foreach (PropertyInfo property in obj.GetType().GetProperties())
            {
                if (dict.ContainsKey(property.Name))
                {
                    var value = dict[property.Name];
                    if (value != null && value.GetType() == property.PropertyType)
                    {
                        property.SetValue(obj, value);
                    }
                }
            }
            return obj;
        }


    }
}
