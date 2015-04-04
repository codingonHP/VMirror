using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VMirror.Framework
{
    public class HandlerOutput<T>
    {
        public int Status { get; set; }
        public String FieldName { get; set; }
        public bool ExcludeFromMapping { get; set; }
        public object GlobalHandler { get; set; }
        public List<T> StoreList{ get; set; }
    }
}
