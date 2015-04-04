using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using VMirror.CustomAttributes;
using VMirror.Extension;
using VMirror.Framework;
using VMirror.Settings;

namespace VMirror.Private
{
    public class Api
    {
        private readonly Dictionary<String, String> _cacheMetadata;

        public Api()
        {
            _cacheMetadata = new Dictionary<string, string>();
        }

        public static HandlerOutput<string> ProcessCustomAttributes(MemberInfo member)
        {
            var customAttrs = member.GetCustomAttributes();
            return customAttrs.ProcessAttrs();
        }

        public Dictionary<String, String> CacheAttributes(Type source, Type target, VConfig configSettings)
        {
            try
            {
                if (_cacheMetadata.Any())
                {
                    return _cacheMetadata;
                }

                var sourceMembers = source.GetMembers().Where(member => member.MemberType == MemberTypes.Field || member.MemberType == MemberTypes.Property).ToList();
                var targetMembers = target.GetMembers().Where(member => member.MemberType == MemberTypes.Field || member.MemberType == MemberTypes.Property).ToList();
                CacheType(sourceMembers, targetMembers);
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

        private void CacheType(List<MemberInfo> srcMembers, List<MemberInfo> targetMembers)
        {
            srcMembers.ForEach(srcMem =>
            {
                var appliedCustomAttrs = srcMem.GetCustomAttributes().ToList();

                if (appliedCustomAttrs.Any())
                {
                    Process(_cacheMetadata,srcMem, appliedCustomAttrs);
                }
                else
                {
                    _cacheMetadata.Add(srcMem.Name,String.Empty);   
                }
            });

            targetMembers.ForEach(tarMem =>
            {
                var appliedCustomAttrs = tarMem.GetCustomAttributes().ToList();
                if (appliedCustomAttrs.Any())
                {
                    Process(_cacheMetadata, tarMem, appliedCustomAttrs);
                }
                else
                {
                    if (!_cacheMetadata.ContainsKey(tarMem.Name))
                    {
                        _cacheMetadata.Add(tarMem.Name, String.Empty);
                    }
                    else
                    {
                        _cacheMetadata[tarMem.Name] = tarMem.Name;
                    }
                }
               
            });

            var badKeys = _cacheMetadata.Keys.Where(item => String.IsNullOrEmpty(_cacheMetadata[item])).ToList();
            badKeys.ForEach(key => _cacheMetadata.Remove(key));
        }

        private void Process(Dictionary<String, String> holder, MemberInfo member, IEnumerable<Attribute> appliedCustomAttrs)
        {
            foreach (var attr in appliedCustomAttrs)
            {
                var handler = attr as IHandler<String>;
                if (handler != null)
                {
                    var result = handler.CustomAttrHandler();
                    if (!result.ExcludeFromMapping)
                    {
                        if (!holder.ContainsKey(result.FieldName))
                        {
                            holder.Add(result.FieldName, member.Name);
                        }
                        else
                        {
                            holder[result.FieldName] = member.Name;
                        }
                    }

                }
            }
        }
    }
}
