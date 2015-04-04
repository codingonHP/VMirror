using System;
using VMirror.Framework;

namespace VMirror.CustomAttributes
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class Sync : Attribute, IHandler<String>
    {
        public String FieldName { get; set; }

        public HandlerOutput<string> CustomAttrHandler()
        {
            return new HandlerOutput<string>
            {
                FieldName = FieldName,
                ExcludeFromMapping = false
            };
        }
    }


    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Class)]
    public class Exclude : Attribute, IHandler<String>
    {
       public String FieldName { get; set; }

       public HandlerOutput<string> CustomAttrHandler()
       {
           return new HandlerOutput<string>
           {
               ExcludeFromMapping = true
           };
       }
    }

}
