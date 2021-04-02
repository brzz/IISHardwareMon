using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Web;

 /// <summary>
    /// 一些类型转换函数如DataTable转List
    /// </summary>
    public class ConvertDef
    {
        /// <summary>  
        /// DataTable转化为List集合  
        /// </summary>  
        /// <typeparam name="T">实体对象</typeparam>  
        /// <param name="dt">datatable表</param>  
        /// <param name="isStoreDB">是否存入数据库datetime字段，date字段没事，取出不用判断</param>  
        /// <returns>返回list集合</returns>  
        public static List<T> TableToList<T>(DataTable dt, bool isStoreDB = true)
        {
            List<T> list = new List<T>();
            Type type = typeof(T);
            List<string> listColums = new List<string>();
            foreach (DataRow row in dt.Rows)
            {
                PropertyInfo[] pArray = type.GetProperties(); //集合属性数组  
                T entity = Activator.CreateInstance<T>(); //新建对象实例  
                foreach (PropertyInfo p in pArray)
                {
                    if (!dt.Columns.Contains(p.Name) || row[p.Name] == null || row[p.Name] == DBNull.Value)
                    {
                        continue;  //DataTable列中不存在集合属性或者字段内容为空则，跳出循环，进行下个循环  
                    }
                    if (isStoreDB && p.PropertyType == typeof(DateTime) && Convert.ToDateTime(row[p.Name]) < Convert.ToDateTime("1753-01-01"))
                    {
                        continue;
                    }
                    try
                    {
                        var obj = Convert.ChangeType(row[p.Name], p.PropertyType);//类型强转，将table字段类型转为集合字段类型  
                        p.SetValue(entity, obj, null);
                    }
                    catch (Exception)
                    {
                        // throw;  
                    }
                    //if (row[p.Name].GetType() == p.PropertyType)  
                    //{  
                    //    p.SetValue(entity, row[p.Name], null); //如果不考虑类型异常，foreach下面只要这一句就行  
                    //}  
                    //object obj = null;  
                    //if (ConvertType(row[p.Name], p.PropertyType,isStoreDB, out obj))  
                    //{                      
                    //    p.SetValue(entity, obj, null);  
                    //}  
                }
                list.Add(entity);
            }
            return list;
        }


        ///// <summary>  
        ///// List集合转DataTable  
        ///// </summary>  
        ///// <typeparam name="T">实体类型</typeparam>  
        ///// <param name="list">传入集合</param>  
        ///// <param name="isStoreDB">是否存入数据库DateTime字段，date时间范围没事，取出展示不用设置TRUE</param>  
        ///// <returns>返回datatable结果</returns>  
        //public static DataTable ListToTable<T>(List<T> list, bool isStoreDB = true)
        //{
        //    Type tp = typeof(T);
        //    PropertyInfo[] proInfos = tp.GetProperties();
        //    DataTable dt = new DataTable();
        //    foreach (var item in proInfos)
        //    {
        //        dt.Columns.Add(item.Name, item.PropertyType); //添加列明及对应类型  
        //    }
        //    foreach (var item in list)
        //    {
        //        DataRow dr = dt.NewRow();
        //        foreach (var proInfo in proInfos)
        //        {
        //           // object obj = proInfo.GetValue(item);
        //            object obj = proInfo.GetValue(item);
        //            if (obj == null)
        //            {
        //                continue;
        //            }
        //            //if (obj != null)  
        //            // {  
        //            if (isStoreDB && proInfo.PropertyType == typeof(DateTime) && Convert.ToDateTime(obj) < Convert.ToDateTime("1753-01-01"))
        //            {
        //                continue;
        //            }
        //            // dr[proInfo.Name] = proInfo.GetValue(item);  
        //            dr[proInfo.Name] = obj;
        //            // }  
        //        }
        //        dt.Rows.Add(dr);
        //    }
        //    return dt;
        //}

        /// <summary>  
        /// table指定行转对象  
        /// </summary>  
        /// <typeparam name="T">实体</typeparam>  
        /// <param name="dt">传入的表格</param>  
        /// <param name="rowindex">table行索引，默认为第一行</param>
        /// <param name="isStoreDB">是否存入数据库datetime字段，date字段没事，取出不用判断</param>  
        /// <returns>返回实体对象</returns>  
        public static T TableToEntity<T>(DataTable dt, int rowindex = 0, bool isStoreDB = true)
        {
            Type type = typeof(T);
            T entity = Activator.CreateInstance<T>(); //创建对象实例  
            if (dt == null)
            {
                return entity;
            }
            //if (dt != null)  
            //{  
            DataRow row = dt.Rows[rowindex]; //要查询的行索引  
            PropertyInfo[] pArray = type.GetProperties();
            foreach (PropertyInfo p in pArray)
            {
                if (!dt.Columns.Contains(p.Name) || row[p.Name] == null || row[p.Name] == DBNull.Value)
                {
                    continue;
                }

                if (isStoreDB && p.PropertyType == typeof(DateTime) && Convert.ToDateTime(row[p.Name]) < Convert.ToDateTime("1753-01-02"))
                {
                    continue;
                }
                try
                {
                    var obj = Convert.ChangeType(row[p.Name], p.PropertyType);//类型强转，将table字段类型转为对象字段类型  
                    p.SetValue(entity, obj, null);
                }
                catch (Exception)
                {
                    // throw;  
                }
                // p.SetValue(entity, row[p.Name], null);                     
            }
            //  }  
            return entity;
        }


    /// <summary>  
    /// Unix时间戳转为C#格式时间  
    /// </summary>  
    /// <param name="timeStamp">Unix时间戳格式,例如1482115779</param>  
    /// <returns>C#格式时间</returns>  
    public static DateTime GetTime(string timeStamp)
    {
        DateTime dtStart = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
        long lTime = long.Parse(timeStamp + "0000000");
        TimeSpan toNow = new TimeSpan(lTime);
        return dtStart.Add(toNow);
    }


    /// <summary>  
    /// DateTime时间格式转换为Unix时间戳格式  
    /// </summary>  
    /// <param name="time"> DateTime时间格式</param>  
    /// <returns>Unix时间戳格式</returns>  
    public static long ConvertDateTimeInt(System.DateTime time)
    {
        System.DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1));
        return (long)(time - startTime).TotalSeconds;
    }




}
