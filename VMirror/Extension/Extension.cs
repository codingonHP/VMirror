using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using VMirror.CustomAttributes;
using VMirror.Framework;
using VMirror.Settings;

namespace VMirror.Extension
{
    public static class MirrorApiExtension
    {
        public static HandlerOutput<string> ProcessAttrs(this IEnumerable<Attribute> customAttrs)
        {
            try
            {
                foreach (var attr in customAttrs)
                {
                    var method = attr as IHandler<String>;
                    if (method != null)
                    {
                       var handle = method.CustomAttrHandler();
                        return handle;
                    }
                }

                return null;
            }
            catch (Exception)
            {
              throw new Exception("Failed to Decide if Custom Attribute is applied or not");
            }
          
        }

        public static Object GetPropValue(this Object obj, Type sourceType, String name)
        {
            foreach (String part in name.Split('.'))
            {
                if (obj == null) { return null; }

                PropertyInfo info = sourceType.GetProperty(part);
                if (info == null) { return null; }

                obj = info.GetValue(obj, null);
            }
            return obj;
        }

        public static void SetPropValue(this Object obj, Type type, String name, object value)
        {

            try
            {
                if (obj == null) throw new Exception("Object cannot be null");
                var info = type.GetProperty(name);
                if (info == null) { throw new Exception("Field not found : " + name); }
                info.SetValue(obj, value);
            }
            catch (Exception)
            {
                throw;
            }
          
        }

        public static void FillType(this Object target, Object source, Type sourceType, Type targetType, Dictionary<String,String> propertyNames, VConfig config)
        {
            try
            {
                foreach (var prop in propertyNames)
                {
                    try
                    {
                       var value =  GetPropValue(source, sourceType, prop.Key);
                       SetPropValue(target, targetType,prop.Value,value);
                    }
                    catch (Exception)
                    {
                        if (config.UseStrict)
                        {
                            throw;
                        }    
                    }
                   
                }
            }
            catch (Exception)
            {
                if (config.UseStrict)
                {
                    throw;
                }    
               
            }
           
        }
    }

    public static class LinqExtension
    {
        public static IEnumerable<TSource> SelectTop<TSource>(this IEnumerable<TSource> source, int topRecordCount )
        {
            int count = -1;
            return source.TakeWhile(item => ++count != topRecordCount);
        }
    }
}
