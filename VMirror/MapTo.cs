using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VMirror
{
    public class ToMap : Attribute
    {
        String TargetName { get; set; }

        public ToMap(String targetName)
        {
            TargetName = targetName;
        }
    }
}
