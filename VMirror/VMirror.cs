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
            var config = new VConfig
            {
                UseStrict = useStrict
            };
            MapTo(parent, target, config);
        }

        public static void MapTo<TSource, TDestination>(this TSource parent, TDestination target, VConfig config)
        {
            try
            {
                Type sourceType = typeof(TSource);
                Type targetType = typeof(TDestination);
               
                var cache = new Api();
                var mappingFields = cache.CacheAttributes(sourceType, targetType, config);
                target.FillType(parent, sourceType, targetType, mappingFields, config);

            }
            catch (Exception ex)
            {
                if (config.UseStrict)
                {
                    throw new Exception(String.Format("Source and Destination are incompactable : " + ex.Message));
                }

            }
        }

        public static void SyncCollection<TSource, TDestination>(this IEnumerable<TSource> sourceCollection, ICollection<TDestination> targetCollection, VConfig settings)
        {
            Type sourceType = typeof (TSource);
            Type targetType = typeof (TDestination);
           
            var cache = new Api();
            var mappingFields = cache.CacheAttributes(sourceType, targetType, settings);

            foreach (var item in sourceCollection)
            {
                var instance = Activator.CreateInstance<TDestination>();
                instance.FillType(item, sourceType, targetType, mappingFields, settings);
                targetCollection.Add(instance);
            
            }
        }

        public static void SyncCollection<TSource, TDestination>(this IEnumerable<TSource> sourceCollection,ICollection<TDestination> targetCollection,bool useStrict = true)
        {
            var config = new VConfig
            {
                UseStrict = useStrict
            };

            SyncCollection(sourceCollection, targetCollection, config);
        }

    }
}
