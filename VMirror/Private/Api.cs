using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using VMirror.CustomAttributes;
using VMirror.Extension;
using VMirror.Settings;

namespace VMirror.Private
{
    public class Api
    {
        private readonly List<String> _cacheMetadata;

        public Api(List<string> cacheMetadata)
        {
            _cacheMetadata = cacheMetadata;
        }

        public static String ProcessCustomAttributes(MemberInfo member)
        {
            var customAttrs = member.GetCustomAttributes();
            return customAttrs.ProcessAttrs();
        }

        public List<String> CacheAttributes(Type source, Type target, VConfig configSettings)
        {
            try
            {
                if (_cacheMetadata.Any())
                {
                    return _cacheMetadata;
                }

                var sourceMembers = source.GetMembers().Where(member => member.MemberType == MemberTypes.Field || member.MemberType == MemberTypes.Property).ToList();
                var targetMembers = target.GetMembers().Where(member => member.MemberType == MemberTypes.Field || member.MemberType == MemberTypes.Property).ToList();
                CacheType(configSettings.ReverseMap ? targetMembers : sourceMembers);
            }
            catch (Exception)
            {
                if (configSettings.UseStrict)
                {
                    throw;
                }
            }

            return _cacheMetadata;
        }

        private void CacheType(List<MemberInfo> members)
        {
            members.ForEach(srcMem =>
            {
                var customAttrsWithSrc = srcMem.GetCustomAttributes().ToList();
                var syncAttr = customAttrsWithSrc.Find(cAttr => typeof(Sync) == cAttr.GetType());

                if (syncAttr != null)
                {
                    var customSyncAttr = syncAttr as Sync;
                    if (customSyncAttr != null)
                    {
                        var syncWithField = customSyncAttr.FieldName;
                        _cacheMetadata.Add(syncWithField);
                    }
                }
                else
                {
                    var excludeAttr = customAttrsWithSrc.Find(cAttr => typeof(Exclude) == cAttr.GetType());
                    var excludeCustomAttr = excludeAttr as Exclude;
                    if (excludeCustomAttr == null)
                    {
                        //If Exclude is not applied to the property add to the cache list.
                        _cacheMetadata.Add(srcMem.Name);
                    }
                }
            });
        }
    }
}
