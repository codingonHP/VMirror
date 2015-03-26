using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using VMirror.CustomAttributes;
using VMirror.Settings;

namespace VMirror.Extension
{
    public static class MirrorApiExtension
    {
        public static String ProcessAttrs<TSource>(this IEnumerable<TSource> customAttrs)
        {
            String customAttribute = Constant.CustomAttrNotApplied;
            String mapField = String.Empty;

            try
            {
                foreach (var attr in customAttrs)
                {
                    var type = attr.GetType();

                    if (type == typeof(Exclude))
                    {
                        customAttribute = Constant.ExcludeFromMapping;
                    }

                    if (type == typeof(Sync))
                    {
                        var customAttr = attr as Sync;
                        if (customAttr != null)
                        {
                            mapField = customAttr.FieldName;
                        }
                    }
                }

                return String.IsNullOrEmpty(mapField) ? customAttribute : mapField;
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

        public static void FillType(this Object target, Object source, Type sourceType, Type targetType, List<String> propertyName, VConfig config)
        {
            try
            {
                foreach (var prop in propertyName)
                {
                    try
                    {
                       var value =  GetPropValue(source, sourceType, prop);
                       SetPropValue(target, targetType,prop,value);
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
