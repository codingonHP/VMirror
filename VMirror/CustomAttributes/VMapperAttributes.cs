using System;

namespace VMirror.CustomAttributes
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class Sync : Attribute
    {
        public String FieldName { get; set; }
    }

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Class)]
    public class Exclude : Attribute
    {
       public String FieldName { get; set; }
    }

}
