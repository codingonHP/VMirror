using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VMirror.Framework
{
    public  interface IHandler<T>
    {
        HandlerOutput<T> CustomAttrHandler();
    }
}
