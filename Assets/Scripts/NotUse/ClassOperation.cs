using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace MyTools
{
    public static class ClassOperation
    {
        /// <summary>
        /// 获取类的所有属性名
        /// </summary>
        /// <returns></returns>
        public static List<string> GetClassFieldesName<T>(T t)
        {
            if (t == null)
            {
                return null;
            }
            FieldInfo[] fields = t.GetType().GetFields();
            List<string> nameList = new List<string>();
            foreach (FieldInfo p in fields)
            {
                nameList.Add(p.Name);
            }
            return nameList;
        }

        /// <summary>
        /// 获取类的所有属性以数组的型式返回
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <returns></returns>
        public static FieldInfo[] GetClassFields<T>(T t)
        {
            FieldInfo[] fields = t.GetType().GetFields();
            if (fields.Length <= 0)//当类中没有属性的时候
            {
                return null;
            }
            return fields;
        }
        /// <summary>
        /// 获取类的属性以list型式返回
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static List<FieldInfo> GetClassFields<T>()
        {
            Type type = typeof(T);
            return type.GetFields().ToList();
        }

        /// <summary>
        /// 获取无实体类的名字
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static string GetClassName<T>()
        {
            Type type = typeof(T);
            return type.Name;
        }

        /// <summary>
        /// 获取实体类的名字
        /// </summary>
        /// <returns></returns>
        public static string GetClassName(object obj)
        {
            //去掉点，根据点拆分字符串
            var strs = obj.ToString().Split('.');
            //取最后一个字符串
            var objName = strs[strs.Length - 1];
            //返回类的名称
            return objName;
        }

        /// <summary>
        /// 获取无实体的类型，用于泛型
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static Type GetType<T>()
        {
            return typeof(T);
        }
        /// <summary>
        /// 获取有实例的类型
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static Type GetType<T>(T Entity)
        {
            return Entity.GetType();
        }

        /// <summary>
        /// 用于类的二进制流转换
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static MemoryStream SerializeBinary(object request)
        {
            BinaryFormatter serializer = new BinaryFormatter();
            MemoryStream memStream = new MemoryStream();
            serializer.Serialize(memStream, request);
            return memStream;
        }
        /// <summary>
        /// 用于将二进制流转换成类
        /// </summary>
        /// <param name="memStream"></param>
        /// <returns></returns>
        public static object DeSerializeBinary(System.IO.MemoryStream memStream)
        {
            BinaryFormatter deserializer = new BinaryFormatter();
            deserializer.Binder = new UBinder();
            object newObject = deserializer.Deserialize(memStream);
            return newObject;
        }
             
        public class UBinder : SerializationBinder
        {
            public override Type BindToType(string assemblyName, string typeName)
            {
                Assembly ass = Assembly.GetExecutingAssembly();
                return ass.GetType(typeName);
            }
        }
    }
}
