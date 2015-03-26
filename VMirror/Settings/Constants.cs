using System;

namespace VMirror.Settings
{
    public static class Constant
    {
        public static String ExcludeFromMapping = "ExcludeFromMapping";
        public static String ExclusionNotApplied = "ProhibitExclusion";
        public static String CustomAttrNotApplied = "NoCustomAttrApplied";
    }

    public class VConfig
    {
        public bool UseStrict { get; set; }
        public bool ReverseMap { get; set; }
    }

}
