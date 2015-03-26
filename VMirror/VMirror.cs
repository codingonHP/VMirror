using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using VMirror.Extension;
using VMirror.Private;
using VMirror.Settings;

namespace VMirror
{
    public static class VMirror
    {

        public static void MapTo<TSource, TDestination>(this TSource parent, TDestination target, bool useStrict = true)
        {
            try
            {
                Type sourceType = typeof(TSource);
                Type destinationType = typeof(TDestination);

                var sourceMembers = sourceType.GetMembers().Where(m => m.MemberType == MemberTypes.Property || m.MemberType == MemberTypes.Field);

                foreach (var s in sourceMembers)
                {
                    String targetParam = Api.ProcessCustomAttributes(s);
                    if (targetParam.Equals(Constant.ExcludeFromMapping))
                    {
                        continue;
                    }

                    if (targetParam.Equals(Constant.CustomAttrNotApplied))
                    {
                        targetParam = s.Name;
                    }

                    var sourceValue = parent.GetPropValue(sourceType, s.Name);
                    target.SetPropValue(destinationType, targetParam, sourceValue);
                }
            }
            catch (Exception ex)
            {
                if (useStrict)
                {
                    throw new Exception(String.Format("Source and Destination are incompactable : " + ex.Message));
                }

            }
        }

        /// <summary>
        /// Map the source and target type
        /// </summary>
        /// <typeparam name="TSource">Source </typeparam>
        /// <typeparam name="TDestination">Target object</typeparam>
        /// <param name="parent"></param>
        /// <param name="target"></param>
        /// <param name="reverseMap">true if annotations are applied in target class</param>
        /// <param name="useStrict">non strict mode will supress errors, default is true</param>
        public static void MapTo<TSource, TDestination>(this TSource parent, TDestination target, bool useStrict, bool reverseMap)
        {
            try
            {
                if (!reverseMap)
                {
                    MapTo(parent, target, useStrict);
                    return;
                }

                Type sourceType = typeof(TSource);
                Type destinationType = typeof(TDestination);

                var destinationMembers = destinationType.GetMembers().Where(m => m.MemberType == MemberTypes.Property || m.MemberType == MemberTypes.Field);

                foreach (var d in destinationMembers)
                {
                    String param = Api.ProcessCustomAttributes(d);
                    if (param.Equals(Constant.ExcludeFromMapping))
                    {
                        continue;
                    }

                    if (param.Equals(Constant.CustomAttrNotApplied))
                    {
                        param = d.Name;
                    }

                    try
                    {
                        var sourceValue = parent.GetPropValue(sourceType, param);
                        target.SetPropValue(destinationType, d.Name, sourceValue);
                    }
                    catch (Exception)
                    {
                        if (useStrict)
                        {
                            throw;
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                throw new Exception(String.Format("Source and Destination are incompactable : " + ex.Message));
            }
        }

        public static void MapTo<TSource, TDestination>(this TSource parent, TDestination target, VConfig config)
        {
            parent.MapTo(target, config.UseStrict, config.ReverseMap);
        }

        public static void SyncCollection<TSource, TDestination>(this IEnumerable<TSource> parentCollection, ICollection<TDestination> target, bool useStrict, bool reverseMap)
        {
            Type sourceType = typeof (TSource);
            Type targetType = typeof (TDestination);
            var settings = new VConfig
            {
                UseStrict = useStrict,
                ReverseMap = reverseMap
            };
            var mappingFields = new List<string>();

            var cache = new Api(mappingFields);
            cache.CacheAttributes(sourceType, targetType, settings);

            var i = 0;
            foreach (var item in parentCollection)
            {
                var instance = Activator.CreateInstance<TDestination>();
                item.MapTo(instance, useStrict, reverseMap);
                //instance.FillType(item, sourceType, targetType, mappingFields, settings);
                target.Add(instance);
                Debug.WriteLine(++i);
            }
        }

        public static void SyncCollection<TSource, TDestination>(this IEnumerable<TSource> parentCollection, ICollection<TDestination> target, VConfig config)
        {
            parentCollection.MapTo(target, config.UseStrict, config.ReverseMap);
        }


    }
}
